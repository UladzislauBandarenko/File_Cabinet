using FileCabinetApp.Utilities;

namespace FileCabinetApp.CommandHandlers
{
    public class EditCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public EditCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        public override void Handle(string command)
        {
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
                    var firstName = InputReader.ReadInput<string>("First name", RecordValidator.ValidateFirstName, s => s);
                    var lastName = InputReader.ReadInput<string>("Last name", RecordValidator.ValidateLastName, s => s);
                    var dateOfBirth = InputReader.ReadInput<DateTime>("Date of birth (mm/dd/yyyy)", RecordValidator.ValidateDateOfBirth, DateTime.Parse);
                    var age = InputReader.ReadInput<short>("Age", RecordValidator.ValidateAge, short.Parse);
                    var salary = InputReader.ReadInput<decimal>("Salary", RecordValidator.ValidateSalary, decimal.Parse);
                    var gender = InputReader.ReadInput<char>("Gender (M/F)", RecordValidator.ValidateGender, s => s[0]);

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