using System.Text;
using System.Xml;

namespace Part05.CoreWcfDocsSample.Middleware;

public sealed class WsdlCustomizerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!IsWsdlRequest(context.Request))
        {
            await next(context);
            return;
        }

        RewriteQueryString(context.Request);

        var originalBody = context.Response.Body;
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        try
        {
            await next(context);

            if (context.Response.StatusCode != StatusCodes.Status200OK)
            {
                buffer.Position = 0;
                await buffer.CopyToAsync(originalBody);
                return;
            }

            buffer.Position = 0;
            var xml = await new StreamReader(buffer, Encoding.UTF8).ReadToEndAsync();
            var customized = CustomizeWsdl(xml, context.Request);

            context.Response.Body = originalBody;
            context.Response.ContentLength = Encoding.UTF8.GetByteCount(customized);
            context.Response.ContentType = "text/xml; charset=utf-8";
            await context.Response.WriteAsync(customized);
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private static bool IsWsdlRequest(HttpRequest request)
    {
        var query = request.QueryString.Value ?? string.Empty;
        return query.Contains("wsdl", StringComparison.OrdinalIgnoreCase);
    }

    private static void RewriteQueryString(HttpRequest request)
    {
        var query = request.QueryString.Value ?? string.Empty;
        if (query.Contains("?wsdl", StringComparison.OrdinalIgnoreCase))
        {
            request.QueryString = new QueryString(
                query.Replace("?wsdl", "?singleWsdl", StringComparison.OrdinalIgnoreCase));
        }
    }

    private static string CustomizeWsdl(string xml, HttpRequest request)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        var nsManager = CreateNamespaceManager(xmlDoc);

        RenameMessageNodes(xmlDoc, nsManager);
        RenameBinding(xmlDoc, nsManager);
        RenamePortBindingReference(xmlDoc, nsManager);
        RewriteSoapAddress(xmlDoc, nsManager, request);
        RemovePolicyNodes(xmlDoc, nsManager);

        using var writer = new Utf8StringWriter();
        using var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings
        {
            OmitXmlDeclaration = false,
            Encoding = Encoding.UTF8
        });

        xmlDoc.Save(xmlWriter);
        xmlWriter.Flush();
        return writer.ToString();
    }

    private static XmlNamespaceManager CreateNamespaceManager(XmlDocument xmlDoc)
    {
        var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
        nsManager.AddNamespace("wsdl", "http://schemas.xmlsoap.org/wsdl/");
        nsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/wsdl/soap/");
        nsManager.AddNamespace("wsp", "http://schemas.xmlsoap.org/ws/2004/09/policy");
        nsManager.AddNamespace("wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
        return nsManager;
    }

    private static void RenameMessageNodes(XmlDocument xmlDoc, XmlNamespaceManager nsManager)
    {
        var messageNodes = xmlDoc.SelectNodes("//wsdl:message", nsManager);
        if (messageNodes is null)
        {
            return;
        }

        foreach (var messageNode in messageNodes)
        {
            if (messageNode is not XmlElement element)
            {
                continue;
            }

            var newName = element.GetAttribute("name")
                .Replace("SoapService_", string.Empty, StringComparison.Ordinal)
                .Replace("_InputMessage", "SoapIn", StringComparison.Ordinal)
                .Replace("_OutputMessage", "SoapOut", StringComparison.Ordinal);

            element.SetAttribute("name", newName);
        }
    }

    private static void RenameBinding(XmlDocument xmlDoc, XmlNamespaceManager nsManager)
    {
        if (xmlDoc.SelectSingleNode("//wsdl:binding[@name='BasicHttpBinding_SoapService']", nsManager) is XmlElement bindingElement)
        {
            bindingElement.SetAttribute("name", "SoapServiceSoap");

            var typeValue = bindingElement.GetAttribute("type");
            if (string.Equals(typeValue, "tns:BasicHttpBinding_SoapService", StringComparison.Ordinal))
            {
                bindingElement.SetAttribute("type", "tns:SoapServiceSoap");
            }
        }
    }

    private static void RenamePortBindingReference(XmlDocument xmlDoc, XmlNamespaceManager nsManager)
    {
        if (xmlDoc.SelectSingleNode("//wsdl:service[@name='SoapService']/wsdl:port", nsManager) is XmlElement portElement)
        {
            portElement.SetAttribute("name", "SoapServiceSoap");

            if (portElement.HasAttribute("binding"))
            {
                portElement.SetAttribute("binding", "tns:SoapServiceSoap");
            }
        }
    }

    private static void RewriteSoapAddress(XmlDocument xmlDoc, XmlNamespaceManager nsManager, HttpRequest request)
    {
        if (xmlDoc.SelectSingleNode("//wsdl:service[@name='SoapService']/wsdl:port/soap:address", nsManager) is not XmlElement addressElement)
        {
            return;
        }

        var baseUrl = Environment.GetEnvironmentVariable("baseURL");
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = $"{request.Scheme}://{request.Host}/";
        }

        baseUrl = baseUrl.TrimEnd('/') + "/";
        addressElement.SetAttribute("location", $"{baseUrl}SoapService.asmx");
    }

    private static void RemovePolicyNodes(XmlDocument xmlDoc, XmlNamespaceManager nsManager)
    {
        var policyNodes = xmlDoc.SelectNodes("//wsp:Policy", nsManager);
        if (policyNodes is not null)
        {
            foreach (var node in policyNodes.Cast<XmlNode>().ToArray())
            {
                node.ParentNode?.RemoveChild(node);
            }
        }

        var policyReferenceNodes = xmlDoc.SelectNodes("//wsp:PolicyReference", nsManager);
        if (policyReferenceNodes is not null)
        {
            foreach (var node in policyReferenceNodes.Cast<XmlNode>().ToArray())
            {
                node.ParentNode?.RemoveChild(node);
            }
        }
    }

    private sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
