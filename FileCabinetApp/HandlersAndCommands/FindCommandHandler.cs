using System;
using System.Collections.ObjectModel;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Find command handler.
    /// </summary>
    public class FindCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly IReadOnlyCollection<HelpMessage> helpMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <param name="helpMessages">The help messages.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService, IReadOnlyCollection<HelpMessage> helpMessages)
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

            if (command.StartsWith("find", StringComparison.InvariantCultureIgnoreCase))
            {
                var inputs = command.Split(' ', 3);
                if (inputs.Length < 3)
                {
                    var findHelp = this.helpMessages.FirstOrDefault(m => m.Command == "find");
                    Console.WriteLine("Invalid command format.");
                    if (findHelp != null)
                    {
                        Console.WriteLine(findHelp.DetailedDescription);
                    }

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