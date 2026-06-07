using CoreWCF;
using Part03.CoreWcfSoapSample.Contracts;

namespace Part03.CoreWcfSoapSample.Services;

[ServiceBehavior(IncludeExceptionDetailInFaults = false, Name = "SoapService")]
public class SoapServiceController(LegacyLoginService legacyLoginService) : ISoapService
{
    public string Login(string username, string password)
    {
        return legacyLoginService.Login(username, password);
    }
}
