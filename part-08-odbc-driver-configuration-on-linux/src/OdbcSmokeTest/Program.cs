using System.Data.Odbc;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: OdbcSmokeTest \"Driver={...};Server=...;Database=...;...\"");
    return 1;
}

var connectionString = args[0];

try
{
    using var connection = new OdbcConnection(connectionString);
    Console.WriteLine("Opening ODBC connection...");
    connection.Open();
    Console.WriteLine("Connection opened successfully.");
    Console.WriteLine($"Driver: {connection.Driver}");
    Console.WriteLine($"Server version: {connection.ServerVersion}");
    return 0;
}
catch (OdbcException ex)
{
    Console.Error.WriteLine("ODBC connection failed.");
    foreach (OdbcError error in ex.Errors)
    {
        Console.Error.WriteLine($"SQLSTATE={error.SQLState}; NativeError={error.NativeError}; Message={error.Message}");
    }

    return 2;
}
catch (Exception ex)
{
    Console.Error.WriteLine("Unexpected error while opening ODBC connection.");
    Console.Error.WriteLine(ex.Message);
    return 3;
}
