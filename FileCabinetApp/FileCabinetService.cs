namespace FileCabinetApp;

public class FileCabinetService
{
    private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

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
}