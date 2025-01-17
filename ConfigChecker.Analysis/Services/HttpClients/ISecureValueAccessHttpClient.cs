
namespace ConfigChecker.Analysis.Services.HttpClients
{
    public interface ISecureValueAccessHttpClient
    {
        Task GetValueFromRedis(string value);
    }
}