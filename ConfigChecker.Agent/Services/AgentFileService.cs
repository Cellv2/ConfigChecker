using ConfigChecker.Agent.Models;
using System.Text;

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

        var fileData = File.ReadAllText(configFile);

        //return configFile;
        return fileData;

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

        var clientDirs = GetDirectoriesByClientCode(key);

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

    private string[] GetDirectoriesByClientCode(string key)
    {
        var dirsToSearch = Constants.Directory.DirectoriesToSearch;
        List<string> targetDirectories = [];

        foreach (var searchDir in dirsToSearch)
        {
            var dir = Directory.EnumerateDirectories(searchDir, key, SearchOption.TopDirectoryOnly);
            targetDirectories.AddRange(dir);
        }

        return targetDirectories.ToArray();
    }
}
