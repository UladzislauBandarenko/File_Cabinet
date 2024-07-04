using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvReader
    {
        private readonly TextReader reader;

        public FileCabinetRecordCsvReader(TextReader reader)
        {
            this.reader = reader;
        }

        public FileCabinetServiceSnapshot ReadAll()
        {
            var records = new List<FileCabinetRecord>();
            string line;

            // Skip the header
            reader.ReadLine();

            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                if (values.Length != 7)
                {
                    throw new FormatException($"Invalid record format: {line}");
                }

                var record = new FileCabinetRecord
                {
                    Id = int.Parse(values[0]),
                    FirstName = values[1].Trim('"'),
                    LastName = values[2].Trim('"'),
                    DateOfBirth = DateTime.Parse(values[3], CultureInfo.InvariantCulture),
                    Age = short.Parse(values[4]),
                    Salary = decimal.Parse(values[5], CultureInfo.InvariantCulture),
                    Gender = char.Parse(values[6])
                };

                records.Add(record);
            }

            return new FileCabinetServiceSnapshot(new System.Collections.ObjectModel.ReadOnlyCollection<FileCabinetRecord>(records));
        }
    }
}