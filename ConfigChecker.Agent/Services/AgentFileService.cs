namespace ConfigChecker.Agent.Services;

public sealed class AgentFileService : IAgentFileService
{
    public string[] GetConfigFileValue(string clientCode)
    {
        var configFilePaths = TryGetConfigPaths(clientCode);
        if (configFilePaths.Length == 0)
        {
            return ["Unknown"];
        }

        return configFilePaths;
    }

    private string[] TryGetConfigPaths(string clientCode)
    {
        var configFiles = GetConfigFilePathsForClientCode(clientCode);

        if (configFiles.Length == 0)
        {
            Console.WriteLine("No config matches");
            return [];
        }

        return configFiles;
    }

    private string[] GetConfigFilePathsForClientCode(string clientCode)
    {
        var dirsToSearch = Constants.ConfigSearch.DirectoriesToSearch;
        var configFileNamesToSearch = Constants.ConfigSearch.JsonConfigFileNamesToSearch;

        List<string> configPaths = [];

        var clientDirs = GetDirectoriesByClientCode(clientCode);

        foreach (var clientDir in clientDirs)
        {
            foreach (var fileNameToSearch in configFileNamesToSearch)
            {
                var searchPattern = string.Join("*", fileNameToSearch);
                var configs = Directory.EnumerateFileSystemEntries(clientDir, searchPattern, SearchOption.AllDirectories);

                configPaths.AddRange(configs);
            }
        }

        return configPaths.ToArray();
    }

    private string[] GetDirectoriesByClientCode(string clientCode)
    {
        var dirsToSearch = Constants.ConfigSearch.DirectoriesToSearch;
        List<string> targetDirectories = [];

        foreach (var searchDir in dirsToSearch)
        {
            var dir = Directory.EnumerateDirectories(searchDir, clientCode, SearchOption.TopDirectoryOnly);
            targetDirectories.AddRange(dir);
        }

        return targetDirectories.ToArray();
    }
}
