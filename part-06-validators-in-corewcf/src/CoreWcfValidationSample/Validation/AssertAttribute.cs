namespace Part06.CoreWcfValidationSample.Validation;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class AssertAttribute(
    string name,
    AssertType assertType,
    AssertComparisonOperator comparisonOperator,
    int value,
    string message) : Attribute
{
    public string Name { get; } = name;
    public AssertType AssertType { get; } = assertType;
    public AssertComparisonOperator ComparisonOperator { get; } = comparisonOperator;
    public int Value { get; } = value;
    public string Message { get; } = message;
}

public enum AssertType
{
    StringLength,
    Number
}

public enum AssertComparisonOperator
{
    Less,
    LessOrEqual,
    More,
    MoreOrEqual,
    Equal,
    NotEqual
}
