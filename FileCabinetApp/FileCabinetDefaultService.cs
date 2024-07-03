namespace FileCabinetApp;

public class FileCabinetDefaultService : FileCabinetService
{
    protected override void ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < MinNameLength || firstName.Length > MaxNameLength)
        {
            throw new ArgumentException($"First name must be between {MinNameLength} and {MaxNameLength} characters and not empty.", nameof(firstName));
        }
    }

    protected override void ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < MinNameLength || lastName.Length > MaxNameLength)
        {
            throw new ArgumentException($"Last name must be between {MinNameLength} and {MaxNameLength} characters and not empty.", nameof(lastName));
        }
    }

    protected override void ValidateDateOfBirth(DateTime dateOfBirth)
    {
        if (dateOfBirth < MinDateOfBirth || dateOfBirth > DateTime.Now)
        {
            throw new ArgumentException($"Date of birth must be between {MinDateOfBirth:d} and the current date.", nameof(dateOfBirth));
        }
    }

    protected override void ValidateAge(short age)
    {
        if (age < MinAge || age > MaxAge)
        {
            throw new ArgumentException($"Age must be between {MinAge} and {MaxAge}.", nameof(age));
        }
    }

    protected override void ValidateSalary(decimal salary)
    {
        if (salary < MinSalary || salary > MaxSalary)
        {
            throw new ArgumentException($"Salary must be between {MinSalary:C} and {MaxSalary:C}.", nameof(salary));
        }
    }

    protected override void ValidateGender(char gender)
    {
        if (gender != 'M' && gender != 'F')
        {
            throw new ArgumentException("Gender must be either 'M' or 'F'.", nameof(gender));
        }
    }
}