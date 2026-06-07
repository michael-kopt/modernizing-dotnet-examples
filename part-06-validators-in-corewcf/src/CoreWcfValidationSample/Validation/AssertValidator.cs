using CoreWCF;

namespace Part06.CoreWcfValidationSample.Validation;

public sealed class AssertValidator(AssertAttribute attribute)
{
    public void Validate(object? value)
    {
        switch (attribute.AssertType)
        {
            case AssertType.StringLength:
            {
                var str = Convert.ToString(value) ?? string.Empty;
                ValidateNumber(str.Length);
                break;
            }
            case AssertType.Number:
            {
                var number = Convert.ToInt32(value);
                ValidateNumber(number);
                break;
            }
            default:
                throw new FaultException($"Unsupported assert type: {attribute.AssertType}");
        }
    }

    private void ValidateNumber(int number)
    {
        var result = Compare(attribute.ComparisonOperator, number, attribute.Value);
        if (!result)
        {
            throw new FaultException(attribute.Message);
        }
    }

    public static bool Compare<T>(AssertComparisonOperator op, T left, T right)
        where T : IComparable<T>
    {
        return op switch
        {
            AssertComparisonOperator.Less => left.CompareTo(right) < 0,
            AssertComparisonOperator.LessOrEqual => left.CompareTo(right) <= 0,
            AssertComparisonOperator.More => left.CompareTo(right) > 0,
            AssertComparisonOperator.MoreOrEqual => left.CompareTo(right) >= 0,
            AssertComparisonOperator.Equal => left.Equals(right),
            AssertComparisonOperator.NotEqual => !left.Equals(right),
            _ => false
        };
    }
}
