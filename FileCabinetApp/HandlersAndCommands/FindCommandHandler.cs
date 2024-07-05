using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Find command handler.
    /// </summary>
    public class FindCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService)
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

                IFileCabinetRecordIterator iterator;

                switch (property)
                {
                    case "firstname":
                        iterator = this.fileCabinetService.FindByFirstName(value);
                        break;
                    case "lastname":
                        iterator = this.fileCabinetService.FindByLastName(value);
                        break;
                    case "dateofbirth":
                        iterator = this.fileCabinetService.FindByDateOfBirth(value);
                        break;
                    default:
                        Console.WriteLine($"Searching by {property} is not supported.");
                        return;
                }

                int count = 0;
                while (iterator.MoveNext())
                {
                    var record = iterator.Current;
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MMM-dd}, {record.Age}, {record.Salary:C2}, {record.Gender}");
                    count++;
                }

                if (count == 0)
                {
                    Console.WriteLine("No records found.");
                }
                else
                {
                    Console.WriteLine($"{count} record(s) found.");
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}