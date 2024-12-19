namespace ConfigChecker.Agent.Services;

public interface IAgentFileService
{
    string[] GetConfigFileValue(string key);
}