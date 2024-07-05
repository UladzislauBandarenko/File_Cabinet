using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Delete command handler.
    /// </summary>
    public class DeleteCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
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

            if (command.StartsWith("delete", StringComparison.InvariantCultureIgnoreCase))
            {
                var match = Regex.Match(command, @"delete\s+where\s+(\w+)\s*=\s*'([^']*)'", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string field = match.Groups[1].Value.ToLowerInvariant();
                    string value = match.Groups[2].Value;

                    var deletedIds = this.fileCabinetService.DeleteRecords(field, value);

                    if (deletedIds.Any())
                    {
                        if (deletedIds.Count == 1)
                        {
                            Console.WriteLine($"Record #{deletedIds[0]} is deleted.");
                        }
                        else
                        {
                            Console.WriteLine($"Records #{string.Join(", #", deletedIds)} are deleted.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No records found matching the criteria.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid delete command format. Usage: delete where <field> = '<value>'");
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }
    }
}