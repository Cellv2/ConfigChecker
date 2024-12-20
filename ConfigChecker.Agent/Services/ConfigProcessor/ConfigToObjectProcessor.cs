using ConfigChecker.Agent.Services.ConfigProcessor.ConfigToObjectMappers;

namespace ConfigChecker.Agent.Services.ConfigProcessor;

public sealed class ConfigToObjectProcessor(IEnumerable<IConfigToObjectMapperBase> configMappers) : IConfigMappingProcessor
{
    public dynamic[] ProcessConfigsPathsToObjects(string[] configPaths)
    {
        List<object> dynamicObjects = [];

        foreach (var configPath in configPaths)
        {
            var mapped = MapConfigPathToObject(configPath);
            if (mapped != null)
            {
                dynamicObjects.Add(mapped);
            }
        }

        return dynamicObjects.ToArray();
    }

    private object? MapConfigPathToObject(string configPath)
    {
        foreach (var configMapper in configMappers)
        {
            var result = configMapper.MapFileToObject(configPath);
            if (result != null)
            {
                return result;
            }
        }

        Console.WriteLine($"Unable to resolve mapper for {configPath}");
        return null;
    }
}
