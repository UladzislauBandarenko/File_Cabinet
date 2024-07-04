﻿using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp;

public static class Program
{
    private const string DeveloperName = "Uladzislau Bandarenko";
    private const string HintMessage = "Enter your command, or enter 'help' to get help.";
    private const int CommandHelpIndex = 0;
    private const int DescriptionHelpIndex = 1;
    private const int ExplanationHelpIndex = 2;
    private static IFileCabinetService fileCabinetService;

    private static bool isRunning = true;

    private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
    {
        new Tuple<string, Action<string>>("help", PrintHelp),
        new Tuple<string, Action<string>>("exit", Exit),
        new Tuple<string, Action<string>>("stat", Stat),
        new Tuple<string, Action<string>>("create", Create),
        new Tuple<string, Action<string>>("list", List),
        new Tuple<string, Action<string>>("edit", Edit),
        new Tuple<string, Action<string>>("find", Find),
        new Tuple<string, Action<string>>("export", Export),
        new Tuple<string, Action<string>>("import", Import),
        new Tuple<string, Action<string>>("remove", Remove),
        new Tuple<string, Action<string>>("purge", Purge),
    };

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

        IRecordValidator validator = validationRules == "default"
            ? new DefaultValidator()
            : new CustomValidator();

        if (storage == "file")
        {
            string filePath = "cabinet-records.db";
            FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileCabinetService = new FileCabinetFilesystemService(validator, fileStream);
        }
        else
        {
            fileCabinetService = new FileCabinetMemoryService(validator);
        }

        Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
        Console.WriteLine($"Using {storage} storage.");
        Console.WriteLine($"Using {validationRules} validation rules.");
        Console.WriteLine(Program.HintMessage);
        Console.WriteLine();

