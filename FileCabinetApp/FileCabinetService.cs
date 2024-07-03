using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
        private readonly IRecordValidator validator;

        public FileCabinetService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        public int CreateRecord(PersonalInfo personalInfo)
        {
            this.ValidatePersonalInfo(personalInfo);

            var record = new FileCabinetRecord
            {
                Id = this.records.Count + 1,
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

        public FileCabinetRecord[] GetRecords()
        {
            return this.records.ToArray();
        }

        public int GetStat()
        {
            return this.records.Count;
        }

        public void EditRecord(int id, PersonalInfo personalInfo)
        {
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

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            return FindByIndex(this.firstNameIndex, firstName);
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            return FindByIndex(this.lastNameIndex, lastName);
        }

        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            if (DateTime.TryParse(dateOfBirth, out DateTime date))
            {
                return FindByIndex(this.dateOfBirthIndex, date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }

            return Array.Empty<FileCabinetRecord>();
        }

        private void ValidatePersonalInfo(PersonalInfo personalInfo)
        {
            this.validator.ValidateFirstName(personalInfo.FirstName);
            this.validator.ValidateLastName(personalInfo.LastName);
            this.validator.ValidateDateOfBirth(personalInfo.DateOfBirth);
            this.validator.ValidateAge(personalInfo.Age);
            this.validator.ValidateSalary(personalInfo.Salary);
            this.validator.ValidateGender(personalInfo.Gender);
        }

        private void RemoveFromIndices(FileCabinetRecord record)
        {
            this.RemoveFromIndex(this.firstNameIndex, record.FirstName, record);
            this.RemoveFromIndex(this.lastNameIndex, record.LastName, record);
            this.RemoveFromIndex(this.dateOfBirthIndex, record.DateOfBirth.ToString("yyyy-MM-dd"), record);
        }

        private void RemoveFromIndex(Dictionary<string, List<FileCabinetRecord>> index, string key, FileCabinetRecord record)
        {
            if (index.ContainsKey(key))
            {
                index[key].Remove(record);
            }
        }

        private void AddToIndices(FileCabinetRecord record)
        {
            AddToIndex(this.firstNameIndex, record.FirstName, record);
            AddToIndex(this.lastNameIndex, record.LastName, record);
            AddToIndex(this.dateOfBirthIndex, record.DateOfBirth.ToString("yyyy-MM-dd"), record);
        }

        private static FileCabinetRecord[] FindByIndex(Dictionary<string, List<FileCabinetRecord>> index, string key)
        {
            if (index.TryGetValue(key, out var records))
            {
                return records.ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        private static void AddToIndex(Dictionary<string, List<FileCabinetRecord>> index, string key, FileCabinetRecord record)
        {
            if (!index.ContainsKey(key))
            {
                index[key] = new List<FileCabinetRecord>();
            }

            index[key].Add(record);
        }
    }
}
