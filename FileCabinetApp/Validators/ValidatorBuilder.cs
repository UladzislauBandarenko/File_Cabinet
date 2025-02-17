namespace FileCabinetApp.Validators;

/// <summary>
/// Validator builder.
/// </summary>
public class ValidatorBuilder
{
    private readonly CompositeValidator<string> firstNameValidator = new CompositeValidator<string>();
    private readonly CompositeValidator<string> lastNameValidator = new CompositeValidator<string>();
    private readonly CompositeValidator<DateTime> dateOfBirthValidator = new CompositeValidator<DateTime>();
    private readonly CompositeValidator<short> ageValidator = new CompositeValidator<short>();
    private readonly CompositeValidator<decimal> salaryValidator = new CompositeValidator<decimal>();
    private readonly CompositeValidator<char> genderValidator = new CompositeValidator<char>();

    /// <summary>
    /// Adds the first name validator.
    /// </summary>
    /// <param name="validator">The validator.</param>
    /// <returns>The builder.</returns>
    public ValidatorBuilder AddFirstNameValidator(IValidator<string> validator)
    {
        this.firstNameValidator.AddValidator(validator);
        return this;
    }

    /// <summary>
    /// Adds the last name validator.
    /// </summary>
    /// <param name="validator">The validator.</param>
    /// <returns>The builder.</returns>
    public ValidatorBuilder AddLastNameValidator(IValidator<string> validator)
    {
        this.lastNameValidator.AddValidator(validator);
        return this;
    }

    /// <summary>
    /// Adds the date of birth validator.
    /// </summary>
    /// <param name="validator">The validator.</param>
    /// <returns>The builder.</returns>
    public ValidatorBuilder AddDateOfBirthValidator(IValidator<DateTime> validator)
    {
        this.dateOfBirthValidator.AddValidator(validator);
        return this;
    }

    /// <summary>
    /// Adds the age validator.
    /// </summary>
    /// <param name="validator">The validator.</param>
    /// <returns>The builder.</returns>
    public ValidatorBuilder AddAgeValidator(IValidator<short> validator)
    {
        this.ageValidator.AddValidator(validator);
        return this;
    }

    /// <summary>
    /// Adds the salary validator.
    /// </summary>
    /// <param name="validator">The validator.</param>
    /// <returns>The builder.</returns>
    public ValidatorBuilder AddSalaryValidator(IValidator<decimal> validator)
    {
        this.salaryValidator.AddValidator(validator);
        return this;
    }

    /// <summary>
    /// Adds the gender validator.
    /// </summary>
    /// <param name="validator">The validator.</param>
    /// <returns>The builder.</returns>
    public ValidatorBuilder AddGenderValidator(IValidator<char> validator)
    {
        this.genderValidator.AddValidator(validator);
        return this;
    }

    /// <summary>
    /// Adds the validators based on the configuration.
    /// </summary>
    /// <param name="config">The validation configuration.</param>
    /// <returns>The builder.</returns>
    public ValidatorBuilder AddValidators(ValidationConfig config)
    {
        if (config is null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        this.AddFirstNameValidator(new FirstNameValidator(config.FirstName.Min, config.FirstName.Max));
        this.AddLastNameValidator(new LastNameValidator(config.LastName.Min, config.LastName.Max));
        this.AddDateOfBirthValidator(new DateOfBirthValidator(config.DateOfBirth.From, config.DateOfBirth.To));
        this.AddAgeValidator(new AgeValidator((short)config.Age.Min, (short)config.Age.Max));
        this.AddSalaryValidator(new SalaryValidator(config.Salary.Min, config.Salary.Max));
        this.AddGenderValidator(new GenderValidator(config.Gender.Select(g => g[0]).ToArray()));
        return this;
    }

    /// <summary>
    /// Builds this instance.
    /// </summary>
    /// <returns>The record validator.</returns>
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