using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Edit command handler.
    /// </summary>
    public class EditCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        public EditCommandHandler(IFileCabinetService fileCabinetService)
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

            if (command.StartsWith("edit", StringComparison.InvariantCultureIgnoreCase))
            {
                string[] parts = command.Split(' ', 2);
                if (parts.Length < 2 || !int.TryParse(parts[1], out int id))
                {
                    Console.WriteLine("Invalid command format. Usage: edit <id>");
                    return;
                }

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
                    this.fileCabinetService.EditRecord(id, personalInfo);
                    Console.WriteLine($"Record #{id} is updated.");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Failed to edit record: {ex.Message}");
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}