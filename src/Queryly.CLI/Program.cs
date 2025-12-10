using Spectre.Console;

AnsiConsole.Write(
    new FigletText("Queryly")
        .Centered()
        .Color(Color.Blue));

AnsiConsole.MarkupLine("[grey]Your local database companion[/]");

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

if (command == "connect" && args.Length > 1)
{
    var subCommand = args[1].ToLower();
    
    if (subCommand == "list")
    {
        await ConnectCommand.ListConnectionsAsync();
        return;
    } else if (subCommand == "add")
    {
        await ConnectCommand.AddConnectionAsync();
        return;
    } else if (subCommand == "test" && args.Length > 2)
    {
        var name = args[2];
        await ConnectCommand.TestConnectionAsync(name);
        return;
    } else if (subCommand == "remove" && args.Length > 2)
    {
        var name = args[2];
        await ConnectCommand.RemoveConnectionAsync(name);
        return;
    }
}

static void ShowHelp()
{
    AnsiConsole.MarkupLine("[bold]Queryly Commands:[/]");
    AnsiConsole.MarkupLine("  connect list    - List all connections");
    AnsiConsole.MarkupLine("  connect add     - Add a new connection");
    AnsiConsole.MarkupLine("  connect test <name> - Test a connection by name");
    AnsiConsole.MarkupLine("  connect remove <name> - Remove a connection by name");
    // More commands later
}