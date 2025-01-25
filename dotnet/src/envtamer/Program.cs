using envtamer.List;
using envtamer.Pull;
using envtamer.Push;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
    config.AddCommand<PushCommand>("push")
        .WithDescription("Push the contents of the env file to a secure storage.");
    config.AddCommand<PullCommand>("pull")
        .WithDescription("Pull the contents of the env file from secure storage.");
    config.AddCommand<ListCommand>("list")
        .WithDescription("List the env variables for a specified directory.");
});

AnsiConsole.Write(
    new FigletText("envtamer"));

return app.Run(args);
