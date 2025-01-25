using System.ComponentModel;
using envtamer.Data;
using envtamer.Utils;
using Spectre.Console.Cli;

namespace envtamer.List;

public class ListCommand : Command<ListCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[DIRECTORY_NAME]")]
        [Description("Name of the directory to list env variables for. If not specified, lists all stored directories.")]
        public string? DirectoryName { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        using var db = new EnvTamerContext();

        db.Database.EnsureCreated();

        if (string.IsNullOrEmpty(settings.DirectoryName))
        {
            ListAllDirectories(db);
        }
        else
        {
            ListDirectoryContents(db, settings.DirectoryName);
        }

        return 0;
    }

    private void ListAllDirectories(EnvTamerContext db)
    {
        var directories = db.EnvVariables
            .Select(e => e.Directory)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (directories.Count == 0)
        {
            Console.WriteLine("No directories found in the database.");
            return;
        }

        Console.WriteLine("Stored directories:");
        foreach (var directory in directories)
        {
            Console.WriteLine(directory);
        }
    }

    private void ListDirectoryContents(EnvTamerContext db, string directoryName)
    {
        var sanitizedDirectory = DirectorySanitizer.SanitizeDirectoryName(directoryName);

        var envVariables = db.EnvVariables
            .Where(e => e.Directory == sanitizedDirectory)
            .OrderBy(e => e.Key)
            .ToList();

        if (envVariables.Count == 0)
        {
            Console.WriteLine($"No environment variables found for directory: {directoryName}");
            return;
        }

        Console.WriteLine($"Environment variables for directory: {directoryName}");
        Console.WriteLine("----------------------------------------");

        foreach (var variable in envVariables)
        {
            Console.WriteLine($"{variable.Key} = {variable.Value}");
        }
    }
}
