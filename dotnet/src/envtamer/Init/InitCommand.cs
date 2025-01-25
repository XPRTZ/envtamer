using envtamer.Data;
using Spectre.Console.Cli;

namespace envtamer.Init;

public class InitCommand : Command<InitCommand.Settings>
{
    public class Settings : CommandSettings { }

    public override int Execute(CommandContext context, Settings settings)
    {
        var homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var envTamerFolder = Path.Combine(homeFolder, ".envtamer");
        var dbPath = Path.Combine(envTamerFolder, "envtamer.db");

        if (File.Exists(dbPath))
        {
            Console.WriteLine("🛑 Database file already exists. Initialization skipped.");
            return 0;
        }

        try
        {
            Directory.CreateDirectory(envTamerFolder);
            using (File.Create(dbPath)) { }
            Console.WriteLine("🗄️ Empty database file created successfully.");
            Console.WriteLine("⏳ Running migrations...");
            using var db = new EnvTamerContext();

            db.Database.EnsureCreated();
            Console.WriteLine("✅ Migrations applied successfully.");
            Console.WriteLine("🚀 Ready to push and pull env files.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🛑 Error creating database: {ex.Message}");
            return 1;
        }
    }
}
