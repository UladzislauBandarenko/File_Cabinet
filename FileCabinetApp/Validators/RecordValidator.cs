using FileCabinetApp.Validators;

namespace FileCabinetApp.Validators;

public class RecordValidator : IRecordValidator
{
    private readonly IValidator<string> firstNameValidator;
    private readonly IValidator<string> lastNameValidator;
    private readonly IValidator<DateTime> dateOfBirthValidator;
    private readonly IValidator<short> ageValidator;
    private readonly IValidator<decimal> salaryValidator;
    private readonly IValidator<char> genderValidator;

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

    public bool ValidateFirstName(string firstName, out string errorMessage)
    {
        return this.firstNameValidator.Validate(firstName, out errorMessage);
    }

    public bool ValidateLastName(string lastName, out string errorMessage)
    {
        return this.lastNameValidator.Validate(lastName, out errorMessage);
    }

    public bool ValidateDateOfBirth(DateTime dateOfBirth, out string errorMessage)
    {
        return this.dateOfBirthValidator.Validate(dateOfBirth, out errorMessage);
    }

    public bool ValidateAge(short age, out string errorMessage)
    {
        return this.ageValidator.Validate(age, out errorMessage);
    }

    public bool ValidateSalary(decimal salary, out string errorMessage)
    {
        return this.salaryValidator.Validate(salary, out errorMessage);
    }

    public bool ValidateGender(char gender, out string errorMessage)
    {
        return this.genderValidator.Validate(gender, out errorMessage);
    }
}