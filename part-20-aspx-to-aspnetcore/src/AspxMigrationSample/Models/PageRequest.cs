using Microsoft.AspNetCore.Http;

namespace AspxMigrationSample.Models;

public sealed record PageRequest(bool IsLite, string EventTarget, string EventArgument, IFormCollection Form);
