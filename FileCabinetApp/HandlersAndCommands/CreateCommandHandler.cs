using System.Globalization;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Create command handler.
    /// </summary>
    public class CreateCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly IReadOnlyCollection<HelpMessage> helpMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <param name="helpMessages">The help messages.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService, IReadOnlyCollection<HelpMessage> helpMessages)
        {
            this.fileCabinetService = fileCabinetService;
            this.helpMessages = helpMessages;
        }

        /// <inheritdoc/>
        public override void Handle(string command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.Equals("create", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    Console.Write("First name: ");
                    var firstName = Console.ReadLine() ?? string.Empty;

                    Console.Write("Last name: ");
                    var lastName = Console.ReadLine() ?? string.Empty;

                    Console.Write("Date of birth (yyyy-mm-dd): ");
                    var dateOfBirth = DateTime.Parse(Console.ReadLine() ?? string.Empty, CultureInfo.InvariantCulture);

                    Console.Write("Age: ");
                    var age = short.Parse(Console.ReadLine() ?? string.Empty, CultureInfo.InvariantCulture);

                    Console.Write("Salary: ");
                    var salary = decimal.Parse(Console.ReadLine() ?? string.Empty, CultureInfo.InvariantCulture);

                    Console.Write("Gender (M/F): ");
                    var gender = char.Parse(Console.ReadLine() ?? string.Empty);

                    var personalInfo = new PersonalInfo(firstName, lastName, dateOfBirth, age, salary, gender);
                    var recordId = this.fileCabinetService.CreateRecord(personalInfo);
                    Console.WriteLine($"Record #{recordId} is created.");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Failed to create record: {ex.Message}");
                }
                catch (FormatException)
                {
                    var createHelp = this.helpMessages.FirstOrDefault(m => m.Command == "create");
                    Console.WriteLine("Invalid input format.");
                    if (createHelp != null)
                    {
                        Console.WriteLine(createHelp.DetailedDescription);
                    }
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}