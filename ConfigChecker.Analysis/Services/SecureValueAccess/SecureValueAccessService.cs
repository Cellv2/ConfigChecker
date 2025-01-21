using ConfigChecker.Analysis.Services.HttpClients;

namespace ConfigChecker.Analysis.Services.SecureValueAccess;

public sealed class SecureValueAccessService(ISecureValueAccessHttpClient secureValueAccessHttpClient) : ISecureValueAccessService
{
    public async Task<bool> DoesConfigValueMatch(string redisKey, dynamic configValue)
    {
        var redisValue = await secureValueAccessHttpClient.GetValueForKeyFromRedis(redisKey);
        if (redisValue == null)
        {
            Console.WriteLine($"Panic, redis value was null for {redisKey}");
            return false;
        }

        // TODO: this won't like things that aren't strings
        //if (string.Equals(redisValue, configValue, StringComparison.Ordinal))
        //{
        //    Console.WriteLine($"Redis and config values match for key: {redisKey}");
        //    return true;
        //}

        // is this dumb?
        // yes ? - Microsoft.CSharp.RuntimeBinder.RuntimeBinderException: Operator '==' cannot be applied to operands of type 'string' and 'System.Text.Json.JsonElement'
        // get rid of the cast
        if (redisValue == configValue)
        {
            Console.WriteLine($"Redis and config values match for key: {redisKey}");
            return true;
        }

        Console.WriteLine($"Redis and config values do NOT match for key: {redisKey}");
        return false;
    }
}
