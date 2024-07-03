using System.Globalization;

namespace FileCabinetApp;

public static class Program
{
    private const string DeveloperName = "Uladzislau Bandarenko";
    private const string HintMessage = "Enter your command, or enter 'help' to get help.";
    private const int CommandHelpIndex = 0;
    private const int DescriptionHelpIndex = 1;
    private const int ExplanationHelpIndex = 2;
    private static FileCabinetService fileCabinetService;

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
        new string[] { "--validation-rules", "sets the validation rules", "The '--validation-rules' or '-v' parameter sets the validation rules. Usage: --validation-rules <default|custom>" },
    };

    public static void Main(string[] args)
    {
        string validationRules = "default";
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--validation-rules" || args[i] == "-v")
            {
                if (i + 1 < args.Length)
                {
                    validationRules = args[i + 1].ToLower();
                    if (validationRules != "default" && validationRules != "custom")
                    {
                        Console.WriteLine("Invalid validation rules specified. Using default rules.");
                        validationRules = "default";
                    }
                    break;
                }
            }
        }

        fileCabinetService = validationRules == "default"
            ? new FileCabinetDefaultService()
            : new FileCabinetCustomService();

        Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
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

            var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
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
        string firstName = GetValidInput<string>("First name", s => !string.IsNullOrWhiteSpace(s) && s.Length >= 2 && s.Length <= 60);
        string lastName = GetValidInput<string>("Last name", s => !string.IsNullOrWhiteSpace(s) && s.Length >= 2 && s.Length <= 60);
        DateTime dateOfBirth = GetValidInput<DateTime>("Date of birth (mm/dd/yyyy)", s => DateTime.TryParse(s, out var date) && date >= new DateTime(1950, 1, 1) && date <= DateTime.Now);
        short age = GetValidInput<short>("Age", s => short.TryParse(s, out var a) && a >= 0 && a <= 120);
        decimal salary = GetValidInput<decimal>("Salary", s => decimal.TryParse(s, out var sal) && sal >= 0 && sal <= 1000000);
        char gender = GetValidInput<char>("Gender (M/F)", s => s.Length == 1 && (s[0] == 'M' || s[0] == 'F'));
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
            string firstName = GetValidInput<string>("First name", s => !string.IsNullOrWhiteSpace(s) && s.Length >= 2 && s.Length <= 60);
            string lastName = GetValidInput<string>("Last name", s => !string.IsNullOrWhiteSpace(s) && s.Length >= 2 && s.Length <= 60);
            DateTime dateOfBirth = GetValidInput<DateTime>("Date of birth (mm/dd/yyyy)", s => DateTime.TryParse(s, out var date) && date >= new DateTime(1950, 1, 1) && date <= DateTime.Now);
            short age = GetValidInput<short>("Age", s => short.TryParse(s, out var a) && a >= 0 && a <= 120);
            decimal salary = GetValidInput<decimal>("Salary", s => decimal.TryParse(s, out var sal) && sal >= 0 && sal <= 1000000);
            char gender = GetValidInput<char>("Gender (M/F)", s => s.Length == 1 && (s[0] == 'M' || s[0] == 'F'));

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

        var property = inputs[0].ToLower();
        var value = inputs[1].Trim('"');

        FileCabinetRecord[] records = null;

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

        if (records != null && records.Length > 0)
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

    private static T GetValidInput<T>(string prompt, Func<string, bool> validator)
    {
        string input;
        do
        {
            Console.Write($"{prompt}: ");
            input = Console.ReadLine();
        } while (!validator(input));

        return (T)Convert.ChangeType(input, typeof(T));
    }
}