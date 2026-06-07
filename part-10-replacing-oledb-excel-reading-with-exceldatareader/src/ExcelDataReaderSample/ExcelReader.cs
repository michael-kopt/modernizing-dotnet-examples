using System.Data;
using ExcelDataReader;
using ExcelDataReader.Exceptions;

namespace Part10.ExcelDataReaderSample;

public sealed class ExcelReader
{
    public DataSet ReadExcelFile(string filePath)
    {
        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            UseColumnDataType = true,
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                UseHeaderRow = true
            }
        });

        return dataSet;
    }

    public DataSet SafeReadExcel(string filePath)
    {
        try
        {
            return ReadExcelFile(filePath);
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine($"File access error: {ex.Message}");
            throw;
        }
        catch (ExcelReaderException ex)
        {
            Console.Error.WriteLine($"Excel format error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }

    public void ProcessAllSheets(string filePath)
    {
        var dataSet = SafeReadExcel(filePath);
        foreach (DataTable table in dataSet.Tables)
        {
            Console.WriteLine($"Sheet: {table.TableName}, Columns: {table.Columns.Count}, Rows: {table.Rows.Count}");
        }
    }

    public void ExportToDelimitedFile(DataTable table, string outputPath, char delimiter = '|')
    {
        using var writer = new StreamWriter(outputPath);

        var headers = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
        writer.WriteLine(string.Join(delimiter, headers));

        foreach (DataRow row in table.Rows)
        {
            var values = new List<string>();
            for (var i = 0; i < table.Columns.Count; i++)
            {
                var cleaned = FormatCellValue(row[i])
                    .Replace('"', '\'')
                    .Replace(delimiter, '-')
                    .Trim();

                if (cleaned.Contains('\r') || cleaned.Contains('\n'))
                {
                    cleaned = $"\"{cleaned}\"";
                }

                values.Add(cleaned);
            }

            writer.WriteLine(string.Join(delimiter, values));
        }
    }

    public void ProcessLargeExcel(string filePath)
    {
        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        do
        {
            var rowCount = 0;
            while (reader.Read())
            {
                rowCount++;
                var values = new List<string>();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    values.Add(FormatCellValue(reader.GetValue(i)));
                }

                Console.WriteLine(string.Join(" | ", values));
            }

            Console.WriteLine($"Processed {rowCount} rows from current sheet.");
        } while (reader.NextResult());
    }

    public DataTable GetFirstWorksheet(string filePath)
    {
        var dataSet = SafeReadExcel(filePath);
        if (dataSet.Tables.Count == 0)
        {
            throw new InvalidOperationException("The workbook does not contain any worksheets.");
        }

        return dataSet.Tables[0];
    }

    private static string FormatCellValue(object? cellValue)
    {
        if (cellValue is null || cellValue == DBNull.Value)
        {
            return string.Empty;
        }

        return cellValue switch
        {
            DateTime dt when dt.TimeOfDay == TimeSpan.Zero => dt.ToString("yyyy/MM/dd"),
            DateTime dt => dt.ToString("yyyy/MM/dd HH:mm:ss"),
            double num when num == (int)num => ((int)num).ToString(),
            double num => num.ToString("0.00"),
            _ => cellValue.ToString() ?? string.Empty
        };
    }
}
