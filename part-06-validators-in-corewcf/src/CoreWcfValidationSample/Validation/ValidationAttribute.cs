using System.Reflection;
using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;

namespace Part06.CoreWcfValidationSample.Validation;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ValidationAttribute : Attribute, IOperationBehavior
{
    public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
    {
    }

    public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
    {
        var methodInfo = operationDescription.SyncMethod
            ?? operationDescription.TaskMethod
            ?? operationDescription.BeginMethod;

        if (methodInfo is null)
        {
            throw new InvalidOperationException($"No service method could be resolved for operation '{operationDescription.Name}'.");
        }

        dispatchOperation.ParameterInspectors.Add(new CoreWcfParameterInspector(methodInfo));
    }

    public void Validate(OperationDescription operationDescription)
    {
    }
}
