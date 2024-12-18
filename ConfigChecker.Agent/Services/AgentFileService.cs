using ConfigChecker.Agent.Models;

namespace ConfigChecker.Agent.Services;

public sealed class AgentFileService
{
    public string GetConfigFileValue(string key)
    {
        var configFile = TryGetConfig(key);
        if (string.IsNullOrEmpty(configFile))
        {
            return "Unknown";
        }

        return configFile;

        //return key switch
        //{
        //    "thing" => "_thing_",
        //    "thing2" => "_thing2",
        //    _ => "unknown"
        //};
    }

    //private ConfigFile TryGetConfig(string key)
    private string? TryGetConfig(string key)
    {
        var configFiles = GetConfigFilePathsForClientCode(key);

        if (configFiles.Length == 0)
        {
            Console.WriteLine("No config matches");
            return null;
        }

        return configFiles[0];
    }

    private string[] GetConfigFilePathsForClientCode(string key)
    {
        var dirsToSearch = Constants.Directory.DirectoriesToSearch;
        var configFileNamesToSearch = Constants.Directory.ConfigFileNamesToSearch;

        List<string> configPaths = [];

        foreach (var searchDir in dirsToSearch)
        {
            var clientDirs = Directory.EnumerateDirectories(searchDir, key, SearchOption.TopDirectoryOnly);

            foreach (var fileNameToSearch in configFileNamesToSearch)
            {
                foreach (var clientDir in clientDirs)
                {
                    var topLevelMatchingPaths = Directory.EnumerateFileSystemEntries(clientDir, fileNameToSearch, SearchOption.TopDirectoryOnly);
                    configPaths.AddRange(topLevelMatchingPaths);

                    var childDirs = Directory.EnumerateDirectories(clientDir);

                    foreach (var childDir in childDirs)
                    {
                        var paths = Directory.EnumerateFiles(childDir, fileNameToSearch, SearchOption.AllDirectories);
                        configPaths.AddRange(paths);
                    }
                }
            }
        }

        return configPaths.ToArray();
    }
}
