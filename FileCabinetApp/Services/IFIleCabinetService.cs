using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a delegate for printing a record.
    /// </summary>
    /// <param name="record">The record to print.</param>
    /// <returns>The string representation of the record.</returns>
    public delegate string RecordPrinter(FileCabinetRecord record);

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
        /// <param name="printer">The printer to use.</param>
        ReadOnlyCollection<FileCabinetRecord> GetRecords(RecordPrinter printer);

        /// <summary>
        /// Gets the number of records in the file cabinet.
        /// </summary>
        /// <returns>The number of records.</returns>
        int GetStat();

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

        /// <summary>
        /// Checks if a record with the given id exists.
        /// </summary>
        /// <param name="id">The id of the record to check.</param>
        /// <returns>True if the record exists, false otherwise.</returns>
        bool RecordExists(int id);

        /// <summary>
        /// Inserts a new record with the specified id and personal information.
        /// </summary>
        /// <param name="id">The id of the new record.</param>
        /// <param name="personalInfo">The personal information for the new record.</param>
        /// <returns>The id of the newly inserted record.</returns>
        int InsertRecord(int id, PersonalInfo personalInfo);

        /// <summary>
        /// Deletes records based on the specified field and value.
        /// </summary>
        /// <param name="field">The field to match.</param>
        /// <param name="value">The value to match.</param>
        /// <returns>A list of deleted record IDs.</returns>
        ReadOnlyCollection<int> DeleteRecords(string field, string value);

        /// <summary>
        /// Updates records based on the specified field and value.
        /// </summary>
        /// <param name="fieldsToUpdate">The fields to update.</param>
        /// <param name="conditions">The conditions to match.</param>
        /// <returns>The number of updated records.</returns>
        int UpdateRecords(Dictionary<string, string> fieldsToUpdate, Dictionary<string, string> conditions);
    }
}