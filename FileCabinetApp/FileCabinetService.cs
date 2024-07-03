using System.Globalization;

namespace FileCabinetApp;

public abstract class FileCabinetService
{
    protected const int MinNameLength = 2;
    protected const int MaxNameLength = 60;
    protected const int MinAge = 0;
    protected const int MaxAge = 120;
    protected const decimal MinSalary = 0;
    protected const decimal MaxSalary = 1000000;
    protected static readonly DateTime MinDateOfBirth = new DateTime(1950, 1, 1);
    protected readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();
    protected readonly Dictionary<string, List<FileCabinetRecord>> firstNameIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
    protected readonly Dictionary<string, List<FileCabinetRecord>> lastNameIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
    protected readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

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

    protected abstract void ValidateFirstName(string firstName);

    protected abstract void ValidateLastName(string lastName);

    protected abstract void ValidateDateOfBirth(DateTime dateOfBirth);

    protected abstract void ValidateAge(short age);

    protected abstract void ValidateSalary(decimal salary);

    protected abstract void ValidateGender(char gender);

    protected void ValidatePersonalInfo(PersonalInfo personalInfo)
    {
        this.ValidateFirstName(personalInfo.FirstName);
        this.ValidateLastName(personalInfo.LastName);
        this.ValidateDateOfBirth(personalInfo.DateOfBirth);
        this.ValidateAge(personalInfo.Age);
        this.ValidateSalary(personalInfo.Salary);
        this.ValidateGender(personalInfo.Gender);
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
