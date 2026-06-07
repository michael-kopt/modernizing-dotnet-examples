# Part 10: Replacing OleDB Excel Reading with ExcelDataReader

Article: [Modernizing .NET - Part 10 Replacing OleDB Excel Reading with ExcelDataReader](https://medium.com/@michael.kopt/modernizing-net-part-10-replacing-oledb-excel-reading-with-exceldatareader-a2cdae1e2cb8)

This sample replaces the classic OleDB Excel import pattern with `ExcelDataReader` and demonstrates the same practical tasks covered in the article:

- opening XLS/XLSX files
- reading all worksheets into a `DataSet`
- formatting cell values
- exporting a worksheet to a delimited file
- streaming large files row by row

## Commands

Summary:

```powershell
dotnet run --project .\src\ExcelDataReaderSample\ExcelDataReaderSample.csproj -- summary .\tests\sample.xlsx
```

Export first worksheet to pipe-delimited text:

```powershell
dotnet run --project .\src\ExcelDataReaderSample\ExcelDataReaderSample.csproj -- export .\tests\sample.xlsx .\tests\sample.txt
```

Stream all rows without loading the full workbook:

```powershell
dotnet run --project .\src\ExcelDataReaderSample\ExcelDataReaderSample.csproj -- stream .\tests\sample.xlsx
```

## Notes

- The sample registers `CodePagesEncodingProvider` at startup, which is required for legacy XLS support.
- `summary` and `export` use `AsDataSet()`, which is convenient for small and medium files.
- `stream` uses forward-only reading, which is the better pattern for large workbooks.
- `tests/sample.xlsx` is a tiny fixture workbook with two sheets so the sample can be exercised immediately.
