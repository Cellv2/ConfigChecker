using ConfigChecker.Agent.Constants;

namespace ConfigChecker.Agent.Services.ConfigProcessor.ConfigToObjectMappers;

public sealed class XmlConfigMapper : IConfigToObjectMapperBase
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

        throw new NotImplementedException("Need an XML mapper");
    }
}
