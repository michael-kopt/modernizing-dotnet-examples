using CoreWCF;
using Part06.CoreWcfValidationSample.Contracts;

namespace Part06.CoreWcfValidationSample.Services;

[ServiceBehavior(IncludeExceptionDetailInFaults = false, Name = "SoapService")]
public class SoapServiceController(LegacyLoginService legacyLoginService) : ISoapService
{
    public string Login(string username, string password)
    {
        return legacyLoginService.Login(username, password);
    }
}
