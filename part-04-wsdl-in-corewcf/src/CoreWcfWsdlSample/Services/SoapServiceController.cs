using CoreWCF;
using Part04.CoreWcfWsdlSample.Contracts;

namespace Part04.CoreWcfWsdlSample.Services;

[ServiceBehavior(IncludeExceptionDetailInFaults = false, Name = "SoapService")]
public class SoapServiceController(LegacyLoginService legacyLoginService) : ISoapService
{
    public string Login(string username, string password)
    {
        return legacyLoginService.Login(username, password);
    }
}
