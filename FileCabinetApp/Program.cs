using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Models;
using FileCabinetApp.Validators;

namespace FileCabinetApp;

/// <summary>
/// The main program class.
/// </summary>
public static class Program
{
    private static readonly IReadOnlyCollection<HelpMessage> HelpMessages = Models.HelpMessages.Messages;

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

        Config.Initialize();

        var validationConfig = Config.GetValidationRules(validationRules);
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

        PrintStartupMessages(storage, validationRules, useStopwatch, useLogger);

        var commandHandler = CreateCommandHandlers(fileCabinetService);

        RunCommandLoop(commandHandler);
    }

    private static int LevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0)
        {
            return m;
        }

        if (m == 0)
        {
            return n;
        }

        for (int i = 0; i <= n; d[i, 0] = i++)
        {
        }

        for (int j = 0; j <= m; d[0, j] = j++)
        {
        }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }

    private static List<string> FindSimilarCommands(string input, IEnumerable<string> commands, int maxDistance = 3)
    {
        var similarCommands = new List<string>();
        foreach (var command in commands)
        {
            int distance = LevenshteinDistance(input.ToLowerInvariant(), command.ToLowerInvariant());
            if (distance <= maxDistance)
            {
                similarCommands.Add(command);
            }
        }

        return similarCommands.OrderBy(c => LevenshteinDistance(input.ToLowerInvariant(), c.ToLowerInvariant())).ToList();
    }

    private static ICommandHandler CreateCommandHandlers(IFileCabinetService fileCabinetService)
    {
        var helpHandler = new HelpCommandHandler(HelpMessages);
        var exitHandler = new ExitCommandHandler(isRunning => Program.isRunning = isRunning);
        var statHandler = new StatCommandHandler(fileCabinetService);
        var createHandler = new CreateCommandHandler(fileCabinetService, HelpMessages);
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
        createHandler.SetNext(exportHandler);
        exportHandler.SetNext(importHandler);
        importHandler.SetNext(purgeHandler);
        purgeHandler.SetNext(insertCommandHandler);
        insertCommandHandler.SetNext(deleteCommandHandler);
        deleteCommandHandler.SetNext(updateCommandHandler);
        updateCommandHandler.SetNext(selectCommandHandler);

        return helpHandler;
    }

    private static void PrintStartupMessages(string storage, string validationRules, bool useStopwatch, bool useLogger)
    {
        Console.WriteLine($"File Cabinet Application, developed by {AppConstants.DeveloperName}");
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

        Console.WriteLine(AppConstants.HintMessage);
        Console.WriteLine();
    }

    private static void RunCommandLoop(ICommandHandler commandHandler)
    {
        do
        {
            Console.Write("> ");
            var command = Console.ReadLine();

            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine(AppConstants.HintMessage);
                continue;
            }

            var commandName = command.Split(' ')[0].ToLowerInvariant();
            var validCommands = HelpMessages.Select(m => m.Command.ToLowerInvariant()).ToList();

            if (validCommands.Contains(commandName))
            {
                commandHandler.Handle(command);
            }
            else
            {
                var similarCommands = FindSimilarCommands(commandName, validCommands);
                Console.WriteLine($"'{commandName}' is not a valid command. See 'help' for available commands.");

                if (similarCommands.Any())
                {
                    Console.WriteLine("The most similar command" + (similarCommands.Count > 1 ? "s are" : " is"));
                    foreach (var similarCommand in similarCommands)
                    {
                        Console.WriteLine($"\t{similarCommand}");
                    }
                }
            }
        }
        while (isRunning);
    }
}