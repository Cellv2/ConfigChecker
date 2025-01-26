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
        var serializedKeyToCheck = JsonSerializer.Serialize(new { key = redisKeyToCheck });
        HttpContent content = new StringContent(serializedKeyToCheck, System.Text.Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("/getRedisValueByKey", content);
        var responseVal = await response.Content.ReadAsStringAsync();
        var deserializedResponseVal = JsonSerializer.Deserialize<string>(responseVal);

        Console.WriteLine(deserializedResponseVal);

        if (deserializedResponseVal == null)
        {
            return null;
        }

        return deserializedResponseVal;
    }
}
