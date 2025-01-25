using System.ComponentModel;
using Spectre.Console.Cli;
using envtamer.Data;
using envtamer.Utils;

namespace envtamer.Push;
public class PushCommand : Command<PushCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[DIRECTORY_NAME]")]
        [Description("Name of the directory to push. Defaults to current working directory if not specified.")]
        public string? DirectoryName { get; set; }

        [CommandOption("-p|--path <PATH>")]
        [Description("Path to the env file. Defaults to '.env' in the specified or current directory.")]
        [DefaultValue(".env")]
        public string EnvFilePath { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var directory = settings.DirectoryName ?? Directory.GetCurrentDirectory();
        var sanitizedDirectory = DirectorySanitizer.SanitizeDirectoryName(directory);
        var fullPath = Path.Combine(directory, settings.EnvFilePath);

        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"Error: .env file not found at {fullPath}");
            return 1;
        }

        var envContents = readEnvFile(fullPath);
        storeEnvContents(sanitizedDirectory, envContents);

        Console.WriteLine($"Successfully pushed .env contents for {directory}");

        return 0;
    }

    private Dictionary<string, string> readEnvFile(string filePath)
    {
        var envContents = new Dictionary<string, string>();

        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                envContents[parts[0].Trim()] = parts[1].Trim();
            }
        }

        return envContents;
    }

    private void storeEnvContents(string directory, Dictionary<string, string> envContents)
    {
        using var db = new EnvTamerContext();

        db.Database.EnsureCreated();

        var existingEntries =
            db.EnvVariables.Where(e => e.Directory == directory).ToList();
        db.EnvVariables.RemoveRange(existingEntries);

        foreach (var entry in envContents)
        {
            db.EnvVariables.Add(new EnvVariable
            {
                Directory = directory,
                Key = entry.Key,
                Value = entry.Value
            });
        }

        db.SaveChanges();
    }
}
