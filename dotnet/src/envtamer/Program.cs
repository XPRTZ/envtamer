using envtamer.Init;
using envtamer.List;
using envtamer.Pull;
using envtamer.Push;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.AddCommand<InitCommand>("init")
        .WithDescription("Initialize the secure storage.");
    config.AddCommand<PushCommand>("push")
        .WithDescription("Push the contents of the env file to the secure storage.");
    config.AddCommand<PullCommand>("pull")
        .WithDescription("Pull the contents of the env file from the secure storage.");
    config.AddCommand<ListCommand>("list")
        .WithDescription("List the env variables from the secure storage for a specified directory.");
});

AnsiConsole.Write(
    new FigletText("envtamer"));

return app.Run(args);
