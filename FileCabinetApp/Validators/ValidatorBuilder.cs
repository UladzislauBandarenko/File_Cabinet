namespace FileCabinetApp.Validators;
public class ValidatorBuilder
{
    private readonly CompositeValidator<string> firstNameValidator = new CompositeValidator<string>();
    private readonly CompositeValidator<string> lastNameValidator = new CompositeValidator<string>();
    private readonly CompositeValidator<DateTime> dateOfBirthValidator = new CompositeValidator<DateTime>();
    private readonly CompositeValidator<short> ageValidator = new CompositeValidator<short>();
    private readonly CompositeValidator<decimal> salaryValidator = new CompositeValidator<decimal>();
    private readonly CompositeValidator<char> genderValidator = new CompositeValidator<char>();

    public ValidatorBuilder AddFirstNameValidator(IValidator<string> validator)
    {
        this.firstNameValidator.AddValidator(validator);
        return this;
    }

    public ValidatorBuilder AddLastNameValidator(IValidator<string> validator)
    {
        this.lastNameValidator.AddValidator(validator);
        return this;
    }

    public ValidatorBuilder AddDateOfBirthValidator(IValidator<DateTime> validator)
    {
        this.dateOfBirthValidator.AddValidator(validator);
        return this;
    }

    public ValidatorBuilder AddAgeValidator(IValidator<short> validator)
    {
        this.ageValidator.AddValidator(validator);
        return this;
    }

    public ValidatorBuilder AddSalaryValidator(IValidator<decimal> validator)
    {
        this.salaryValidator.AddValidator(validator);
        return this;
    }

    public ValidatorBuilder AddGenderValidator(IValidator<char> validator)
    {
        this.genderValidator.AddValidator(validator);
        return this;
    }

    public IRecordValidator Build()
    {
        return new RecordValidator(
            this.firstNameValidator,
            this.lastNameValidator,
            this.dateOfBirthValidator,
            this.ageValidator,
            this.salaryValidator,
            this.genderValidator);
    }
}