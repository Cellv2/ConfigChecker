namespace ConfigChecker.Agent.Services.ConfigProcessor;

public interface IConfigMappingProcessor
{
    dynamic[] ProcessConfigsPathsToObjects(string[] configPaths);
}