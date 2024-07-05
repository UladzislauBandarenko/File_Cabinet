using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Service logger class.
    /// </summary>
    public class ServiceLogger : IFileCabinetService, IDisposable
    {
        private readonly IFileCabinetService service;
        private readonly StreamWriter logWriter;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">Service to log.</param>
        /// <param name="logFilePath">Path to the log file.</param>
        public ServiceLogger(IFileCabinetService service, string logFilePath)
        {
            this.service = service;
            this.logWriter = new StreamWriter(logFilePath, true);
        }

        /// <inheritdoc/>
        public int CreateRecord(PersonalInfo personalInfo)
        {
            this.Log("CreateRecord", $"PersonalInfo: {personalInfo}");
            return this.service.CreateRecord(personalInfo);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords(RecordPrinter printer)
        {
            if (printer is null)
            {
                throw new ArgumentNullException(nameof(printer));
            }

            this.Log("GetRecords", $"Printer: {printer.Method.Name}");
            return this.service.GetRecords(printer);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            this.Log("GetStat");
            return this.service.GetStat();
        }

        /// <inheritdoc/>
        public void EditRecord(int id, PersonalInfo personalInfo)
        {
            this.Log("EditRecord", $"Id: {id}, PersonalInfo: {personalInfo}");
            this.service.EditRecord(id, personalInfo);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.Log("FindByFirstName", $"FirstName: {firstName}");
            return this.service.FindByFirstName(firstName);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.Log("FindByLastName", $"LastName: {lastName}");
            return this.service.FindByLastName(lastName);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            this.Log("FindByDateOfBirth", $"DateOfBirth: {dateOfBirth}");
            return this.service.FindByDateOfBirth(dateOfBirth);
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.Log("MakeSnapshot");
            return this.service.MakeSnapshot();
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.Log("Restore", $"Snapshot: {snapshot}");
            this.service.Restore(snapshot);
        }

        /// <inheritdoc/>
        public bool RemoveRecord(int id)
        {
            this.Log("RemoveRecord", $"Id: {id}");
            return this.service.RemoveRecord(id);
        }

        /// <inheritdoc/>
        public int PurgeRecords()
        {
            this.Log("PurgeRecords");
            return this.service.PurgeRecords();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the service logger.
        /// </summary>
        /// <param name="disposing">Whether to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.logWriter.Dispose();
                }

                this.disposed = true;
            }
        }

        private void Log(string methodName, string parameters = "")
        {
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {methodName}";
            if (!string.IsNullOrEmpty(parameters))
            {
                logMessage += $" - Parameters: {parameters}";
            }

            this.logWriter.WriteLine(logMessage);
            this.logWriter.Flush();
        }
    }
}