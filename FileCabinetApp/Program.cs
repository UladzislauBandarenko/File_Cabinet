﻿using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Models;
using FileCabinetApp.Validators;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp;

/// <summary>
/// The main program class.
/// </summary>
public static class Program
{
    private const string DeveloperName = "Uladzislau Bandarenko";
    private const string HintMessage = "Enter your command, or enter 'help' to get help.";

    private static readonly IReadOnlyCollection<HelpMessage> HelpMessages = new[]
    {
        new HelpMessage("help", "prints the help screen", "The 'help' command prints the help screen."),
        new HelpMessage("exit", "exits the application", "The 'exit' command exits the application."),
        new HelpMessage("stat", "prints the number of records", "The 'stat' command prints the number of records."),
        new HelpMessage("create", "creates a new record", "The 'create' command creates a new record."),
        new HelpMessage("list", "lists all records", "The 'list' command lists all records."),
        new HelpMessage("find", "finds records by property", "The 'find' command finds records by property. Usage: find <property> <value>"),
        new HelpMessage("export", "exports records to a file", "The 'export' command exports all records to a file. Usage: export [csv|xml] <filename>"),
        new HelpMessage("import", "imports records from a file", "The 'import' command imports records from a file. Usage: import [csv|xml] <filename>"),
        new HelpMessage("purge", "purges all records", "The 'purge' command purges all records."),
        new HelpMessage("insert", "inserts a new record", "The 'insert' command inserts a new record. Usage: insert (id, firstname, lastname, dateofbirth, height, weight, gender) values (<id>, <firstname>, <lastname>, <dateofbirth>, <height>, <weight>, <gender>)"),
        new HelpMessage("delete", "deletes records", "The 'delete' command deletes records. Usage: delete where <field>=<value>"),
        new HelpMessage("update", "updates records", "The 'update' command updates records. Usage: update set <field1>=<value1>, <field2>=<value2>, ... where <field3>=<value3>, <field4>=<value4>, ..."),
        new HelpMessage("select", "selects records", "The 'select' command selects records based on specified fields and conditions. Usage: select <field1>, <field2>, ... [where <condition1> and/or <condition2> ...]"),
    };

    private static IFileCabinetService? fileCabinetService;

    private static bool isRunning = true;

    /// <summary>
    /// The main entry point of the application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    public static void Main(string[] args)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        string validationRules = "default";
        string storage = "memory";
        bool useStopwatch = false;
        bool useLogger = false;

        int i = 0;
        while (i < args.Length)
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

                    i += 2;
                    continue;
                }
            }
            else if (args[i] == "--storage" || (args[i] == "-s" && i + 1 < args.Length))
            {
                storage = args[i + 1].ToLowerInvariant();
                if (storage != "memory" && storage != "file")
                {
                    Console.WriteLine("Invalid storage type specified. Using memory storage.");
                    storage = "memory";
                }

                i += 2;
                continue;
            }
            else if (args[i] == "--use-stopwatch")
            {
                useStopwatch = true;
                i++;
                continue;
            }
            else if (args[i] == "--use-logger")
            {
                useLogger = true;
                i++;
                continue;
            }

            i++;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("validation-rules.json", optional: false, reloadOnChange: true)
            .Build();

        var validationConfig = configuration.GetSection(validationRules).Get<ValidationConfig>() ?? new ValidationConfig();
        ValidatorBuilder validatorBuilder = new ValidatorBuilder();
        validatorBuilder.AddValidators(validationConfig);

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

        if (useStopwatch)
        {
            fileCabinetService = new ServiceMeter(fileCabinetService);
        }

        if (useLogger)
        {
            string logFilePath = "filecabinet-log.txt";
            fileCabinetService = new ServiceLogger(fileCabinetService, logFilePath);
        }

        Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
        Console.WriteLine($"Using {storage} storage.");
        Console.WriteLine($"Using {validationRules} validation rules.");
        if (useStopwatch)
        {
            Console.WriteLine("Execution time measurement is enabled.");
        }

        if (useLogger)
        {
            Console.WriteLine("Service logging is enabled.");
        }

        Console.WriteLine(Program.HintMessage);
        Console.WriteLine();

        var commandHandler = CreateCommandHandlers(fileCabinetService);

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

    private static ICommandHandler CreateCommandHandlers(IFileCabinetService fileCabinetService)
    {
        var helpHandler = new HelpCommandHandler(HelpMessages);
        var exitHandler = new ExitCommandHandler(isRunning => Program.isRunning = isRunning);
        var statHandler = new StatCommandHandler(fileCabinetService);
        var createHandler = new CreateCommandHandler(fileCabinetService, HelpMessages);
        var listHandler = new ListCommandHandler(fileCabinetService);
        var findHandler = new FindCommandHandler(fileCabinetService, HelpMessages);
        var exportHandler = new ExportCommandHandler(fileCabinetService, HelpMessages);
        var importHandler = new ImportCommandHandler(fileCabinetService, HelpMessages);
        var purgeHandler = new PurgeCommandHandler(fileCabinetService);
        var insertCommandHandler = new InsertCommandHandler(fileCabinetService, HelpMessages);
        var deleteCommandHandler = new DeleteCommandHandler(fileCabinetService, HelpMessages);
        var updateCommandHandler = new UpdateCommandHandler(fileCabinetService, HelpMessages);
        var selectCommandHandler = new SelectCommandHandler(fileCabinetService, HelpMessages);

        helpHandler.SetNext(exitHandler);
        exitHandler.SetNext(statHandler);
        statHandler.SetNext(createHandler);
        createHandler.SetNext(listHandler);
        listHandler.SetNext(findHandler);
        findHandler.SetNext(exportHandler);
        exportHandler.SetNext(importHandler);
        importHandler.SetNext(purgeHandler);
        purgeHandler.SetNext(insertCommandHandler);
        insertCommandHandler.SetNext(deleteCommandHandler);
        deleteCommandHandler.SetNext(updateCommandHandler);
        updateCommandHandler.SetNext(selectCommandHandler);

        return helpHandler;
    }
}