using System.Reflection;
using CoreWCF.Dispatcher;

namespace Part06.CoreWcfValidationSample.Validation;

public sealed class CoreWcfParameterInspector(MethodInfo methodInfo) : IParameterInspector
{
    private readonly string[] _inputNames = methodInfo
        .GetParameters()
        .Select(parameter => parameter.Name ?? string.Empty)
        .ToArray();

    public object? BeforeCall(string operationName, object[] inputs)
    {
        var inputsDictionary = _inputNames
            .Zip(inputs, (name, value) => new { name, value })
            .ToDictionary(
                item => item.name.ToLowerInvariant(),
                item => item.value,
                StringComparer.Ordinal);

        var assertions = methodInfo
            .GetCustomAttributes(true)
            .OfType<AssertAttribute>()
            .ToList();

        foreach (var assertion in assertions)
        {
            if (inputsDictionary.TryGetValue(assertion.Name.ToLowerInvariant(), out var value))
            {
                new AssertValidator(assertion).Validate(value);
            }
        }

        return null;
    }

    public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
    {
    }
}
