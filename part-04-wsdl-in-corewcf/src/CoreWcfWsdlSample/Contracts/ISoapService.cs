using System.ComponentModel;
using CoreWCF;

namespace Part04.CoreWcfWsdlSample.Contracts;

[ServiceContract(Namespace = "http://www.namespace.com/", ConfigurationName = "ISoapService", Name = "SoapService")]
[Description("A SOAP webservice.")]
public interface ISoapService
{
    [OperationContract(Action = "http://www.namespace.com/Login")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    [Description("Log in to this web service. Pass the user name and password.")]
    string Login(string username, string password);
}
