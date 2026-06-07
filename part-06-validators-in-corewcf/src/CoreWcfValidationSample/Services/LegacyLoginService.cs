namespace Part06.CoreWcfValidationSample.Services;

public sealed class LegacyLoginService
{
    public int CallCount { get; private set; }

    public string Login(string username, string password)
    {
        CallCount++;

        if (username == "demo" && password == "pass123")
        {
            return "LOGIN_OK:demo-token";
        }

        return "INVALID_CREDENTIALS";
    }

    public void Reset()
    {
        CallCount = 0;
    }
}
