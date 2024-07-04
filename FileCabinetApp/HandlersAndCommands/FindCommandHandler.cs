using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp.CommandHandlers
{
    public class FindCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public FindCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        public override void Handle(string command)
        {
            if (command.StartsWith("find", StringComparison.InvariantCultureIgnoreCase))
            {
                var inputs = command.Split(' ', 3);
                if (inputs.Length < 3)
                {
                    Console.WriteLine("Invalid command format. Usage: find <property> <value>");
                    return;
                }

                var property = inputs[1].ToLowerInvariant();
                var value = inputs[2].Trim('"');

                ReadOnlyCollection<FileCabinetRecord> records;

                switch (property)
                {
                    case "firstname":
                        records = this.fileCabinetService.FindByFirstName(value);
                        break;
                    case "lastname":
                        records = this.fileCabinetService.FindByLastName(value);
                        break;
                    case "dateofbirth":
                        records = this.fileCabinetService.FindByDateOfBirth(value);
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
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}