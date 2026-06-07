using ProtobufMigrationSample.Models;

namespace ProtobufMigrationSample.Services;

public static class DemoRunner
{
    public static string RunSessionDemo()
    {
        var session = new ProtoSessionState();
        var report = new SessionReport
        {
            ReportName = "QuarterlyRevenue",
            CreatedAtUtc = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc),
            Sections = ["Summary", "Top Customers", "Exceptions"]
        };

        session.Set("CurrentReport", report);
        var restored = session.Get<SessionReport>("CurrentReport");

        return
            $"[session] restored={restored is not null}, " +
            $"name={restored?.ReportName}, " +
            $"sections={restored?.Sections.Count}, " +
            $"payloadBytes={session.GetRaw("CurrentReport")?.Length ?? 0}";
    }

    public static string RunFileDemo(string path)
    {
        var fullPath = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        var order = new Order
        {
            OrderId = 42,
            Items =
            [
                new OrderItem { ProductId = "A-100", Quantity = 2, Price = 19.95m },
                new OrderItem { ProductId = "B-200", Quantity = 1, Price = 120.00m }
            ],
            Metadata = new Dictionary<string, string>
            {
                ["source"] = "legacy-cache",
                ["region"] = "eu"
            }
        };

        using (var stream = File.Create(fullPath))
        {
            var protobufStream = BinaryUtils.ObjectToStream(order)!;
            protobufStream.CopyTo(stream);
        }

        using var input = File.OpenRead(fullPath);
        var restored = BinaryUtils.StreamToObject<Order>(input);

        return
            $"[file] path={fullPath}, " +
            $"orderId={restored.OrderId}, " +
            $"items={restored.Items.Count}, " +
            $"metadataKeys={restored.Metadata.Count}";
    }

    public static string RunInheritanceDemo()
    {
        var employees = new List<Employee>
        {
            new Manager { Id = 1, Name = "Grace", TeamSize = 6 },
            new Developer { Id = 2, Name = "Ada", ProgrammingLanguage = "C#" }
        };

        var bytes = BinaryUtils.ObjectToByteArray(employees)!;
        var restored = BinaryUtils.ByteArrayToObject<List<Employee>>(bytes);

        return
            $"[inheritance] types={string.Join(",", restored.Select(employee => employee.GetType().Name))}, " +
            $"payloadBytes={bytes.Length}";
    }

    public static string RunVersioningDemo()
    {
        var legacyUser = new UserV1
        {
            Id = 7,
            Name = "Michael"
        };

        var bytes = BinaryUtils.ObjectToByteArray(legacyUser)!;
        var upgraded = BinaryUtils.ByteArrayToObject<UserV2>(bytes);

        return
            $"[versioning] id={upgraded.Id}, " +
            $"name={upgraded.Name}, " +
            $"email='{upgraded.Email}', " +
            $"payloadBytes={bytes.Length}";
    }
}
