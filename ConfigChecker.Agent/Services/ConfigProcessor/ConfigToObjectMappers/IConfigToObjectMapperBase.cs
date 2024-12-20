namespace ConfigChecker.Agent.Services.ConfigProcessor.ConfigToObjectMappers;

public interface IConfigToObjectMapperBase
{
    public abstract string[] ConfigsToProcess { get; }
    public abstract object? MapFileToObject(string filePath);
}