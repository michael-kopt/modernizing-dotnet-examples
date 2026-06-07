using DictionaryToRedisSample.Models;

namespace DictionaryToRedisSample.Services;

public sealed class SessionHelper(DistributedTokenManager tokenManager)
{
    public async Task<string> GenerateLoginTokenAsync(User user)
    {
        var token = Guid.NewGuid().ToString("N");
        await tokenManager.StoreUserAsync(token, user);
        await tokenManager.StoreTokenTimestampAsync(token, DateTime.UtcNow);
        return token;
    }

    public Task<User?> GetUserAsync(string token)
    {
        return tokenManager.GetUserAsync(token);
    }

    public Task LogoutAsync(string token)
    {
        return tokenManager.RemoveTokenAsync(token);
    }

    public async Task<bool> IsValidTokenAsync(string token)
    {
        var timestamp = await tokenManager.GetTokenTimestampAsync(token);
        if (timestamp is null)
        {
            return false;
        }

        if ((DateTime.UtcNow - timestamp.Value).TotalMinutes >= 10)
        {
            return false;
        }

        await tokenManager.UpdateTokenTimestampAsync(token);
        return true;
    }
}
