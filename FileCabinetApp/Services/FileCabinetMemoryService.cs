using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a service for working with the file cabinet.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
        private readonly IRecordValidator validator;

        // Add a dictionary to store memoized results
        private readonly Dictionary<string, ReadOnlyCollection<FileCabinetRecord>> memoizedResults = new Dictionary<string, ReadOnlyCollection<FileCabinetRecord>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validatorBuilder">The validator to use for validating personal information.</param>
        public FileCabinetMemoryService(ValidatorBuilder validatorBuilder)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder));
            }

            this.validator = validatorBuilder.Build();
        }

        /// <inheritdoc/>
        public bool RecordExists(int id)
        {
            return this.records.Exists(r => r.Id == id);
        }

        /// <inheritdoc/>
        public int PurgeRecords()
        {
            return 0; // No purging needed for memory storage
        }

        /// <inheritdoc/>
        public bool RemoveRecord(int id)
        {
            var record = this.records.Find(r => r.Id == id);
            if (record == null)
            {
                return false;
            }

            this.records.Remove(record);

            this.RemoveFromIndices(record);

            return true;
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            // Clear existing records and indices
            this.records.Clear();
            this.firstNameIndex.Clear();
            this.lastNameIndex.Clear();
            this.dateOfBirthIndex.Clear();

            // Add records from the snapshot
            foreach (var record in snapshot.Records)
            {
                if (record.FirstName is null || record.LastName is null)
                {
                    throw new ArgumentNullException(nameof(snapshot));
                }

                this.records.Add(record);
                AddToIndex(this.firstNameIndex, record.FirstName, record);
                AddToIndex(this.lastNameIndex, record.LastName, record);
                AddToIndex(this.dateOfBirthIndex, record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), record);
            }
        }

        /// <inheritdoc/>
        public int CreateRecord(PersonalInfo personalInfo)
        {
            if (personalInfo is null)
            {
                throw new ArgumentNullException(nameof(personalInfo));
            }

            this.ValidatePersonalInfo(personalInfo);

            int nextId = 1;
            if (this.records.Any())
            {
                nextId = this.records.Max(r => r.Id) + 1;
            }

            var record = new FileCabinetRecord
            {
                Id = nextId,
                FirstName = personalInfo.FirstName,
                LastName = personalInfo.LastName,
                DateOfBirth = personalInfo.DateOfBirth,
                Age = personalInfo.Age,
                Salary = personalInfo.Salary,
                Gender = personalInfo.Gender,
            };

            this.records.Add(record);
            AddToIndex(this.firstNameIndex, personalInfo.FirstName, record);
            AddToIndex(this.lastNameIndex, personalInfo.LastName, record);
            AddToIndex(this.dateOfBirthIndex, personalInfo.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), record);

            this.ClearMemoizedResults();

            return record.Id;
        }

        /// <inheritdoc/>
        public int InsertRecord(int id, PersonalInfo personalInfo)
        {
            if (personalInfo is null)
            {
                throw new ArgumentNullException(nameof(personalInfo));
            }

            this.ValidatePersonalInfo(personalInfo);

            if (this.records.Any(r => r.Id == id))
            {
                throw new ArgumentException($"Record with id {id} already exists.", nameof(id));
            }

            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = personalInfo.FirstName,
                LastName = personalInfo.LastName,
                DateOfBirth = personalInfo.DateOfBirth,
                Age = personalInfo.Age,
                Salary = personalInfo.Salary,
                Gender = personalInfo.Gender,
            };

            this.records.Add(record);
            AddToIndex(this.firstNameIndex, personalInfo.FirstName, record);
            AddToIndex(this.lastNameIndex, personalInfo.LastName, record);
            AddToIndex(this.dateOfBirthIndex, personalInfo.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), record);

            this.ClearMemoizedResults();

            return record.Id;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<int> DeleteRecords(string field, string value)
        {
            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            var deletedIds = new List<int>();
            var recordsToRemove = new List<FileCabinetRecord>();

            foreach (var record in this.records)
            {
                bool match = false;
                switch (field.ToLowerInvariant())
                {
                    case "id":
                        match = record.Id.ToString(CultureInfo.InvariantCulture) == value;
                        break;
                    case "firstname":
                        match = record.FirstName?.Equals(value, StringComparison.OrdinalIgnoreCase) ?? false;
                        break;
                    case "lastname":
                        match = record.LastName?.Equals(value, StringComparison.OrdinalIgnoreCase) ?? false;
                        break;
                    case "dateofbirth":
                        match = record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == value;
                        break;
                    case "age":
                        match = record.Age.ToString(CultureInfo.InvariantCulture) == value;
                        break;
                    case "salary":
                        match = record.Salary.ToString(CultureInfo.InvariantCulture) == value;
                        break;
                    case "gender":
                        match = record.Gender.ToString().Equals(value, StringComparison.OrdinalIgnoreCase);
                        break;
                }

                if (match)
                {
                    deletedIds.Add(record.Id);
                    recordsToRemove.Add(record);
                }
            }

            foreach (var record in recordsToRemove)
            {
                this.records.Remove(record);
                this.RemoveFromIndices(record);
            }

            this.ClearMemoizedResults();

            return new ReadOnlyCollection<int>(deletedIds);
        }

        /// <inheritdoc/>
        public int UpdateRecords(Dictionary<string, string> fieldsToUpdate, Dictionary<string, string> conditions)
        {
            if (fieldsToUpdate is null)
            {
                throw new ArgumentNullException(nameof(fieldsToUpdate));
            }

            if (conditions is null)
            {
                throw new ArgumentNullException(nameof(conditions));
            }

            int updatedCount = 0;

            foreach (var record in this.records)
            {
                if (MatchesConditions(record, conditions))
                {
                    UpdateRecord(record, fieldsToUpdate);
                    updatedCount++;
                }
            }

            this.ClearMemoizedResults();

            return updatedCount;
        }

        /// <summary>
        /// Selects the records.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="conditions">The conditions.</param>
        /// <returns>The records.</returns>
        public IEnumerable<FileCabinetRecord> SelectRecords(List<string> fields, Dictionary<string, string> conditions)
        {
            IEnumerable<FileCabinetRecord> records = this.records;

            if (conditions != null && conditions.Count > 0)
            {
                records = records.Where(record => MatchesConditions(record, conditions));
            }

            return records;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords(RecordPrinter printer)
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.records.Select(r => new FileCabinetRecord
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                DateOfBirth = r.DateOfBirth,
                Age = r.Age,
                Salary = r.Salary,
                Gender = r.Gender,
                PrintedRepresentation = printer(r),
            }).ToList());
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            return this.records.Count;
        }

        /// <inheritdoc/>

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            string key = $"FirstName:{firstName}";
            if (this.memoizedResults.TryGetValue(key, out var cachedResult))
            {
                return cachedResult;
            }

            var result = new ReadOnlyCollection<FileCabinetRecord>(FindByIndex(this.firstNameIndex, firstName));
            this.memoizedResults[key] = result;
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            string key = $"LastName:{lastName}";
            if (this.memoizedResults.TryGetValue(key, out var cachedResult))
            {
                return cachedResult;
            }

            var result = new ReadOnlyCollection<FileCabinetRecord>(FindByIndex(this.lastNameIndex, lastName));
            this.memoizedResults[key] = result;
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            string key = $"DateOfBirth:{dateOfBirth}";
            if (this.memoizedResults.TryGetValue(key, out var cachedResult))
            {
                return cachedResult;
            }

            if (DateTime.TryParse(dateOfBirth, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                var result = new ReadOnlyCollection<FileCabinetRecord>(FindByIndex(this.dateOfBirthIndex, date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
                this.memoizedResults[key] = result;
                return result;
            }

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords(DefaultRecordPrinter));
        }

        private static string DefaultRecordPrinter(FileCabinetRecord record)
        {
            return $"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MMM-dd}, {record.Age}, {record.Salary:C2}, {record.Gender}";
        }

        private static void RemoveFromIndex(Dictionary<string, List<FileCabinetRecord>> index, string key, FileCabinetRecord record)
        {
            if (index.ContainsKey(key))
            {
                index[key].Remove(record);
            }
        }

        private static List<FileCabinetRecord> FindByIndex(Dictionary<string, List<FileCabinetRecord>> index, string key)
        {
            if (index.TryGetValue(key, out var records))
            {
                return records;
            }

            return new List<FileCabinetRecord>();
        }

        private static void AddToIndex(Dictionary<string, List<FileCabinetRecord>> index, string key, FileCabinetRecord record)
        {
            if (!index.ContainsKey(key))
            {
                index[key] = new List<FileCabinetRecord>();
            }

            index[key].Add(record);
        }

        private static bool MatchesConditions(FileCabinetRecord record, Dictionary<string, string> conditions)
        {
            foreach (var condition in conditions)
            {
                switch (condition.Key.ToLowerInvariant())
                {
                    case "id":
                        if (record.Id.ToString(CultureInfo.InvariantCulture) != condition.Value)
                        {
                            return false;
                        }

                        break;
                    case "firstname":
                        if (record.FirstName?.Equals(condition.Value, StringComparison.OrdinalIgnoreCase) != true)
                        {
                            return false;
                        }

                        break;
                    case "lastname":
                        if (record.LastName?.Equals(condition.Value, StringComparison.OrdinalIgnoreCase) != true)
                        {
                            return false;
                        }

                        break;
                    case "dateofbirth":
                        if (record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) != condition.Value)
                        {
                            return false;
                        }

                        break;
                    case "age":
                        if (record.Age.ToString(CultureInfo.InvariantCulture) != condition.Value)
                        {
                            return false;
                        }

                        break;
                    case "salary":
                        if (record.Salary.ToString(CultureInfo.InvariantCulture) != condition.Value)
                        {
                            return false;
                        }

                        break;
                    case "gender":
                        if (record.Gender.ToString().Equals(condition.Value, StringComparison.OrdinalIgnoreCase) != true)
                        {
                            return false;
                        }

                        break;
                }
            }

            return true;
        }

        private static void UpdateRecord(FileCabinetRecord record, Dictionary<string, string> fieldsToUpdate)
        {
            foreach (var field in fieldsToUpdate)
            {
                switch (field.Key.ToLowerInvariant())
                {
                    case "firstname":
                        record.FirstName = field.Value;
                        break;
                    case "lastname":
                        record.LastName = field.Value;
                        break;
                    case "dateofbirth":
                        if (DateTime.TryParse(field.Value, out DateTime dateOfBirth))
                        {
                            record.DateOfBirth = dateOfBirth;
                        }

                        break;
                    case "age":
                        if (short.TryParse(field.Value, out short age))
                        {
                            record.Age = age;
                        }

                        break;
                    case "salary":
                        if (decimal.TryParse(field.Value, out decimal salary))
                        {
                            record.Salary = salary;
                        }

                        break;
                    case "gender":
                        if (char.TryParse(field.Value, out char gender))
                        {
                            record.Gender = gender;
                        }

                        break;
                }
            }
        }

        private void ValidatePersonalInfo(PersonalInfo personalInfo)
        {
            if (!this.validator.ValidateFirstName(personalInfo.FirstName, out string errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }

            if (!this.validator.ValidateLastName(personalInfo.LastName, out errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }

            if (!this.validator.ValidateDateOfBirth(personalInfo.DateOfBirth, out errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }

            if (!this.validator.ValidateAge(personalInfo.Age, out errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }

            if (!this.validator.ValidateSalary(personalInfo.Salary, out errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }

            if (!this.validator.ValidateGender(personalInfo.Gender, out errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }
        }

        private void RemoveFromIndices(FileCabinetRecord record)
        {
            if (record.FirstName is null || record.LastName is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            RemoveFromIndex(this.firstNameIndex, record.FirstName, record);
            RemoveFromIndex(this.lastNameIndex, record.LastName, record);
            RemoveFromIndex(this.dateOfBirthIndex, record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), record);
        }

        private void AddToIndices(FileCabinetRecord record)
        {
            if (record.FirstName is null || record.LastName is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            AddToIndex(this.firstNameIndex, record.FirstName, record);
            AddToIndex(this.lastNameIndex, record.LastName, record);
            AddToIndex(this.dateOfBirthIndex, record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), record);
        }

        private void ClearMemoizedResults()
        {
            this.memoizedResults.Clear();
        }
    }
}
