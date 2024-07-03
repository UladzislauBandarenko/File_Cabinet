using System.Globalization;

namespace FileCabinetApp;

/// <summary>
/// Provides services for managing file cabinet records.
/// </summary>
public class FileCabinetService
{
    private const int MinNameLength = 2;
    private const int MaxNameLength = 60;
    private const int MinAge = 0;
    private const int MaxAge = 120;
    private const decimal MinSalary = 0;
    private const decimal MaxSalary = 1000000;
    private static readonly DateTime MinDateOfBirth = new DateTime(1950, 1, 1);
    private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();
    private readonly Dictionary<string, List<FileCabinetRecord>> firstNameIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<FileCabinetRecord>> lastNameIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthIndex = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Creates a new record in the file cabinet.
    /// </summary>
    /// <param name="firstName">The first name of the person.</param>
    /// <param name="lastName">The last name of the person.</param>
    /// <param name="dateOfBirth">The date of birth of the person.</param>
    /// <param name="age">The age of the person.</param>
    /// <param name="salary">The salary of the person.</param>
    /// <param name="gender">The gender of the person ('M' or 'F').</param>
    /// <returns>The ID of the newly created record.</returns>
    /// <exception cref="ArgumentException">Thrown when any of the input parameters are invalid.</exception>
    public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short age, decimal salary, char gender)
    {
        ValidatePersonalInfo(firstName, lastName, dateOfBirth, age, salary, gender);

        var record = new FileCabinetRecord
        {
            Id = this.records.Count + 1,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Age = age,
            Salary = salary,
            Gender = gender,
        };

        this.records.Add(record);
        AddToIndex(this.firstNameIndex, firstName, record);
        AddToIndex(this.lastNameIndex, lastName, record);
        AddToIndex(this.dateOfBirthIndex, dateOfBirth.ToString("yyyy-MM-dd"), record);

        return record.Id;
    }

    /// <summary>
    /// Gets all records in the file cabinet.
    /// </summary>
    /// <returns>An array of all records.</returns>
    public FileCabinetRecord[] GetRecords()
    {
        return this.records.ToArray();
    }

    /// <summary>
    /// Gets the total number of records in the file cabinet.
    /// </summary>
    /// <returns>The total number of records.</returns>
    public int GetStat()
    {
        return this.records.Count;
    }

    /// <summary>
    /// Edits an existing record in the file cabinet.
    /// </summary>
    /// <param name="id">The ID of the record to edit.</param>
    /// <param name="firstName">The new first name.</param>
    /// <param name="lastName">The new last name.</param>
    /// <param name="dateOfBirth">The new date of birth.</param>
    /// <param name="age">The new age.</param>
    /// <param name="salary">The new salary.</param>
    /// <param name="gender">The new gender.</param>
    /// <exception cref="ArgumentException">Thrown when the record is not found or any of the input parameters are invalid.</exception>
    public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short age, decimal salary, char gender)
    {
        var record = this.records.FirstOrDefault(r => r.Id == id);
        if (record == null)
        {
            throw new ArgumentException($"Record with id {id} does not exist.", nameof(id));
        }

        ValidatePersonalInfo(firstName, lastName, dateOfBirth, age, salary, gender);

        RemoveFromIndices(record);

        record.FirstName = firstName;
        record.LastName = lastName;
        record.DateOfBirth = dateOfBirth;
        record.Age = age;
        record.Salary = salary;
        record.Gender = gender;

        AddToIndices(record);
    }

    private void RemoveFromIndices(FileCabinetRecord record)
    {
        RemoveFromIndex(this.firstNameIndex, record.FirstName, record);
        RemoveFromIndex(this.lastNameIndex, record.LastName, record);
        RemoveFromIndex(this.dateOfBirthIndex, record.DateOfBirth.ToString("yyyy-MM-dd"), record);
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

    /// <summary>
    /// Finds records by first name.
    /// </summary>
    /// <param name="firstName">The first name to search for.</param>
    /// <returns>An array of matching records.</returns>
    public FileCabinetRecord[] FindByFirstName(string firstName)
    {
        return FindByIndex(this.firstNameIndex, firstName);
    }

    /// <summary>
    /// Finds records by last name.
    /// </summary>
    /// <param name="lastName">The last name to search for.</param>
    /// <returns>An array of matching records.</returns>
    public FileCabinetRecord[] FindByLastName(string lastName)
    {
        return FindByIndex(this.lastNameIndex, lastName);
    }

    /// <summary>
    /// Finds records by date of birth.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth to search for.</param>
    /// <returns>An array of matching records.</returns>
    public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
    {
        if (DateTime.TryParse(dateOfBirth, out DateTime date))
        {
            return FindByIndex(this.dateOfBirthIndex, date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }

        return Array.Empty<FileCabinetRecord>();
    }

    private static FileCabinetRecord[] FindByIndex(Dictionary<string, List<FileCabinetRecord>> index, string key)
    {
        if (index.TryGetValue(key, out var records))
        {
            return records.ToArray();
        }

        return Array.Empty<FileCabinetRecord>();
    }

    private static void ValidatePersonalInfo(string firstName, string lastName, DateTime dateOfBirth, short age, decimal salary, char gender)
    {
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < MinNameLength || firstName.Length > MaxNameLength)
        {
            throw new ArgumentException($"First name must be between {MinNameLength} and {MaxNameLength} characters and not empty.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < MinNameLength || lastName.Length > MaxNameLength)
        {
            throw new ArgumentException($"Last name must be between {MinNameLength} and {MaxNameLength} characters and not empty.", nameof(lastName));
        }

        if (dateOfBirth < MinDateOfBirth || dateOfBirth > DateTime.Now)
        {
            throw new ArgumentException($"Date of birth must be between {MinDateOfBirth:d} and the current date.", nameof(dateOfBirth));
        }

        if (age < MinAge || age > MaxAge)
        {
            throw new ArgumentException($"Age must be between {MinAge} and {MaxAge}.", nameof(age));
        }

        if (salary < MinSalary || salary > MaxSalary)
        {
            throw new ArgumentException($"Salary must be between {MinSalary:C} and {MaxSalary:C}.", nameof(salary));
        }

        if (gender != 'M' && gender != 'F')
        {
            throw new ArgumentException("Gender must be either 'M' or 'F'.", nameof(gender));
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
}