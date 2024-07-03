namespace FileCabinetApp;

public class FileCabinetCustomService : FileCabinetService
{
    protected override void ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 30)
        {
            throw new ArgumentException("First name must be between 2 and 30 characters and not empty.", nameof(firstName));
        }
    }

    protected override void ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 30)
        {
            throw new ArgumentException("Last name must be between 2 and 30 characters and not empty.", nameof(lastName));
        }
    }

    protected override void ValidateDateOfBirth(DateTime dateOfBirth)
    {
        if (dateOfBirth < new DateTime(1930, 1, 1) || dateOfBirth > DateTime.Now)
        {
            throw new ArgumentException("Date of birth must be between 01/01/1930 and the current date.", nameof(dateOfBirth));
        }
    }

    protected override void ValidateAge(short age)
    {
        if (age < 18 || age > 110)
        {
            throw new ArgumentException("Age must be between 18 and 110.", nameof(age));
        }
    }

    protected override void ValidateSalary(decimal salary)
    {
        if (salary < 500 || salary > 5000000)
        {
            throw new ArgumentException("Salary must be between $500 and $5,000,000.", nameof(salary));
        }
    }

    protected override void ValidateGender(char gender)
    {
        if (gender != 'M' && gender != 'F' && gender != 'O')
        {
            throw new ArgumentException("Gender must be either 'M', 'F', or 'O'.", nameof(gender));
        }
    }
}