using CoreWCF;
using Part05.CoreWcfDocsSample.Contracts;

namespace Part05.CoreWcfDocsSample.Services;

[ServiceBehavior(IncludeExceptionDetailInFaults = false, Name = "SoapService")]
public class SoapServiceController(LegacyLoginService legacyLoginService) : ISoapService
{
    public string Login(string username, string password)
    {
        return legacyLoginService.Login(username, password);
    }
}
