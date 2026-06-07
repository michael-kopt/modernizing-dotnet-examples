using System.ComponentModel;
using CoreWCF;
using Part06.CoreWcfValidationSample.Validation;

using Check = Part06.CoreWcfValidationSample.Validation.AssertType;
using Operator = Part06.CoreWcfValidationSample.Validation.AssertComparisonOperator;

namespace Part06.CoreWcfValidationSample.Contracts;

[ServiceContract(Namespace = "http://www.namespace.com/", ConfigurationName = "ISoapService", Name = "SoapService")]
[Description("A SOAP webservice.")]
public interface ISoapService
{
    [OperationContract(Action = "http://www.namespace.com/Login")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document, Use = OperationFormatUse.Literal)]
    [Description("Log in to this web service. Pass the user name and password.")]
    [Validation]
    [Assert("username", Check.StringLength, Operator.More, 0, "Username must not be empty.")]
    [Assert("username", Check.StringLength, Operator.Less, 100, "Username must be shorter than 100 characters.")]
    [Assert("password", Check.StringLength, Operator.More, 3, "Password must be longer than 3 characters.")]
    [Assert("password", Check.StringLength, Operator.Less, 20, "Password must be shorter than 20 characters.")]
    string Login(string username, string password);
}
