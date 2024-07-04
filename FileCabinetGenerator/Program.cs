using System;
using System.CommandLine;
using System.IO;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    class Program
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
            throw new NotImplementedException();
        }
    }
}