        do
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            var inputs = line != null ? line.Split(' ', 2) : new string[] { string.Empty, string.Empty };
            const int commandIndex = 0;
            var command = inputs[commandIndex];

            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine(Program.HintMessage);
                continue;
            }

            var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                commands[index].Item2(parameters);
            }
            else
            {
                PrintMissedCommandInfo(command);
            }
        }
        while (isRunning);
    }

    private static void PrintMissedCommandInfo(string command)
    {
        Console.WriteLine($"There is no '{command}' command.");
        Console.WriteLine();
    }

    private static void PrintHelp(string parameters)
    {
        if (!string.IsNullOrEmpty(parameters))
        {
            var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
            }
            else
            {
                Console.WriteLine($"There is no explanation for '{parameters}' command.");
            }
        }
        else
        {
            Console.WriteLine("Available commands:");

            foreach (var helpMessage in helpMessages)
            {
                Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
            }
        }

        Console.WriteLine();
    }

    private static void Exit(string parameters)
    {
        Console.WriteLine("Exiting an application...");
        isRunning = false;
    }

    private static void Stat(string parameters)
    {
        var recordsCount = Program.fileCabinetService.GetStat();
        Console.WriteLine($"{recordsCount} record(s).");
    }

    private static void Create(string parameters)
    {
        var firstName = ReadInput<string>("First name", ValidateFirstName, s => s);
        var lastName = ReadInput<string>("Last name", ValidateLastName, s => s);
        var dateOfBirth = ReadInput<DateTime>("Date of birth (mm/dd/yyyy)", ValidateDateOfBirth, DateTime.Parse);
        var age = ReadInput<short>("Age", ValidateAge, short.Parse);
        var salary = ReadInput<decimal>("Salary", ValidateSalary, decimal.Parse);
        var gender = ReadInput<char>("Gender (M/F)", ValidateGender, s => s[0]);

        try
        {
            var personalInfo = new PersonalInfo(firstName, lastName, dateOfBirth, age, salary, gender);
            var recordId = Program.fileCabinetService.CreateRecord(personalInfo);
            Console.WriteLine($"Record #{recordId} is created.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Failed to create record: {ex.Message}");
        }
    }

    private static void Edit(string parameters)
        {
        if (!int.TryParse(parameters, out int id))
        {
            Console.WriteLine($"Invalid record id: {parameters}");
            return;
        }

        try
        {
            var firstName = ReadInput<string>("First name", ValidateFirstName, s => s);
            var lastName = ReadInput<string>("Last name", ValidateLastName, s => s);
            var dateOfBirth = ReadInput<DateTime>("Date of birth (mm/dd/yyyy)", ValidateDateOfBirth, DateTime.Parse);
            var age = ReadInput<short>("Age", ValidateAge, short.Parse);
            var salary = ReadInput<decimal>("Salary", ValidateSalary, decimal.Parse);
            var gender = ReadInput<char>("Gender (M/F)", ValidateGender, s => s[0]);

            var personalInfo = new PersonalInfo(firstName, lastName, dateOfBirth, age, salary, gender);
            Program.fileCabinetService.EditRecord(id, personalInfo);
            Console.WriteLine($"Record #{id} is updated.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Failed to edit record: {ex.Message}");
        }
    }

    private static void List(string parameters)
    {
        var records = Program.fileCabinetService.GetRecords();

        foreach (var record in records)
        {
            Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MMM-dd}, {record.Age}, {record.Salary:C2}, {record.Gender}");
        }
    }

    private static void Find(string parameters)
    {
        var inputs = parameters.Split(' ', 2);
        if (inputs.Length < 2)
        {
            Console.WriteLine("Invalid command format. Usage: find <property> <value>");
            return;
        }

        var property = inputs[0].ToLowerInvariant();
        var value = inputs[1].Trim('"');

        ReadOnlyCollection<FileCabinetRecord> records;

        switch (property)
        {
            case "firstname":
                records = Program.fileCabinetService.FindByFirstName(value);
                break;
            case "lastname":
                records = Program.fileCabinetService.FindByLastName(value);
                break;
            case "dateofbirth":
                records = Program.fileCabinetService.FindByDateOfBirth(value);
                break;
            default:
                Console.WriteLine($"Searching by {property} is not supported.");
                return;
        }

        if (records.Count > 0)
        {
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MMM-dd}, {record.Age}, {record.Salary:C2}, {record.Gender}");
            }
        }
        else
        {
            Console.WriteLine("No records found.");
        }
    }

    private static void Export(string parameters)
    {
        var inputs = parameters.Split(' ', 2);
        if (inputs.Length < 2)
        {
            Console.WriteLine("Invalid command format. Usage: export [csv|xml] <filename>");
            return;
        }

        var format = inputs[0].ToLowerInvariant();
        var fileName = inputs[1];

        if (string.IsNullOrEmpty(fileName))
        {
            Console.WriteLine("File name is missing.");
            return;
        }

        try
        {
            var snapshot = fileCabinetService.MakeSnapshot();
            if (File.Exists(fileName))
            {
                Console.Write($"File is exist - rewrite {fileName}? [Y/n] ");
                var answer = Console.ReadLine();
                if (string.IsNullOrEmpty(answer) || answer.ToUpperInvariant() != "Y")
                {
                    Console.WriteLine("Export canceled.");
                    return;
                }
            }

            using (var writer = new StreamWriter(fileName))
            {
                switch (format)
                {
                    case "csv":
                        var csvWriter = new FileCabinetRecordCsvWriter(writer);
                        csvWriter.Write(snapshot.Records);
                        break;
                    case "xml":
                        var xmlWriter = new FileCabinetRecordXmlWriter(writer);
                        xmlWriter.Write(snapshot.Records);
                        break;
                    default:
                        Console.WriteLine($"Unsupported export format: {format}");
                        return;
                }
            }

            Console.WriteLine($"All records are exported to file {fileName}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Export failed: {ex.Message}");
        }
    }

    private static void Import(string parameters)
    {
        var inputs = parameters.Split(' ', 2);
        if (inputs.Length < 2)
        {
            Console.WriteLine("Invalid command format. Usage: import [csv|xml] <filename>");
            return;
        }

        var format = inputs[0].ToLowerInvariant();
        var fileName = inputs[1];

        if (string.IsNullOrEmpty(fileName))
        {
            Console.WriteLine("File name is missing.");
            return;
        }

        if (!File.Exists(fileName))
        {
            Console.WriteLine($"File {fileName} does not exist.");
            return;
        }

        try
        {
            FileCabinetServiceSnapshot snapshot;
            switch (format)
            {
                case "csv":
                    using (var reader = new StreamReader(fileName))
                    {
                        var csvReader = new FileCabinetRecordCsvReader(reader);
                        snapshot = csvReader.ReadAll();
                    }

                    break;
                case "xml":
                    using (var reader = new StreamReader(fileName))
                    {
                        var xmlReader = new FileCabinetRecordXmlReader(reader);
                        snapshot = xmlReader.ReadAll();
                    }

                    break;
                default:
                    Console.WriteLine($"Unsupported import format: {format}");
                    return;
            }

            fileCabinetService.Restore(snapshot);
            Console.WriteLine($"All records from {fileName} were imported.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while importing: {ex.Message}");
        }
    }

    private static void Remove(string parameters)
    {
        if (!int.TryParse(parameters, out int id))
        {
            Console.WriteLine($"Invalid record id: {parameters}");
            return;
        }

        try
        {
            if (fileCabinetService.RemoveRecord(id))
            {
                Console.WriteLine($"Record #{id} is removed.");
            }
            else
            {
                Console.WriteLine($"Record #{id} doesn't exist.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while removing the record: {ex.Message}");
        }
    }

    private static void Purge(string parameters)
    {
        if (fileCabinetService is FileCabinetFilesystemService filesystemService)
        {
            int totalRecords = filesystemService.GetStat();
            int purgedRecords = filesystemService.PurgeRecords();
            Console.WriteLine($"Data file processing is completed: {purgedRecords} of {totalRecords} records were purged.");
        }
        else
        {
            Console.WriteLine("Purge command is available only for file storage.");
        }
    }

    private static T ReadInput<T>(string prompt, Func<string, Tuple<bool, string>> validator, Func<string, T> converter)
    {
        string input;
        bool isValid;
        string errorMessage;

        do
        {
            Console.Write($"{prompt}: ");
            input = Console.ReadLine();
            var validationResult = validator(input);
            isValid = validationResult.Item1;
            errorMessage = validationResult.Item2;

            if (!isValid)
            {
                Console.WriteLine($"Invalid input: {errorMessage}");
            }
        }
        while (!isValid);

        return converter(input);
    }

    private static Tuple<bool, string> ValidateFirstName(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 2 || input.Length > 60)
        {
            return new Tuple<bool, string>(false, "First name must be between 2 and 60 characters and not empty.");
        }

        return new Tuple<bool, string>(true, string.Empty);
    }

    private static Tuple<bool, string> ValidateLastName(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 2 || input.Length > 60)
        {
            return new Tuple<bool, string>(false, "Last name must be between 2 and 60 characters and not empty.");
        }

        return new Tuple<bool, string>(true, string.Empty);
    }

    private static Tuple<bool, string> ValidateDateOfBirth(string input)
    {
        if (DateTime.TryParse(input, out var date) && date >= new DateTime(1950, 1, 1) && date <= DateTime.Now)
        {
            return new Tuple<bool, string>(true, string.Empty);
        }

        return new Tuple<bool, string>(false, "Date of birth must be between 01/01/1950 and the current date.");
    }

    private static Tuple<bool, string> ValidateAge(string input)
    {
        if (short.TryParse(input, out var age) && age >= 0 && age <= 120)
        {
            return new Tuple<bool, string>(true, string.Empty);
        }

        return new Tuple<bool, string>(false, "Age must be between 0 and 120.");
    }

    private static Tuple<bool, string> ValidateSalary(string input)
    {
        if (decimal.TryParse(input, out var salary) && salary >= 0 && salary <= 1000000)
        {
            return new Tuple<bool, string>(true, string.Empty);
        }

        return new Tuple<bool, string>(false, "Salary must be between $0 and $1,000,000.");
    }

    private static Tuple<bool, string> ValidateGender(string input)
    {
        if (input.Length == 1 && (input[0] == 'M' || input[0] == 'F'))
        {
            return new Tuple<bool, string>(true, string.Empty);
        }

        return new Tuple<bool, string>(false, "Gender must be either 'M' or 'F'.");
    }
}