using System;
using System.Globalization;
using System.Text.RegularExpressions;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Insert command handler.
    /// </summary>
    public class InsertCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly IReadOnlyCollection<HelpMessage> helpMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <param name="helpMessages">The help messages.</param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService, IReadOnlyCollection<HelpMessage> helpMessages)
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

            if (command.StartsWith("insert", StringComparison.InvariantCultureIgnoreCase))
            {
                this.ExecuteInsert(command);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }

        private void ExecuteInsert(string command)
        {
            var match = Regex.Match(command, @"insert\s*\((.*?)\)\s*values\s*\((.*?)\)", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                var insertHelp = this.helpMessages.FirstOrDefault(m => m.Command == "insert");
                Console.WriteLine("Invalid insert command format.");
                if (insertHelp != null)
                {
                    Console.WriteLine(insertHelp.DetailedDescription);
                }

                return;
            }

            var fields = match.Groups[1].Value.Split(',').Select(f => f.Trim()).ToArray();
            var values = match.Groups[2].Value.Split(',').Select(v => v.Trim().Trim('\'')).ToArray();

            if (fields.Length != values.Length)
            {
                Console.WriteLine("The number of fields and values do not match.");
                return;
            }

            var personalInfo = new PersonalInfo(string.Empty, string.Empty, DateTime.MinValue, 0, 0, ' ');
            int id = 0;

            for (int i = 0; i < fields.Length; i++)
            {
                switch (fields[i].ToLowerInvariant())
                {
                    case "id":
                        if (!int.TryParse(values[i], out id))
                        {
                            Console.WriteLine($"Invalid id value: {values[i]}");
                            return;
                        }

                        break;
                    case "firstname":
                        personalInfo.FirstName = values[i];
                        break;
                    case "lastname":
                        personalInfo.LastName = values[i];
                        break;
                    case "dateofbirth":
                        if (!DateTime.TryParse(values[i], out DateTime dateOfBirth))
                        {
                            Console.WriteLine($"Invalid date of birth: {values[i]}");
                            return;
                        }

                        personalInfo.DateOfBirth = dateOfBirth;
                        break;
                    case "age":
                        if (!short.TryParse(values[i], out short age))
                        {
                            Console.WriteLine($"Invalid age value: {values[i]}");
                            return;
                        }

                        personalInfo.Age = age;
                        break;
                    case "salary":
                        if (!decimal.TryParse(values[i], out decimal salary))
                        {
                            Console.WriteLine($"Invalid salary value: {values[i]}");
                            return;
                        }

                        personalInfo.Salary = salary;
                        break;
                    case "gender":
                        if (!char.TryParse(values[i], out char gender))
                        {
                            Console.WriteLine($"Invalid gender value: {values[i]}");
                            return;
                        }

                        personalInfo.Gender = gender;
                        break;
                    default:
                        Console.WriteLine($"Unknown field: {fields[i]}");
                        return;
                }
            }

            try
            {
                int insertedId = this.fileCabinetService.InsertRecord(id, personalInfo);
                Console.WriteLine($"Record #{insertedId} is inserted.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Failed to insert record: {ex.Message}");
            }
        }
    }
}
