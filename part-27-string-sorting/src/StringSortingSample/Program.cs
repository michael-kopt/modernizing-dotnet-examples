using StringSortingSample.Sorting;

var values = new List<string>
{
    "test-1",
    "test",
    "Test-2",
    "TEST",
    "cafe",
    "café",
    "Cafe",
    "ábaco",
    "abaco"
};

Console.WriteLine("Original:");
Console.WriteLine(string.Join(", ", values));
Console.WriteLine();

var defaultSorted = new List<string>(values);
defaultSorted.Sort(StringComparer.CurrentCulture);
Console.WriteLine("CurrentCulture Sort:");
Console.WriteLine(string.Join(", ", defaultSorted));
Console.WriteLine();

var nlsSorted = new List<string>(values);
nlsSorted.Sort(NlsLikeStringComparer.Instance);
Console.WriteLine("NlsLikeStringComparer Sort:");
Console.WriteLine(string.Join(", ", nlsSorted));
