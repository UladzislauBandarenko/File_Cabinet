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

            return record.Id;
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
        public void EditRecord(int id, PersonalInfo personalInfo)
        {
            if (personalInfo is null)
            {
                throw new ArgumentNullException(nameof(personalInfo));
            }

            var record = this.records.Find(r => r.Id == id);
            if (record == null)
            {
                throw new ArgumentException($"Record with id {id} does not exist.", nameof(id));
            }

            this.ValidatePersonalInfo(personalInfo);

            this.RemoveFromIndices(record);

            record.FirstName = personalInfo.FirstName;
            record.LastName = personalInfo.LastName;
            record.DateOfBirth = personalInfo.DateOfBirth;
            record.Age = personalInfo.Age;
            record.Salary = personalInfo.Salary;
            record.Gender = personalInfo.Gender;

            this.AddToIndices(record);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return new ReadOnlyCollection<FileCabinetRecord>(FindByIndex(this.firstNameIndex, firstName));
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return new ReadOnlyCollection<FileCabinetRecord>(FindByIndex(this.lastNameIndex, lastName));
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            if (DateTime.TryParse(dateOfBirth, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(FindByIndex(this.dateOfBirthIndex, date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
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
    }
}
