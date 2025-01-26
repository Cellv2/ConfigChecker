using ConfigChecker.Analysis.Services.HttpClients;

namespace ConfigChecker.Analysis.Services.SecureValueAccess;

public sealed class SecureValueAccessService(ISecureValueAccessHttpClient secureValueAccessHttpClient) : ISecureValueAccessService
{
    public async Task<bool> DoesConfigValueMatch(string redisKey, string configValue)
    {
        var redisValue = await secureValueAccessHttpClient.GetValueForKeyFromRedis(redisKey);
        if (redisValue == null)
        {
            Console.WriteLine($"Panic, redis value was null for {redisKey}");
            return false;
        }

        // TODO: this won't like things that aren't strings
        // TODO: think about how to achieve this chain with dynamic
        if (string.Equals(redisValue, configValue, StringComparison.Ordinal))
        {
            Console.WriteLine($"MATCH - Redis and config values DO match for key: {redisKey}");
            return true;
        }

        //if (redisValue == configValue)
        //{
        //    Console.WriteLine($"MATCH - Redis and config values DO match for key: {redisKey}");
        //    return true;
        //}

        Console.WriteLine($"NOT MATCH - Redis and config values do NOT match for key: {redisKey}");
        return false;
    }
}
