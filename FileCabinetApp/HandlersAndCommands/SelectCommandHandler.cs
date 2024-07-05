using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FileCabinetApp.Models;

namespace FileCabinetApp.CommandHandlers
{
    public class SelectCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly IReadOnlyCollection<HelpMessage> helpMessages;

        public SelectCommandHandler(IFileCabinetService fileCabinetService, IReadOnlyCollection<HelpMessage> helpMessages)
        {
            this.fileCabinetService = fileCabinetService;
            this.helpMessages = helpMessages;
        }

        public override void Handle(string command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.StartsWith("select", StringComparison.OrdinalIgnoreCase))
            {
                this.ExecuteSelect(command);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(command);
            }
        }

        private void ExecuteSelect(string command)
        {
            var match = Regex.Match(command, @"select\s+(.*?)(?:\s+where\s+(.*?))?$", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                var selectHelp = this.helpMessages.FirstOrDefault(m => m.Command == "select");
                Console.WriteLine("Invalid select command format.");
                if (selectHelp != null)
                {
                    Console.WriteLine(selectHelp.DetailedDescription);
                }
                return;
            }

            var fields = match.Groups[1].Value.Split(',').Select(f => f.Trim()).ToList();
            var whereClause = match.Groups[2].Success ? match.Groups[2].Value : null;

            Dictionary<string, string> conditions = null;
            if (!string.IsNullOrEmpty(whereClause))
            {
                conditions = ParseWhereClause(whereClause);
            }

            var records = this.fileCabinetService.SelectRecords(fields, conditions);
            PrintRecords(records, fields);
        }

        private static Dictionary<string, string> ParseWhereClause(string whereClause)
        {
            var conditions = new Dictionary<string, string>();
            var parts = whereClause.Split(new[] { "and", "or" }, StringSplitOptions.RemoveEmptyEntries);
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

        private static void PrintRecords(IEnumerable<FileCabinetRecord> records, List<string> fields)
        {
            if (!records.Any())
            {
                Console.WriteLine("No records found.");
                return;
            }

            var columnWidths = CalculateColumnWidths(records, fields);
            PrintTableHeader(fields, columnWidths);
            PrintTableSeparator(columnWidths);

            foreach (var record in records)
            {
                PrintTableRow(record, fields, columnWidths);
            }

            PrintTableSeparator(columnWidths);
        }

        private static Dictionary<string, int> CalculateColumnWidths(IEnumerable<FileCabinetRecord> records, List<string> fields)
        {
            var columnWidths = new Dictionary<string, int>();
            foreach (var field in fields)
            {
                columnWidths[field] = field.Length;
            }

            foreach (var record in records)
            {
                foreach (var field in fields)
                {
                    var value = GetFieldValue(record, field);
                    columnWidths[field] = Math.Max(columnWidths[field], value.Length);
                }
            }

            return columnWidths;
        }

        private static void PrintTableHeader(List<string> fields, Dictionary<string, int> columnWidths)
        {
            Console.Write("|");
            foreach (var field in fields)
            {
                Console.Write($" {field.PadRight(columnWidths[field])} |");
            }
            Console.WriteLine();
        }

        private static void PrintTableSeparator(Dictionary<string, int> columnWidths)
        {
            Console.Write("+");
            foreach (var width in columnWidths.Values)
            {
                Console.Write(new string('-', width + 2) + "+");
            }
            Console.WriteLine();
        }

        private static void PrintTableRow(FileCabinetRecord record, List<string> fields, Dictionary<string, int> columnWidths)
        {
            Console.Write("|");
            foreach (var field in fields)
            {
                var value = GetFieldValue(record, field);
                if (int.TryParse(value, out _) || decimal.TryParse(value, out _))
                {
                    Console.Write($" {value.PadLeft(columnWidths[field])} |");
                }
                else
                {
                    Console.Write($" {value.PadRight(columnWidths[field])} |");
                }
            }
            Console.WriteLine();
        }

        private static string GetFieldValue(FileCabinetRecord record, string field)
        {
            switch (field.ToLowerInvariant())
            {
                case "id":
                    return record.Id.ToString();
                case "firstname":
                    return record.FirstName;
                case "lastname":
                    return record.LastName;
                case "dateofbirth":
                    return record.DateOfBirth.ToString("yyyy-MM-dd");
                case "age":
                    return record.Age.ToString();
                case "salary":
                    return record.Salary.ToString("F2");
                case "gender":
                    return record.Gender.ToString();
                default:
                    return string.Empty;
            }
        }
    }
}