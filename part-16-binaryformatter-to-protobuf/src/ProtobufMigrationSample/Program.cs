using ProtobufMigrationSample.Models;
using ProtobufMigrationSample.Services;

var command = args.FirstOrDefault()?.ToLowerInvariant() ?? "all";

var output = command switch
{
    "session" => DemoRunner.RunSessionDemo(),
    "file" => DemoRunner.RunFileDemo(args.Skip(1).FirstOrDefault() ?? @".\tests\order.bin"),
    "inheritance" => DemoRunner.RunInheritanceDemo(),
    "versioning" => DemoRunner.RunVersioningDemo(),
    "all" => string.Join(
        Environment.NewLine + Environment.NewLine,
        DemoRunner.RunSessionDemo(),
        DemoRunner.RunFileDemo(@".\tests\order.bin"),
        DemoRunner.RunInheritanceDemo(),
        DemoRunner.RunVersioningDemo()),
    _ => """
         Unknown command.
         Supported commands: session, file, inheritance, versioning, all
         """
};

Console.WriteLine(output);
