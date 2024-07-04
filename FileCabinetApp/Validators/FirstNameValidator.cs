namespace FileCabinetApp.Validators;

public class FirstNameValidator : IValidator<string>
{
    private readonly int minLength;
    private readonly int maxLength;

    public FirstNameValidator(int minLength = 2, int maxLength = 60)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
    }

    public bool Validate(string value, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < minLength || value.Length > maxLength)
        {
            errorMessage = $"First name must be between {minLength} and {maxLength} characters and not empty.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

public class LastNameValidator : IValidator<string>
{
    private readonly int minLength;
    private readonly int maxLength;

    public LastNameValidator(int minLength = 2, int maxLength = 60)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
    }

    public bool Validate(string value, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < minLength || value.Length > maxLength)
        {
            errorMessage = $"Last name must be between {minLength} and {maxLength} characters and not empty.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

public class DateOfBirthValidator : IValidator<DateTime>
{
    private readonly DateTime minDate;
    private readonly DateTime maxDate;

    public DateOfBirthValidator(DateTime? minDate = null, DateTime? maxDate = null)
    {
        this.minDate = minDate ?? DateTime.Now.AddYears(-150);
        this.maxDate = maxDate ?? DateTime.Now;
    }

    public bool Validate(DateTime value, out string errorMessage)
    {
        if (value < minDate || value > maxDate)
        {
            errorMessage = $"Date of birth must be between {minDate:yyyy-MM-dd} and {maxDate:yyyy-MM-dd}.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

public class AgeValidator : IValidator<short>
{
    private readonly short minAge;
    private readonly short maxAge;

    public AgeValidator(short minAge = 0, short maxAge = 150)
    {
        this.minAge = minAge;
        this.maxAge = maxAge;
    }

    public bool Validate(short value, out string errorMessage)
    {
        if (value < minAge || value > maxAge)
        {
            errorMessage = $"Age must be between {minAge} and {maxAge}.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

public class SalaryValidator : IValidator<decimal>
{
    private readonly decimal minSalary;
    private readonly decimal maxSalary;

    public SalaryValidator(decimal minSalary = 0, decimal maxSalary = decimal.MaxValue)
    {
        this.minSalary = minSalary;
        this.maxSalary = maxSalary;
    }

    public bool Validate(decimal value, out string errorMessage)
    {
        if (value < minSalary || value > maxSalary)
        {
            errorMessage = $"Salary must be between {minSalary:C2} and {maxSalary:C2}.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

public class GenderValidator : IValidator<char>
{
    private readonly HashSet<char> validGenders;

    public GenderValidator(params char[] validGenders)
    {
        this.validGenders = new HashSet<char>(validGenders.Length == 0 ? new[] { 'M', 'F' } : validGenders);
    }

    public bool Validate(char value, out string errorMessage)
    {
        if (!validGenders.Contains(char.ToUpper(value)))
        {
            errorMessage = $"Gender must be one of the following: {string.Join(", ", validGenders)}.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}