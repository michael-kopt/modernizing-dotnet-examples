using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Xml;
using Part05.CoreWcfDocsSample.Contracts;

namespace Part05.CoreWcfDocsSample.Services;

public sealed class SoapDocumentationService(IHttpClientFactory httpClientFactory)
{
    private readonly SemaphoreSlim _buildLock = new(1, 1);
    private string? _indexPage;
    private Dictionary<string, string>? _operationPages;

    public async Task<string> GetIndexPageAsync()
    {
        await EnsureBuiltAsync();
        return _indexPage ?? "<html><body><h1>No operations found.</h1></body></html>";
    }

    public async Task<string> GetOperationPageAsync(string operationName)
    {
        await EnsureBuiltAsync();

        if (_operationPages is not null && _operationPages.TryGetValue(operationName, out var html))
        {
            return html;
        }

        return $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8">
          <title>Operation Not Found</title>
        </head>
        <body>
          <h1>Operation Not Found</h1>
          <p>No documentation page exists for <strong>{{operationName}}</strong>.</p>
          <p><a href="/SoapService.asmx">Back to operation list</a></p>
        </body>
        </html>
        """;
    }

    private async Task EnsureBuiltAsync()
    {
        if (_indexPage is not null && _operationPages is not null)
        {
            return;
        }

        await _buildLock.WaitAsync();
        try
        {
            if (_indexPage is not null && _operationPages is not null)
            {
                return;
            }

            var wsdlContent = await LoadWsdlAsync();
            var operations = LoadOperationMetadata(wsdlContent);

            _indexPage = BuildIndexPage(operations);
            _operationPages = operations.ToDictionary(
                operation => operation.Name,
                operation => BuildOperationPage(operation),
                StringComparer.Ordinal);
        }
        finally
        {
            _buildLock.Release();
        }
    }

    private async Task<string> LoadWsdlAsync()
    {
        using var client = httpClientFactory.CreateClient();
        return await client.GetStringAsync("http://localhost:5380/SoapService.asmx?wsdl");
    }

    private static List<OperationDocumentation> LoadOperationMetadata(string wsdlContent)
    {
        var contractType = typeof(ISoapService);
        var methods = contractType.GetMethods();
        var operationDescriptions = methods.ToDictionary(
            method => method.Name,
            method => method.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty,
            StringComparer.Ordinal);

        var wsdl = new XmlDocument();
        wsdl.LoadXml(wsdlContent);

        var ns = new XmlNamespaceManager(wsdl.NameTable);
        ns.AddNamespace("wsdl", "http://schemas.xmlsoap.org/wsdl/");
        ns.AddNamespace("s", "http://www.w3.org/2001/XMLSchema");
        ns.AddNamespace("tns", "http://www.namespace.com/");

        var operations = new List<OperationDocumentation>();

        foreach (var method in methods)
        {
            var requestElement = wsdl.SelectSingleNode(
                $"//s:element[@name='{method.Name}']",
                ns) as XmlElement;
            var responseElement = wsdl.SelectSingleNode(
                $"//s:element[@name='{method.Name}Response']",
                ns) as XmlElement;

            var requestBody = requestElement is null
                ? "<!-- Request schema not found -->"
                : BuildElementExample(requestElement, ns, 3);
            var responseBody = responseElement is null
                ? "<!-- Response schema not found -->"
                : BuildElementExample(responseElement, ns, 3);

            operations.Add(new OperationDocumentation(
                method.Name,
                operationDescriptions.GetValueOrDefault(method.Name, string.Empty),
                $$"""
                <?xml version="1.0" encoding="utf-8"?>
                <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                               xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                               xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
                  <soap:Body>
                {{requestBody}}
                  </soap:Body>
                </soap:Envelope>
                """,
                $$"""
                <?xml version="1.0" encoding="utf-8"?>
                <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                               xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                               xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
                  <soap:Body>
                {{responseBody}}
                  </soap:Body>
                </soap:Envelope>
                """));
        }

        return operations;
    }

    private static string BuildIndexPage(List<OperationDocumentation> operations)
    {
        var items = new StringBuilder();
        foreach (var operation in operations)
        {
            items.AppendLine($"<li><a href=\"/SoapService.asmx?op={operation.Name}\">{operation.Name}</a><br><span>{HtmlEncode(operation.Description)}</span></li>");
        }

        return $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8">
          <title>SoapService</title>
          <style>
            body { font-family: Georgia, serif; margin: 2rem auto; max-width: 900px; line-height: 1.5; color: #1d2433; }
            h1 { margin-bottom: 0.5rem; }
            ul { padding-left: 1.25rem; }
            li { margin-bottom: 1rem; }
            span { color: #4f5d75; }
            code, pre { font-family: Consolas, monospace; }
            a { color: #0b57d0; text-decoration: none; }
            a:hover { text-decoration: underline; }
          </style>
        </head>
        <body>
          <h1>SoapService</h1>
          <p>The following operations are supported. For a formal definition, review the generated <a href="/SoapService.asmx?wsdl">WSDL</a>.</p>
          <ul>
        {{items}}
          </ul>
        </body>
        </html>
        """;
    }

    private static string BuildOperationPage(OperationDocumentation operation)
    {
        return $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8">
          <title>{{operation.Name}}</title>
          <style>
            body { font-family: Georgia, serif; margin: 2rem auto; max-width: 900px; line-height: 1.5; color: #1d2433; }
            h1, h2 { margin-bottom: 0.5rem; }
            pre { background: #f6f8fb; border: 1px solid #d6dcea; padding: 1rem; overflow: auto; }
            code, pre { font-family: Consolas, monospace; }
            a { color: #0b57d0; text-decoration: none; }
            a:hover { text-decoration: underline; }
          </style>
        </head>
        <body>
          <h1>{{operation.Name}}</h1>
          <p>{{HtmlEncode(operation.Description)}}</p>
          <p><a href="/SoapService.asmx">Back to operation list</a></p>
          <h2>SOAP 1.1 Request</h2>
          <pre>{{HtmlEncode(operation.RequestExample)}}</pre>
          <h2>SOAP 1.1 Response</h2>
          <pre>{{HtmlEncode(operation.ResponseExample)}}</pre>
        </body>
        </html>
        """;
    }

    private static string BuildElementExample(XmlElement element, XmlNamespaceManager ns, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 2);
        var elementName = element.GetAttribute("name");
        var builder = new StringBuilder();
        builder.AppendLine($"{indent}<{elementName} xmlns=\"http://www.namespace.com/\">");

        var sequence = element.SelectSingleNode(".//s:sequence", ns);
        if (sequence is not null)
        {
            foreach (var child in sequence.ChildNodes.OfType<XmlElement>())
            {
                var childName = child.GetAttribute("name");
                var value = GetSampleValue(child.GetAttribute("type"));
                builder.AppendLine($"{indent}  <{childName}>{value}</{childName}>");
            }
        }

        builder.Append($"{indent}</{elementName}>");
        return builder.ToString();
    }

    private static string GetSampleValue(string typeName)
    {
        return typeName switch
        {
            "s:string" => "string",
            "s:int" => "0",
            "s:boolean" => "false",
            _ => "value"
        };
    }

    private static string HtmlEncode(string value)
    {
        return System.Net.WebUtility.HtmlEncode(value);
    }

    private sealed record OperationDocumentation(
        string Name,
        string Description,
        string RequestExample,
        string ResponseExample);
}
