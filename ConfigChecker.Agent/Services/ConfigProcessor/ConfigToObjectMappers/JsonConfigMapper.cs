using ConfigChecker.Agent.Constants;
using System.Text.Json;

namespace ConfigChecker.Agent.Services.ConfigProcessor.ConfigToObjectMappers
{
    public sealed class JsonConfigMapper : IConfigToObjectMapperBase
    {
        public string[] ConfigsToProcess => ConfigSearch.JsonConfigFileNamesToSearch;

        public object? MapFileToObject(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            if (!ConfigsToProcess.Contains(fileName))
            {
                return null;
            }

            var fileContent = File.ReadAllText(filePath);
            var deserialized = JsonSerializer.Deserialize<object>(fileContent);

            return deserialized;
        }
    }
}
