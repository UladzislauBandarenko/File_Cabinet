using FileCabinetApp.Validators;

namespace FileCabinetApp.Validators;

/// <summary>
/// Record validator.
/// </summary>
public class RecordValidator : IRecordValidator
{
    private readonly IValidator<string> firstNameValidator;
    private readonly IValidator<string> lastNameValidator;
    private readonly IValidator<DateTime> dateOfBirthValidator;
    private readonly IValidator<short> ageValidator;
    private readonly IValidator<decimal> salaryValidator;
    private readonly IValidator<char> genderValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordValidator"/> class.
    /// </summary>
    /// <param name="firstNameValidator">The first name validator.</param>
    /// <param name="lastNameValidator">The last name validator.</param>
    /// <param name="dateOfBirthValidator">The date of birth validator.</param>
    /// <param name="ageValidator">The age validator.</param>
    /// <param name="salaryValidator">The salary validator.</param>
    /// <param name="genderValidator">The gender validator.</param>
    public RecordValidator(
        IValidator<string> firstNameValidator,
        IValidator<string> lastNameValidator,
        IValidator<DateTime> dateOfBirthValidator,
        IValidator<short> ageValidator,
        IValidator<decimal> salaryValidator,
        IValidator<char> genderValidator)
    {
        this.firstNameValidator = firstNameValidator;
        this.lastNameValidator = lastNameValidator;
        this.dateOfBirthValidator = dateOfBirthValidator;
        this.ageValidator = ageValidator;
        this.salaryValidator = salaryValidator;
        this.genderValidator = genderValidator;
    }

    /// <inheritdoc/>
    public bool ValidateFirstName(string firstName, out string errorMessage)
    {
        return this.firstNameValidator.Validate(firstName, out errorMessage);
    }

    /// <inheritdoc/>
    public bool ValidateLastName(string lastName, out string errorMessage)
    {
        return this.lastNameValidator.Validate(lastName, out errorMessage);
    }

    /// <inheritdoc/>
    public bool ValidateDateOfBirth(DateTime dateOfBirth, out string errorMessage)
    {
        return this.dateOfBirthValidator.Validate(dateOfBirth, out errorMessage);
    }

    /// <inheritdoc/>
    public bool ValidateAge(short age, out string errorMessage)
    {
        return this.ageValidator.Validate(age, out errorMessage);
    }

    /// <inheritdoc/>
    public bool ValidateSalary(decimal salary, out string errorMessage)
    {
        return this.salaryValidator.Validate(salary, out errorMessage);
    }

    /// <inheritdoc/>
    public bool ValidateGender(char gender, out string errorMessage)
    {
        return this.genderValidator.Validate(gender, out errorMessage);
    }
}