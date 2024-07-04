namespace FileCabinetApp.Validators;

/// <summary>
/// Validates the first name.
/// </summary>
public class FirstNameValidator : IValidator<string>
{
    private readonly int minLength;
    private readonly int maxLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
    /// </summary>
    /// <param name="minLength">The minimum length of the first name.</param>
    /// <param name="maxLength">The maximum length of the first name.</param>
    public FirstNameValidator(int minLength = 2, int maxLength = 60)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
    }

    /// <inheritdoc/>
    public bool Validate(string value, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < this.minLength || value.Length > this.maxLength)
        {
            errorMessage = $"First name must be between {this.minLength} and {this.maxLength} characters and not empty.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

/// <summary>
/// Validates the last name.
/// </summary>
public class LastNameValidator : IValidator<string>
{
    private readonly int minLength;
    private readonly int maxLength;

    /// <summary>
    /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
    /// </summary>
    /// <param name="minLength">The minimum length of the last name.</param>
    /// <param name="maxLength">The maximum length of the last name.</param>
    public LastNameValidator(int minLength = 2, int maxLength = 60)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
    }

    /// <inheritdoc/>
    public bool Validate(string value, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < this.minLength || value.Length > this.maxLength)
        {
            errorMessage = $"Last name must be between {this.minLength} and {this.maxLength} characters and not empty.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

/// <summary>
/// Validates the date of birth.
/// </summary>
public class DateOfBirthValidator : IValidator<DateTime>
{
    private readonly DateTime minDate;
    private readonly DateTime maxDate;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
    /// </summary>
    /// <param name="minDate">The minimum date of birth.</param>
    /// <param name="maxDate">The maximum date of birth.</param>
    public DateOfBirthValidator(DateTime? minDate = null, DateTime? maxDate = null)
    {
        this.minDate = minDate ?? DateTime.Now.AddYears(-150);
        this.maxDate = maxDate ?? DateTime.Now;
    }

    /// <inheritdoc/>
    public bool Validate(DateTime value, out string errorMessage)
    {
        if (value < this.minDate || value > this.maxDate)
        {
            errorMessage = $"Date of birth must be between {this.minDate:yyyy-MM-dd} and {this.maxDate:yyyy-MM-dd}.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

/// <summary>
/// Validates the age.
/// </summary>
public class AgeValidator : IValidator<short>
{
    private readonly short minAge;
    private readonly short maxAge;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgeValidator"/> class.
    /// </summary>
    /// <param name="minAge">The minimum age.</param>
    /// <param name="maxAge">The maximum age.</param>
    public AgeValidator(short minAge = 0, short maxAge = 150)
    {
        this.minAge = minAge;
        this.maxAge = maxAge;
    }

    /// <inheritdoc/>
    public bool Validate(short value, out string errorMessage)
    {
        if (value < this.minAge || value > this.maxAge)
        {
            errorMessage = $"Age must be between {this.minAge} and {this.maxAge}.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

/// <summary>
/// Validates the salary.
/// </summary>
public class SalaryValidator : IValidator<decimal>
{
    private readonly decimal minSalary;
    private readonly decimal maxSalary;

    /// <summary>
    /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
    /// </summary>
    /// <param name="minSalary">The minimum salary.</param>
    /// <param name="maxSalary">The maximum salary.</param>
    public SalaryValidator(decimal minSalary = 0, decimal maxSalary = decimal.MaxValue)
    {
        this.minSalary = minSalary;
        this.maxSalary = maxSalary;
    }

    /// <inheritdoc/>
    public bool Validate(decimal value, out string errorMessage)
    {
        if (value < this.minSalary || value > this.maxSalary)
        {
            errorMessage = $"Salary must be between {this.minSalary:C2} and {this.maxSalary:C2}.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

/// <summary>
/// Validates the gender.
/// </summary>
public class GenderValidator : IValidator<char>
{
    private readonly HashSet<char> validGenders;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenderValidator"/> class.
    /// </summary>
    /// <param name="validGenders">The valid genders.</param>
    public GenderValidator(params char[] validGenders)
    {
        if (validGenders is null)
        {
            throw new ArgumentNullException(nameof(validGenders));
        }

        this.validGenders = new HashSet<char>(validGenders);
    }

    /// <inheritdoc/>
    public bool Validate(char value, out string errorMessage)
    {
        if (!this.validGenders.Contains(char.ToUpperInvariant(value)))
        {
            errorMessage = $"Gender must be one of the following: {string.Join(", ", this.validGenders)}.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}