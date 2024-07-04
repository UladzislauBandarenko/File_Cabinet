using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            var outputTypeOption = new Option<string>(
                new[] { "--output-type", "-t" },
                description: "Output format type (csv, xml)");

            var outputOption = new Option<string>(
                new[] { "--output", "-o" },
                description: "Output file name");

            var recordsAmountOption = new Option<int>(
                new[] { "--records-amount", "-a" },
                description: "Amount of generated records");

            var startIdOption = new Option<int>(
                new[] { "--start-id", "-i" },
                description: "ID value to start");

            var rootCommand = new RootCommand("File Cabinet Generator")
            {
                outputTypeOption,
                outputOption,
                recordsAmountOption,
                startIdOption
            };

            rootCommand.SetHandler(GenerateRecords, 
                outputTypeOption, outputOption, recordsAmountOption, startIdOption);

            return await rootCommand.InvokeAsync(args);
        }

        static void GenerateRecords(string outputType, string output, int recordsAmount, int startId)
        {
            var records = GenerateRandomRecords(recordsAmount, startId);

            switch (outputType.ToLower())
            {
                case "csv":
                    ExportToCsv(records, output);
                    break;
                case "xml":
                    ExportToXml(records, output);
                    break;
                default:
                    Console.WriteLine($"Unsupported output type: {outputType}");
                    return;
            }

            Console.WriteLine($"{recordsAmount} records were written to {output}.");
        }

        static List<FileCabinetRecord> GenerateRandomRecords(int count, int startId)
        {
            var records = new List<FileCabinetRecord>();
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                records.Add(new FileCabinetRecord
                {
                    Id = startId + i,
                    FirstName = GenerateRandomName(random, 2, 60),
                    LastName = GenerateRandomName(random, 2, 60),
                    DateOfBirth = GenerateRandomDate(random, new DateTime(1950, 1, 1, 0, 0, 0, DateTimeKind.Utc), DateTime.Now),
                    Age = (short)random.Next(0, 121),
                    Salary = (decimal)random.Next(0, 1000001) / 100,
                    Gender = random.Next(2) == 0 ? 'M' : 'F'
                });
            }

            return records;
        }

        static string GenerateRandomName(Random random, int minLength, int maxLength)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            int length = random.Next(minLength, maxLength + 1);
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static DateTime GenerateRandomDate(Random random, DateTime start, DateTime end)
        {
            int range = (end - start).Days;
            return start.AddDays(random.Next(range));
        }

        static void ExportToCsv(List<FileCabinetRecord> records, string outputFile)
        {
            using (var writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("Id,First Name,Last Name,Date of Birth,Age,Salary,Gender");
                foreach (var record in records)
                {
                    writer.WriteLine($"{record.Id},\"{record.FirstName}\",\"{record.LastName}\",{record.DateOfBirth:yyyy-MM-dd},{record.Age},{record.Salary:F2},{record.Gender}");
                }
            }
        }

        static void ExportToXml(List<FileCabinetRecord> records, string outputFile)
        {
            var serializer = new XmlSerializer(typeof(List<FileCabinetRecord>), new XmlRootAttribute("records"));
            using (var writer = new StreamWriter(outputFile))
            {
                serializer.Serialize(writer, records);
            }
        }
    }
}