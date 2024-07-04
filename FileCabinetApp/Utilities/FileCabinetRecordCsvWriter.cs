using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter writer;

        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Write(ReadOnlyCollection<FileCabinetRecord> records)
        {
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