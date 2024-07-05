using System.Collections;
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
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            foreach (var record in this.records)
            {
                if (string.Equals(record.FirstName, firstName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return record;
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            foreach (var record in this.records)
            {
                if (string.Equals(record.LastName, lastName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return record;
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            if (DateTime.TryParse(dateOfBirth, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime date))
            {
                foreach (var record in this.records)
                {
                    if (record.DateOfBirth.Date == date.Date)
                    {
                        yield return record;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords(DefaultRecordPrinter));
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new FileCabinetMemoryIterator(new ReadOnlyCollection<FileCabinetRecord>(this.records));
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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
