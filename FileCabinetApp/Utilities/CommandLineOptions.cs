using System.CommandLine;

namespace FileCabinetApp.Utilities
{
    /// <summary>
    /// Command line options for the File Cabinet Application.
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// Gets or sets the validation rules to use.
        /// </summary>
        /// <value>The validation rules to use.</value>
        public string ValidationRules { get; set; } = "default";

        /// <summary>
        /// Gets or sets the storage type to use.
        /// </summary>
        /// <value>The storage type to use.</value>
        public string Storage { get; set; } = "memory";

        /// <summary>
        /// Gets or sets a value indicating whether to use the stopwatch.
        /// </summary>
        /// <value>The stopwatch.</value>
        public bool UseStopwatch { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to use the logger.
        /// </summary>
        /// <value>The logger.</value>
        public bool UseLogger { get; set; } = false;

        /// <summary>
        /// Parses the command line arguments.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The command line options.</returns>
        public static CommandLineOptions Parse(string[] args)
        {
            var options = new CommandLineOptions();

            var rootCommand = new RootCommand("File Cabinet Application");

            var validationRulesOption = new Option<string>(
                new[] { "--validation-rules", "-v" },
                () => "default",
                "Validation rules to use (default or custom)");
            rootCommand.AddOption(validationRulesOption);

            var storageOption = new Option<string>(
                new[] { "--storage", "-s" },
                () => "memory",
                "Storage type to use (memory or file)");
            rootCommand.AddOption(storageOption);

            var useStopwatchOption = new Option<bool>(
                "--use-stopwatch",
                "Enable execution time measurement");
            rootCommand.AddOption(useStopwatchOption);

            var useLoggerOption = new Option<bool>(
                "--use-logger",
                "Enable service logging");
            rootCommand.AddOption(useLoggerOption);

            rootCommand.SetHandler(
                (validationRules, storage, useStopwatch, useLogger) =>
            {
                options.ValidationRules = validationRules.ToLowerInvariant();
                options.Storage = storage.ToLowerInvariant();
                options.UseStopwatch = useStopwatch;
                options.UseLogger = useLogger;

                if (options.ValidationRules != "default" && options.ValidationRules != "custom")
                {
                    Console.WriteLine("Invalid validation rules specified. Using default rules.");
                    options.ValidationRules = "default";
                }

                if (options.Storage != "memory" && options.Storage != "file")
                {
                    Console.WriteLine("Invalid storage type specified. Using memory storage.");
                    options.Storage = "memory";
                }
            },
                validationRulesOption,
                storageOption,
                useStopwatchOption,
                useLoggerOption);

            rootCommand.Invoke(args);

            return options;
        }
    }
}