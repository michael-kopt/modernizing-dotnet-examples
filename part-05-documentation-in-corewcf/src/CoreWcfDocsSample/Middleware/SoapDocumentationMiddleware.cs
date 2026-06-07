using Part05.CoreWcfDocsSample.Services;

namespace Part05.CoreWcfDocsSample.Middleware;

public sealed class SoapDocumentationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, SoapDocumentationService docs)
    {
        if (!HttpMethods.IsGet(context.Request.Method) ||
            !context.Request.Path.Equals("/SoapService.asmx", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Query.ContainsKey("wsdl") ||
            context.Request.Query.ContainsKey("singleWsdl"))
        {
            await next(context);
            return;
        }

        var op = context.Request.Query["op"].ToString();
        var html = string.IsNullOrWhiteSpace(op)
            ? await docs.GetIndexPageAsync()
            : await docs.GetOperationPageAsync(op);

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(html);
    }
}
