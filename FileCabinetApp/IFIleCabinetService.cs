using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a service for working with the file cabinet.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates a record in the file cabinet.
        /// </summary>
        /// <param name="personalInfo">The personal information of the record.</param>
        /// <returns>The id of the created record.</returns>
        int CreateRecord(PersonalInfo personalInfo);

        /// <summary>
        /// Gets all records from the file cabinet.
        /// </summary>
        /// <returns>The collection of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Gets the number of records in the file cabinet.
        /// </summary>
        /// <returns>The number of records.</returns>
        int GetStat();

        /// <summary>
        /// Edits a record in the file cabinet.
        /// </summary>
        /// <param name="id">The id of the record to edit.</param>
        /// <param name="personalInfo">The personal information of the record.</param>
        void EditRecord(int id, PersonalInfo personalInfo);

        /// <summary>
        /// Finds records by first name.
        /// </summary>
        /// <param name="firstName">The first name to search for.</param>
        /// <returns>The collection of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Finds records by last name.
        /// </summary>
        /// <param name="lastName">The last name to search for.</param>
        /// <returns>The collection of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Finds records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth to search for.</param>
        /// <returns>The collection of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

        /// <summary>
        /// Makes a snapshot of the file cabinet.
        /// </summary>
        /// <returns>The snapshot of the file cabinet.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores the file cabinet from a snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot to restore from.</param>
        void Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Removes a record from the file cabinet.
        /// </summary>
        /// <param name="id">The id of the record to remove.</param>
        /// <returns>True if the record is removed, false otherwise.</returns>
        bool RemoveRecord(int id);

        /// <summary>
        /// Purges deleted records from the file cabinet.
        /// </summary>
        /// <returns>The number of purged records.</returns>
        int PurgeRecords();
    }
}