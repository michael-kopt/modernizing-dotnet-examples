using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using DictionaryToRedisSample.Models;

namespace DictionaryToRedisSample.Services;

public sealed class DistributedTokenManager(IDistributedCache cache)
{
    private const string TokenKeyPrefix = "app:token:session:";
    private const string UserKeyPrefix = "app:user:session:";
    private const string TokenCountKey = "app:token:count";

    private static readonly TimeSpan TokenSlidingExpiry = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan TokenAbsoluteExpiry = TimeSpan.FromHours(24);

    private static readonly DistributedCacheEntryOptions TokenExpiryOptions = new()
    {
        SlidingExpiration = TokenSlidingExpiry,
        AbsoluteExpirationRelativeToNow = TokenAbsoluteExpiry
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        WriteIndented = false
    };

    public async Task StoreUserAsync(string token, User user)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        var userKey = $"{UserKeyPrefix}{token}";
        var userJson = JsonSerializer.Serialize(user, JsonOptions);

        await cache.SetStringAsync(userKey, userJson, TokenExpiryOptions);
        await IncrementTokenCountAsync();
    }

    public async Task<User?> GetUserAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var userKey = $"{UserKeyPrefix}{token}";
        var userJson = await cache.GetStringAsync(userKey);

        if (string.IsNullOrWhiteSpace(userJson))
        {
            return null;
        }

        await cache.RefreshAsync(userKey);
        return JsonSerializer.Deserialize<User>(userJson, JsonOptions);
    }

    public async Task StoreTokenTimestampAsync(string token, DateTime timestampUtc)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        var tokenKey = $"{TokenKeyPrefix}{token}";
        await cache.SetAsync(tokenKey, BitConverter.GetBytes(timestampUtc.ToBinary()), TokenExpiryOptions);
    }

    public async Task<DateTime?> GetTokenTimestampAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var tokenKey = $"{TokenKeyPrefix}{token}";
        var value = await cache.GetAsync(tokenKey);

        if (value is null)
        {
            return null;
        }

        await cache.RefreshAsync(tokenKey);
        return DateTime.FromBinary(BitConverter.ToInt64(value, 0));
    }

    public async Task RemoveTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        var tokenKey = $"{TokenKeyPrefix}{token}";
        var userKey = $"{UserKeyPrefix}{token}";

        await cache.RemoveAsync(tokenKey);
        await cache.RemoveAsync(userKey);
        await DecrementTokenCountAsync();
    }

    public async Task UpdateTokenTimestampAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        var tokenKey = $"{TokenKeyPrefix}{token}";
        var userKey = $"{UserKeyPrefix}{token}";

        await cache.RefreshAsync(tokenKey);
        await cache.RefreshAsync(userKey);
    }

    public async Task<int> GetActiveTokenCountAsync()
    {
        var countValue = await cache.GetStringAsync(TokenCountKey);

        if (string.IsNullOrWhiteSpace(countValue))
        {
            return 0;
        }

        return int.TryParse(countValue, out var count) ? count : 0;
    }

    private async Task IncrementTokenCountAsync()
    {
        var currentCount = await GetActiveTokenCountAsync();
        currentCount++;
        await cache.SetStringAsync(TokenCountKey, currentCount.ToString(), TokenExpiryOptions);
    }

    private async Task DecrementTokenCountAsync()
    {
        var currentCount = await GetActiveTokenCountAsync();
        if (currentCount <= 0)
        {
            return;
        }

        currentCount--;
        await cache.SetStringAsync(TokenCountKey, currentCount.ToString(), TokenExpiryOptions);
    }
}
