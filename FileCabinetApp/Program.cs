using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Validators;

namespace FileCabinetApp;

/// <summary>
/// The main program class.
/// </summary>
public static class Program
{
    private const string DeveloperName = "Uladzislau Bandarenko";
    private const string HintMessage = "Enter your command, or enter 'help' to get help.";
    private static IFileCabinetService? fileCabinetService;

    private static bool isRunning = true;

    private static string[][] helpMessages = new string[][]
    {
        new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
        new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
        new string[] { "stat", "prints the number of records", "The 'stat' command prints the number of records." },
        new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
        new string[] { "list", "lists all records", "The 'list' command lists all records." },
        new string[] { "edit", "edits an existing record", "The 'edit' command edits an existing record." },
        new string[] { "find", "finds records by property", "The 'find' command finds records by property. Usage: find <property> <value>" },
        new string[] { "export", "exports records to a file", "The 'export' command exports all records to a file. Usage: export [csv|xml] <filename>" },
        new string[] { "import", "imports records from a file", "The 'import' command imports records from a file. Usage: import [csv|xml] <filename>" },
        new string[] { "remove", "removes a record", "The 'remove' command removes a record. Usage: remove <id>" },
        new string[] { "purge", "purges all records", "The 'purge' command purges all records." },
    };

    /// <summary>
    /// The main entry point of the application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    public static void Main(string[] args)
    {
        string validationRules = "default";
        string storage = "memory";

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--validation-rules" || args[i] == "-v")
            {
                if (i + 1 < args.Length)
                {
                    validationRules = args[i + 1].ToLowerInvariant();
                    if (validationRules != "default" && validationRules != "custom")
                    {
                        Console.WriteLine("Invalid validation rules specified. Using default rules.");
                        validationRules = "default";
                    }

                    i++;
                }
            }
            else if (args[i] == "--storage" || args[i] == "-s")
            {
                if (i + 1 < args.Length)
                {
                    storage = args[i + 1].ToLowerInvariant();
                    if (storage != "memory" && storage != "file")
                    {
                        Console.WriteLine("Invalid storage type specified. Using memory storage.");
                        storage = "memory";
                    }

                    i++;
                }
            }
        }

        ValidatorBuilder validatorBuilder = new ValidatorBuilder();
        if (validationRules == "default")
        {
            validatorBuilder.AddDefaultValidators();
        }
        else
        {
            validatorBuilder.AddCustomValidators();
        }

        if (storage == "file")
        {
            string filePath = "cabinet-records.db";
            FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileCabinetService = new FileCabinetFilesystemService(validatorBuilder, fileStream);
        }
        else
        {
            fileCabinetService = new FileCabinetMemoryService(validatorBuilder);
        }

        Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
        Console.WriteLine($"Using {storage} storage.");
        Console.WriteLine($"Using {validationRules} validation rules.");
        Console.WriteLine(Program.HintMessage);
        Console.WriteLine();

        var commandHandler = CreateCommandHandlers();

        do
        {
            Console.Write("> ");
            var command = Console.ReadLine();

            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine(Program.HintMessage);
                continue;
            }

            commandHandler.Handle(command);
        }
        while (isRunning);
    }

    private static ICommandHandler CreateCommandHandlers()
    {
        var helpHandler = new HelpCommandHandler(helpMessages);
        var exitHandler = new ExitCommandHandler(isRunning => Program.isRunning = isRunning);
        var statHandler = new StatCommandHandler(fileCabinetService);
        var createHandler = new CreateCommandHandler(fileCabinetService);
        var listHandler = new ListCommandHandler(fileCabinetService);
        var editHandler = new EditCommandHandler(fileCabinetService);
        var findHandler = new FindCommandHandler(fileCabinetService);
        var exportHandler = new ExportCommandHandler(fileCabinetService);
        var importHandler = new ImportCommandHandler(fileCabinetService);
        var removeHandler = new RemoveCommandHandler(fileCabinetService);
        var purgeHandler = new PurgeCommandHandler(fileCabinetService);

        helpHandler.SetNext(exitHandler);
        exitHandler.SetNext(statHandler);
        statHandler.SetNext(createHandler);
        createHandler.SetNext(listHandler);
        listHandler.SetNext(editHandler);
        editHandler.SetNext(findHandler);
        findHandler.SetNext(exportHandler);
        exportHandler.SetNext(importHandler);
        importHandler.SetNext(removeHandler);
        removeHandler.SetNext(purgeHandler);

        return helpHandler;
    }
}