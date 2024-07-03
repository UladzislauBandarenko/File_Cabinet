using System.Globalization;

namespace FileCabinetApp;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
    private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

    public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short age, decimal salary, char gender)
    {
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
        {
            throw new ArgumentException("First name must be between 2 and 60 characters and not empty.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
        {
            throw new ArgumentException("Last name must be between 2 and 60 characters and not empty.", nameof(lastName));
        }

        if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Now)
        {
            throw new ArgumentException("Date of birth must be between 01-Jan-1950 and the current date.", nameof(dateOfBirth));
        }

        if (age < 0 || age > 120)
        {
            throw new ArgumentException("Age must be between 0 and 120.", nameof(age));
        }

        if (salary < 0 || salary > 1000000)
        {
            throw new ArgumentException("Salary must be between 0 and 1,000,000.", nameof(salary));
        }

        if (gender != 'M' && gender != 'F')
        {
            throw new ArgumentException("Gender must be either 'M' or 'F'.", nameof(gender));
        }

        var record = new FileCabinetRecord
        {
            Id = this.list.Count + 1,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Age = age,
            Salary = salary,
            Gender = gender,
        };

        this.list.Add(record);

        if (!this.firstNameDictionary.ContainsKey(firstName))
        {
            this.firstNameDictionary[firstName] = new List<FileCabinetRecord>();
        }

        this.firstNameDictionary[firstName].Add(record);

        if (!this.lastNameDictionary.ContainsKey(lastName))
        {
            this.lastNameDictionary[lastName] = new List<FileCabinetRecord>();
        }

        this.lastNameDictionary[lastName].Add(record);

        if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth.ToString("yyyy-MM-dd")))
        {
            this.dateOfBirthDictionary[dateOfBirth.ToString("yyyy-MM-dd")] = new List<FileCabinetRecord>();
        }

        this.dateOfBirthDictionary[dateOfBirth.ToString("yyyy-MM-dd")].Add(record);

        return record.Id;
    }

    public FileCabinetRecord[] GetRecords()
    {
        return Array.Empty<FileCabinetRecord>();
    }

    public int GetStat()
    {
        return this.list.Count;
    }

    public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short age, decimal salary, char gender)
    {
        var record = this.list.FirstOrDefault(r => r.Id == id);
        if (record == null)
        {
            throw new ArgumentException($"Record with id {id} does not exist.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
        {
            throw new ArgumentException("First name must be between 2 and 60 characters and not empty.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
        {
            throw new ArgumentException("Last name must be between 2 and 60 characters and not empty.", nameof(lastName));
        }

        if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Now)
        {
            throw new ArgumentException("Date of birth must be between 01-Jan-1950 and the current date.", nameof(dateOfBirth));
        }

        if (age < 0 || age > 120)
        {
            throw new ArgumentException("Age must be between 0 and 120.", nameof(age));
        }

        if (salary < 0 || salary > 1000000)
        {
            throw new ArgumentException("Salary must be between 0 and 1,000,000.", nameof(salary));
        }

        if (gender != 'M' && gender != 'F')
        {
            throw new ArgumentException("Gender must be either 'M' or 'F'.", nameof(gender));
        }

        // Remove from old firstName dictionary entry
        if (this.firstNameDictionary.ContainsKey(record.FirstName))
        {
            this.firstNameDictionary[record.FirstName].Remove(record);
        }

        // Remove from old lastName dictionary entry
        if (this.lastNameDictionary.ContainsKey(record.LastName))
        {
            this.lastNameDictionary[record.LastName].Remove(record);
        }

        // Remove from old dateOfBirth dictionary entry
        if (this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth.ToString("yyyy-MM-dd")))
        {
            this.dateOfBirthDictionary[record.DateOfBirth.ToString("yyyy-MM-dd")].Remove(record);
        }

        record.FirstName = firstName;
        record.LastName = lastName;
        record.DateOfBirth = dateOfBirth;
        record.Age = age;
        record.Salary = salary;
        record.Gender = gender;

        // Add to new firstName dictionary entry
        if (!this.firstNameDictionary.ContainsKey(firstName))
        {
            this.firstNameDictionary[firstName] = new List<FileCabinetRecord>();
        }

        this.firstNameDictionary[firstName].Add(record);

        // Add to new lastName dictionary entry
        if (!this.lastNameDictionary.ContainsKey(lastName))
        {
            this.lastNameDictionary[lastName] = new List<FileCabinetRecord>();
        }

        this.lastNameDictionary[lastName].Add(record);

        // Add to new dateOfBirth dictionary entry
        if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth.ToString("yyyy-MM-dd")))
        {
            this.dateOfBirthDictionary[dateOfBirth.ToString("yyyy-MM-dd")] = new List<FileCabinetRecord>();
        }

        this.dateOfBirthDictionary[dateOfBirth.ToString("yyyy-MM-dd")].Add(record);
    }

    public FileCabinetRecord[] FindByFirstName(string firstName)
    {
        if (this.firstNameDictionary.TryGetValue(firstName, out var records))
        {
            return records.ToArray();
        }

        return Array.Empty<FileCabinetRecord>();
    }

    public FileCabinetRecord[] FindByLastName(string lastName)
    {
        if (this.lastNameDictionary.TryGetValue(lastName, out var records))
        {
            return records.ToArray();
        }

        return Array.Empty<FileCabinetRecord>();
    }

    public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
    {
        if (DateTime.TryParse(dateOfBirth, out DateTime date))
        {
            if (this.dateOfBirthDictionary.TryGetValue(date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), out var records))
            {
                return records.ToArray();
            }
        }

        return Array.Empty<FileCabinetRecord>();
    }
}