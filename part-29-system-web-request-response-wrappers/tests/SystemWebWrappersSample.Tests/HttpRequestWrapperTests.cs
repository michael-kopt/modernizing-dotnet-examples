using Microsoft.AspNetCore.Http;
using Xunit;

namespace SystemWebWrappersSample.Tests;

public class HttpRequestWrapperTests
{
    [Fact]
    public void Params_MergesQueryCookiesAndHeaders()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/legacy/request";
        context.Request.QueryString = new QueryString("?id=42");
        context.Request.Headers["X-Test"] = "header";
        context.Request.Headers.Cookie = "session=cookie";

        var accessor = new HttpContextAccessor { HttpContext = context };
        System.Web.HttpContext.Configure(accessor);

        var request = new System.Web.HttpRequest();

        Assert.Equal("42", request.Params["id"]);
        Assert.Equal("cookie", request.Params["session"]);
        Assert.Equal("header", request.Params["X-Test"]);
        Assert.Equal("/legacy/request?id=42", request.RawUrl);
    }
}
