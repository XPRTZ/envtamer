using System.ComponentModel;
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
        public string EnvFilePath { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var directory = settings.DirectoryName ?? Directory.GetCurrentDirectory();
        var fullPath = Path.Combine(directory, settings.EnvFilePath);

        Console.WriteLine($"Pulling env file to {fullPath}");
        // Implement pull logic here
        return 0;
    }
}
