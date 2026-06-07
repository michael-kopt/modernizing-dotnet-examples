namespace DictionaryToRedisSample.Models;

public sealed record CacheMode(bool RedisActive, string RedisConnectionString);
