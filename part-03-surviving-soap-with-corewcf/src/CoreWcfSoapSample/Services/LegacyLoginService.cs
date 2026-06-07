namespace Part03.CoreWcfSoapSample.Services;

public sealed class LegacyLoginService
{
    public string Login(string username, string password)
    {
        if (username == "demo" && password == "pass123")
        {
            return "LOGIN_OK:demo-token";
        }

        return "INVALID_CREDENTIALS";
    }
}
