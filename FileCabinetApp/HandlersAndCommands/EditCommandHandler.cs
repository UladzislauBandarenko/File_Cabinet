using System.Globalization;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Edit command handler.
    /// </summary>
    public class EditCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly IReadOnlyCollection<HelpMessage> helpMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <param name="helpMessages">The help messages.</param>
        public EditCommandHandler(IFileCabinetService fileCabinetService, IReadOnlyCollection<HelpMessage> helpMessages)
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

            if (command.StartsWith("edit", StringComparison.InvariantCultureIgnoreCase))
            {
                string[] parts = command.Split(' ', 2);
                if (parts.Length < 2 || !int.TryParse(parts[1], out int id))
                {
                    var editHelp = this.helpMessages.FirstOrDefault(m => m.Command == "edit");
                    Console.WriteLine("Invalid command format.");
                    if (editHelp != null)
                    {
                        Console.WriteLine(editHelp.DetailedDescription);
                    }
                    return;
                }

                try
                {
                    // Check if the record exists before prompting for input
                    if (!this.fileCabinetService.RecordExists(id))
                    {
                        Console.WriteLine($"Record with id {id} does not exist.");
                        return;
                    }

                    Console.Write("First name: ");
                    var firstName = Console.ReadLine() ?? string.Empty;
                    Console.Write("Last name: ");
                    var lastName = Console.ReadLine() ?? string.Empty;
                    Console.Write("Date of birth (yyyy-mm-dd): ");
                    if (!DateTime.TryParse(Console.ReadLine(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                    {
                        Console.WriteLine("Invalid date format. Please use yyyy-mm-dd.");
                        return;
                    }

                    Console.Write("Age: ");
                    if (!short.TryParse(Console.ReadLine(), out short age))
                    {
                        Console.WriteLine("Invalid age format. Please enter a number.");
                        return;
                    }

                    Console.Write("Salary: ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal salary))
                    {
                        Console.WriteLine("Invalid salary format. Please enter a number.");
                        return;
                    }

                    Console.Write("Gender (M/F): ");
                    if (!char.TryParse(Console.ReadLine(), out char gender))
                    {
                        Console.WriteLine("Invalid gender. Please enter a character. (M/F)");
                        return;
                    }

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