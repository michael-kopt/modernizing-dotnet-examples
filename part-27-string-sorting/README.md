# Part 27: Cross-Platform String Sorting Nuances

Article: [Modernizing .NET - Part 27 Cross-Platform String Sorting Nuances](https://medium.com/@michael.kopt/modernizing-net-part-27-cross-platform-string-sorting-nuances-0f4f4a1c6c5a)

This sample demonstrates the migration pattern from the article: stop relying on platform-default string ordering when business behavior depends on Windows-style sorting, and use a deterministic custom comparer instead.

## Sample Focus

- A custom comparer that approximates legacy Windows NLS-style ordering.
- A demo that contrasts default sorting with the custom comparer.
- Tests that lock in stable, cross-platform expectations.

## Implementation Notes

- `NlsLikeStringComparer` performs a case-insensitive primary comparison.
- Hyphens and em dashes are ignored in the primary comparison when comparing letters.
- Lowercase sorts before uppercase as a secondary tie-breaker.
- Basic Latin diacritic variants are normalized to their base letters during primary comparison.

## Structure

```text
src/StringSortingSample
tests/StringSortingSample.Tests
```

## Run

```powershell
dotnet run --project .\src\StringSortingSample
```

## Output

The demo prints:

- the original list
- the platform-default `Sort(StringComparer.CurrentCulture)` result
- the stable custom-comparer result

## Notes

- The comparer is intentionally focused on the migration scenario from the article. It is not a full reimplementation of Windows NLS.
- The goal is deterministic business behavior across Windows and Linux, not perfect linguistic collation for every locale.
