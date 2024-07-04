using FileCabinetApp.Utilities;

namespace FileCabinetApp.CommandHandlers
{
    public class CreateCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public CreateCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        public override void Handle(string command)
        {
            if (command.Equals("create", StringComparison.InvariantCultureIgnoreCase))
            {
                var firstName = InputReader.ReadInput<string>("First name", RecordValidator.ValidateFirstName, s => s);
                var lastName = InputReader.ReadInput<string>("Last name", RecordValidator.ValidateLastName, s => s);
                var dateOfBirth = InputReader.ReadInput<DateTime>("Date of birth (mm/dd/yyyy)", RecordValidator.ValidateDateOfBirth, DateTime.Parse);
                var age = InputReader.ReadInput<short>("Age", RecordValidator.ValidateAge, short.Parse);
                var salary = InputReader.ReadInput<decimal>("Salary", RecordValidator.ValidateSalary, decimal.Parse);
                var gender = InputReader.ReadInput<char>("Gender (M/F)", RecordValidator.ValidateGender, s => s[0]);

                try
                {
                    var personalInfo = new PersonalInfo(firstName, lastName, dateOfBirth, age, salary, gender);
                    var recordId = this.fileCabinetService.CreateRecord(personalInfo);
                    Console.WriteLine($"Record #{recordId} is created.");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Failed to create record: {ex.Message}");
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}