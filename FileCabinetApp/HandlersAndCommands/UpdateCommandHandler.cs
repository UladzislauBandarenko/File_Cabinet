using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// The update command handler.
    /// </summary>
    public class UpdateCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly IReadOnlyCollection<HelpMessage> helpMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <param name="helpMessages">The help messages.</param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService, IReadOnlyCollection<HelpMessage> helpMessages)
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

            if (command.StartsWith("update", StringComparison.OrdinalIgnoreCase))
            {
                this.ExecuteUpdate(command);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }

        /// <summary>
        /// Parses the set clause.
        /// </summary>
        /// <param name="setClause">The set clause.</param>
        /// <returns>Fields to update.</returns>
        private static Dictionary<string, string> ParseSetClause(string setClause)
        {
            var fields = new Dictionary<string, string>();
            var parts = setClause.Split(',');
            foreach (var part in parts)
            {
                var keyValue = part.Split('=');
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim().ToLowerInvariant();
                    var value = keyValue[1].Trim().Trim('\'', '"');
                    fields[key] = value;
                }
            }

            return fields;
        }

        private static Dictionary<string, string> ParseWhereClause(string whereClause)
        {
            var conditions = new Dictionary<string, string>();
            var parts = whereClause.Split(new[] { "and" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var keyValue = part.Split('=');
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim().ToLowerInvariant();
                    var value = keyValue[1].Trim().Trim('\'', '"');
                    conditions[key] = value;
                }
            }

            return conditions;
        }

        private void ExecuteUpdate(string command)
        {
            var match = Regex.Match(command, @"update\s+set\s+(.*?)\s+where\s+(.*)", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                var updateHelp = this.helpMessages.FirstOrDefault(m => m.Command == "update");
                Console.WriteLine("Invalid update command format.");
                if (updateHelp != null)
                {
                    Console.WriteLine(updateHelp.DetailedDescription);
                }

                return;
            }

            var setClause = match.Groups[1].Value;
            var whereClause = match.Groups[2].Value;

            var fieldsToUpdate = ParseSetClause(setClause);
            var conditions = ParseWhereClause(whereClause);

            try
            {
                int updatedCount = this.fileCabinetService.UpdateRecords(fieldsToUpdate, conditions);
                Console.WriteLine($"{updatedCount} record(s) updated.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Failed to update records: {ex.Message}");
            }
        }
    }
}