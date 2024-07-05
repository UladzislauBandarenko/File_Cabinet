using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Create command handler.
    /// </summary>
    public class CreateCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
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
                    Console.WriteLine("Invalid input format. Please try again.");
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}