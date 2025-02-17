using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FileCabinetApp
{
    /// <summary>
    /// ServiceMeter class.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
            this.stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public IFileCabinetService Service => this.service;

        /// <inheritdoc/>
        public int CreateRecord(PersonalInfo personalInfo)
        {
            this.stopwatch.Restart();
            var result = this.service.CreateRecord(personalInfo);
            this.stopwatch.Stop();
            Console.WriteLine($"CreateRecord method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public int InsertRecord(int id, PersonalInfo personalInfo)
        {
            this.stopwatch.Restart();
            var result = this.service.InsertRecord(id, personalInfo);
            this.stopwatch.Stop();
            Console.WriteLine($"InsertRecord method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> DeleteRecords(string field, string value)
        {
            this.stopwatch.Restart();
            var result = this.service.DeleteRecords(field, value);
            this.stopwatch.Stop();
            Console.WriteLine($"DeleteRecords method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public int UpdateRecords(Dictionary<string, string> fieldsToUpdate, Dictionary<string, string> conditions)
        {
            this.stopwatch.Restart();
            var result = this.service.UpdateRecords(fieldsToUpdate, conditions);
            this.stopwatch.Stop();
            Console.WriteLine($"UpdateRecords method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> SelectRecords(List<string> fields, Dictionary<string, string> conditions)
        {
            this.stopwatch.Restart();
            var result = this.service.SelectRecords(fields, conditions);
            this.stopwatch.Stop();
            Console.WriteLine($"SelectRecords method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords(RecordPrinter printer)
        {
            this.stopwatch.Restart();
            var result = this.service.GetRecords(printer);
            this.stopwatch.Stop();
            Console.WriteLine($"GetRecords method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            this.stopwatch.Restart();
            var result = this.service.GetStat();
            this.stopwatch.Stop();
            Console.WriteLine($"GetStat method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByFirstName(firstName);
            this.stopwatch.Stop();
            Console.WriteLine($"FindByFirstName method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByLastName(lastName);
            this.stopwatch.Stop();
            Console.WriteLine($"FindByLastName method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            this.stopwatch.Restart();
            var result = this.service.FindByDateOfBirth(dateOfBirth);
            this.stopwatch.Stop();
            Console.WriteLine($"FindByDateOfBirth method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.stopwatch.Restart();
            var result = this.service.MakeSnapshot();
            this.stopwatch.Stop();
            Console.WriteLine($"MakeSnapshot method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.stopwatch.Restart();
            this.service.Restore(snapshot);
            this.stopwatch.Stop();
            Console.WriteLine($"Restore method execution time: {this.stopwatch.ElapsedTicks} ticks");
        }

        /// <inheritdoc/>
        public bool RemoveRecord(int id)
        {
            this.stopwatch.Restart();
            var result = this.service.RemoveRecord(id);
            this.stopwatch.Stop();
            Console.WriteLine($"RemoveRecord method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public int PurgeRecords()
        {
            this.stopwatch.Restart();
            var result = this.service.PurgeRecords();
            this.stopwatch.Stop();
            Console.WriteLine($"PurgeRecords method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }

        /// <inheritdoc/>
        public bool RecordExists(int id)
        {
            this.stopwatch.Restart();
            var result = this.service.RecordExists(id);
            this.stopwatch.Stop();
            Console.WriteLine($"RecordExists method execution time: {this.stopwatch.ElapsedTicks} ticks");
            return result;
        }
    }
}