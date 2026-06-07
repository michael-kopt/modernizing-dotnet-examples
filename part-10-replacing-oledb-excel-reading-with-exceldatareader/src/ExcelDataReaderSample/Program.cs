using System.Text;
using Part10.ExcelDataReaderSample;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage:");
    Console.Error.WriteLine("  summary <excelPath>");
    Console.Error.WriteLine("  export <excelPath> <outputPath>");
    Console.Error.WriteLine("  stream <excelPath>");
    return 1;
}

var command = args[0].ToLowerInvariant();
var filePath = args[1];
var excelReader = new ExcelReader();

return command switch
{
    "summary" => RunSummary(excelReader, filePath),
    "export" => RunExport(excelReader, filePath, args),
    "stream" => RunStream(excelReader, filePath),
    _ => UnknownCommand(command)
};

static int RunSummary(ExcelReader excelReader, string filePath)
{
    excelReader.ProcessAllSheets(filePath);
    return 0;
}

static int RunExport(ExcelReader excelReader, string filePath, string[] args)
{
    if (args.Length < 3)
    {
        Console.Error.WriteLine("Missing outputPath for export command.");
        return 1;
    }

    var outputPath = args[2];
    var firstWorksheet = excelReader.GetFirstWorksheet(filePath);
    excelReader.ExportToDelimitedFile(firstWorksheet, outputPath);
    Console.WriteLine($"Exported worksheet '{firstWorksheet.TableName}' to '{outputPath}'.");
    return 0;
}

static int RunStream(ExcelReader excelReader, string filePath)
{
    excelReader.ProcessLargeExcel(filePath);
    return 0;
}

static int UnknownCommand(string command)
{
    Console.Error.WriteLine($"Unknown command '{command}'.");
    return 1;
}
