using CoreWCF;
using Part07.SoapServiceHost.Contracts;

namespace Part07.SoapServiceHost.Services;

[ServiceBehavior(IncludeExceptionDetailInFaults = false, Name = "SoapService")]
public class SoapServiceController(LegacyLoginService legacyLoginService) : ISoapService
{
    public string Login(string username, string password)
    {
        return legacyLoginService.Login(username, password);
    }
}
