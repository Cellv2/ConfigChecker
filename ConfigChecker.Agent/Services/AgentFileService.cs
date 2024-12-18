namespace ConfigChecker.Agent.Services;

public sealed class AgentFileService
{
    public string GetConfigFileValue(string key)
    {
        return key switch
        {
            "thing" => "_thing_",
            "thing2" => "_thing2",
            _ => "unknown"
        };
    }
}
