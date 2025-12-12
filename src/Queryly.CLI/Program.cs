using Queryly.CLI.Commands;
using Spectre.Console;

AnsiConsole.Write(
    new FigletText("Queryly")
        .Centered()
        .Color(Color.Blue));

AnsiConsole.MarkupLine("[grey]Your local database companion[/]");
AnsiConsole.MarkupLine("[grey]Queryly is a cross platform lightweight database manager for developers[/]");
AnsiConsole.MarkupLine("[grey]and data enthusiasts. Manage your database connections, explore schemas,[/]");
AnsiConsole.MarkupLine("[grey]browse data, and execute queries with ease.[/]");
//AnsiConsole.MarkupLine("[grey]https://queryly.dev[/]");
AnsiConsole.MarkupLine("[grey]--------------------------------------------------------[/]");
AnsiConsole.WriteLine();

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

if (command == "connect" && args.Length > 1)
{
    var subCommand = args[1].ToLower();

    if (subCommand == "list") { await ConnectCommand.ListConnectionsAsync(); return; }
    else if (subCommand == "add") { await ConnectCommand.AddConnectionAsync(); return; }
    else if (subCommand == "test" && args.Length > 2) { var name = args[2]; await ConnectCommand.TestConnectionAsync(name); return; }
    else if (subCommand == "remove" && args.Length > 2) { var name = args[2]; await ConnectCommand.RemoveConnectionAsync(name); return; }
    else { AnsiConsole.MarkupLine("[red]Invalid data command.[/]"); ShowHelp(); }
}
else if (command == "schema" && args.Length > 2)
{
    var subCommand = args[1].ToLower();
    var connectionName = args[2];

    if (subCommand == "list") { await SchemaCommand.ListTablesAsync(connectionName); return; }
    else if (subCommand == "info" && args.Length > 3) { var tableName = args[3]; await SchemaCommand.ShowTableInfoAsync(connectionName, tableName); return; }
    else if (subCommand == "tree") { await SchemaCommand.ShowSchemaTreeAsync(connectionName); return; }
    else { AnsiConsole.MarkupLine("[red]Invalid data command.[/]"); ShowHelp(); }
}
else if (command == "data" && args.Length > 1)
{
    var subCommand = args[1].ToLower();
    
    if (subCommand == "browse" && args.Length > 3) { var connectionName = args[2]; var tableName = args[3];
        await DataCommand.BrowseTableAsync(connectionName, tableName); return; }
    else if (subCommand == "query" && args.Length > 2) { var connectionName = args[2];
        await DataCommand.ExecuteQueryAsync(connectionName); return; }
    else if (subCommand == "export" && args.Length > 3) { var connectionName = args[2]; var tableName = args[3];
        var format = args.Length > 4 ? args[4] : "csv"; await DataCommand.ExportTableAsync(connectionName, tableName, format); return;
    }
    else { AnsiConsole.MarkupLine("[red]Invalid data command.[/]"); ShowHelp(); }
}
else { ShowHelp(); }

static void ShowHelp()
{
    AnsiConsole.MarkupLine("[bold]Queryly Commands:[/]");
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[bold blue]Connection Management:[/]");
    AnsiConsole.MarkupLine("  connect list    - List all connections");
    AnsiConsole.MarkupLine("  connect add     - Add a new connection");
    AnsiConsole.MarkupLine("  connect test <name> - Test a connection by name");
    AnsiConsole.MarkupLine("  connect remove <name> - Remove a connection by name");

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[bold blue]Schema Exploration:[/]");
    AnsiConsole.MarkupLine("  schema list <connection-name> - List all tables in the specified connection");
    AnsiConsole.MarkupLine("  schema info <connection-name> <table-name> - Show detailed info about a table");
    AnsiConsole.MarkupLine("  schema tree <connection-name> - Display the database schema as a tree");

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[bold blue]Data Operations:[/]");
    AnsiConsole.MarkupLine("  data browse <connection-name> <table>       - Browse table data");
    AnsiConsole.MarkupLine("  data query <connection-name>                - Execute SQL query");
    AnsiConsole.MarkupLine("  data export <connection-name> <table> <format> - Export (csv/json)");
}