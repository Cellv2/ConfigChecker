
namespace ConfigChecker.Analysis.Services.HttpClients;

public interface ISecureValueAccessHttpClient
{
    Task<string?> GetValueForKeyFromRedis(string value);
}