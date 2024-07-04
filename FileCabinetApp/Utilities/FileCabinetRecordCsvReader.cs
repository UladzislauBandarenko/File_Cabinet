using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a reader for CSV files.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly TextReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        public FileCabinetRecordCsvReader(TextReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads all records from the reader.
        /// </summary>
        /// <returns>The snapshot of the records.</returns>
        public FileCabinetServiceSnapshot ReadAll()
        {
            var records = new List<FileCabinetRecord>();
            string line;

            // Skip the header
            this.reader.ReadLine();

            while ((line = this.reader.ReadLine() !) != null)
            {
                var values = line.Split(',');
                if (values.Length != 7)
                {
                    throw new FormatException($"Invalid record format: {line}");
                }

                var record = new FileCabinetRecord
                {
                    Id = int.Parse(values[0], CultureInfo.InvariantCulture),
                    FirstName = values[1].Trim('"'),
                    LastName = values[2].Trim('"'),
                    DateOfBirth = DateTime.Parse(values[3], CultureInfo.InvariantCulture),
                    Age = short.Parse(values[4], CultureInfo.InvariantCulture),
                    Salary = decimal.Parse(values[5], CultureInfo.InvariantCulture),
                    Gender = char.Parse(values[6]),
                };

                records.Add(record);
            }

            return new FileCabinetServiceSnapshot(new System.Collections.ObjectModel.ReadOnlyCollection<FileCabinetRecord>(records));
        }
    }
}