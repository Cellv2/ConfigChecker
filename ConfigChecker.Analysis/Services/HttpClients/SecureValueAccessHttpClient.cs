using System.Text.Json;

namespace ConfigChecker.Analysis.Services.HttpClients;

public sealed class SecureValueAccessHttpClient : ISecureValueAccessHttpClient
{
    private readonly HttpClient httpClient;

    public SecureValueAccessHttpClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri("https://securevalueaccessor.webapi:8081");
    }

    public async Task<string?> GetValueForKeyFromRedis(string redisKeyToCheck)
    {
        //redisKeyToCheck = JsonSerializer.Serialize(new { key = "key1" });
        HttpContent content = new StringContent(redisKeyToCheck, System.Text.Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("/getRedisValueByKey", content);
        var responseVal = await response.Content.ReadAsStringAsync();

        Console.WriteLine(responseVal);

        if (responseVal == null)
        {
            return null;
        }

        return responseVal;
    }
}
