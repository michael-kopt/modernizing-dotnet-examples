using System.Globalization;
using System.Text;

namespace StringSortingSample.Sorting;

public sealed class NlsLikeStringComparer : IComparer<string>
{
    public static NlsLikeStringComparer Instance { get; } = new();

    private NlsLikeStringComparer()
    {
    }

    public int Compare(string? x, string? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        var leftLetterPrimary = BuildLetterPrimaryKey(x);
        var rightLetterPrimary = BuildLetterPrimaryKey(y);

        var primaryComparison = string.Compare(leftLetterPrimary, rightLetterPrimary, StringComparison.OrdinalIgnoreCase);
        if (primaryComparison != 0)
        {
            return primaryComparison;
        }

        var caseProfileComparison = GetCaseProfile(x).CompareTo(GetCaseProfile(y));
        if (caseProfileComparison != 0)
        {
            return caseProfileComparison;
        }

        var leftContentPrimary = BuildContentPrimaryKey(x);
        var rightContentPrimary = BuildContentPrimaryKey(y);
        var contentComparison = string.Compare(leftContentPrimary, rightContentPrimary, StringComparison.OrdinalIgnoreCase);
        if (contentComparison != 0)
        {
            return contentComparison;
        }

        var accentComparison = CountAccents(x).CompareTo(CountAccents(y));
        if (accentComparison != 0)
        {
            return accentComparison;
        }

        var secondaryComparison = CompareWithLowercaseFirst(x, y);
        if (secondaryComparison != 0)
        {
            return secondaryComparison;
        }

        return string.Compare(x, y, StringComparison.Ordinal);
    }

    private static string BuildLetterPrimaryKey(string value)
    {
        var builder = new StringBuilder(value.Length);

        foreach (var character in value)
        {
            if (!char.IsLetter(character))
            {
                continue;
            }

            builder.Append(NormalizeCharacter(character));
        }

        return builder.ToString();
    }

    private static string BuildContentPrimaryKey(string value)
    {
        var builder = new StringBuilder(value.Length);

        foreach (var character in value)
        {
            if (character is '-' or '—')
            {
                continue;
            }

            builder.Append(NormalizeCharacter(character));
        }

        return builder.ToString();
    }

    private static char NormalizeCharacter(char character)
    {
        var decomposed = character.ToString().Normalize(NormalizationForm.FormD);

        foreach (var item in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(item) != UnicodeCategory.NonSpacingMark)
            {
                return item;
            }
        }

        return character;
    }

    private static int CompareWithLowercaseFirst(string left, string right)
    {
        var max = Math.Min(left.Length, right.Length);

        for (var index = 0; index < max; index++)
        {
            var leftChar = left[index];
            var rightChar = right[index];

            if (char.ToUpperInvariant(leftChar) != char.ToUpperInvariant(rightChar))
            {
                continue;
            }

            if (leftChar == rightChar)
            {
                continue;
            }

            if (char.IsLetter(leftChar) && char.IsLetter(rightChar))
            {
                if (char.IsLower(leftChar) && char.IsUpper(rightChar))
                {
                    return -1;
                }

                if (char.IsUpper(leftChar) && char.IsLower(rightChar))
                {
                    return 1;
                }
            }
        }

        return left.Length.CompareTo(right.Length);
    }

    private static int GetCaseProfile(string value)
    {
        var letters = value.Where(char.IsLetter).ToArray();
        if (letters.Length == 0)
        {
            return 1;
        }

        if (letters.All(char.IsLower))
        {
            return 0;
        }

        if (letters.All(char.IsUpper))
        {
            return 2;
        }

        return 1;
    }

    private static int CountAccents(string value)
    {
        var accentCount = 0;

        foreach (var character in value)
        {
            var decomposed = character.ToString().Normalize(NormalizationForm.FormD);
            accentCount += decomposed.Count(item => CharUnicodeInfo.GetUnicodeCategory(item) == UnicodeCategory.NonSpacingMark);
        }

        return accentCount;
    }
}
