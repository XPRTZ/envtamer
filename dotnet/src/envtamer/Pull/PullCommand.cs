using System.ComponentModel;
using envtamer.Data;
using envtamer.Utils;
using Spectre.Console.Cli;

namespace envtamer.Pull;

public class PullCommand : Command<PullCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[DIRECTORY_NAME]")]
        [Description("Name of the directory to pull the env file from.")]
        public string DirectoryName { get; set; }

        [CommandOption("-p|--path <PATH>")]
        [Description("Path to save the env file. Defaults to '.env' in the specified or current directory.")]
        [DefaultValue(".env")]
        public string EnvFileName { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var directory = settings.DirectoryName;
        var sanitizedDirectory = DirectorySanitizer.SanitizeDirectoryName(directory);
        var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), settings.EnvFileName);

        if (File.Exists(envFilePath))
        {
            Console.WriteLine($"⚠️ An env file already exists at {envFilePath}");
            Console.Write("Overwriting will delete the current file. Continue? (Y/N): ");
            var response = Console.ReadKey();
            Console.WriteLine();

            if (response.Key != ConsoleKey.Y)
            {
                Console.WriteLine("🛑 Operation cancelled.");
                return 0;
            }
        }

        using var db = new EnvTamerContext();

        var envVariables = db.EnvVariables
            .Where(e => e.Directory == sanitizedDirectory)
            .OrderBy(e => e.Key)
            .ToList();

        if (envVariables.Count == 0)
        {
            Console.WriteLine($"ℹ️ No environment variables found for directory: {directory}");
            return 0;
        }

        try
        {
            using (var writer = new StreamWriter(envFilePath, false))
            {
                foreach (var variable in envVariables)
                {
                    writer.WriteLine($"{variable.Key}={variable.Value}");
                }
            }

            Console.WriteLine($"✅ Environment variables successfully pulled to {envFilePath}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🛑 Error writing to env file: {ex.Message}");
            return 1;
        }
    }
}
