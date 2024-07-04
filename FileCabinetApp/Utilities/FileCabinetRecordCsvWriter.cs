using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet record CSV writer.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes the specified records.
        /// </summary>
        /// <param name="records">The records.</param>
        public void Write(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            this.writer.WriteLine("Id,First Name,Last Name,Date of Birth,Age,Salary,Gender");

            foreach (var record in records)
            {
                this.writer.WriteLine(
                    $"{record.Id}," +
                    $"\"{record.FirstName}\"," +
                    $"\"{record.LastName}\"," +
                    $"{record.DateOfBirth:yyyy-MM-dd}," +
                    $"{record.Age}," +
                    $"{record.Salary.ToString("F2", CultureInfo.InvariantCulture)}," +
                    $"{record.Gender}");
            }
        }
    }
}