namespace FileCabinetApp.Validators;

/// <summary>
/// Validator builder extensions.
/// </summary>
public static class ValidatorBuilderExtensions
{
    /// <summary>
    /// Adds the default first name validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>First name validator.</returns>
    public static ValidatorBuilder AddDefaultFirstNameValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddFirstNameValidator(new FirstNameValidator());
    }

    /// <summary>
    /// Adds the custom first name validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>First name validator.</returns>
    public static ValidatorBuilder AddCustomFirstNameValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddFirstNameValidator(new FirstNameValidator(2, 30));
    }

    /// <summary>
    /// Adds the default last name validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>Last name validator.</returns>
    public static ValidatorBuilder AddDefaultLastNameValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddLastNameValidator(new LastNameValidator());
    }

    /// <summary>
    /// Adds the custom last name validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>Last name validator.</returns>
    public static ValidatorBuilder AddCustomLastNameValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddLastNameValidator(new LastNameValidator(2, 30));
    }

    /// <summary>
    /// Adds the default date of birth validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>Date of birth validator.</returns>
    public static ValidatorBuilder AddDefaultDateOfBirthValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddDateOfBirthValidator(new DateOfBirthValidator());
    }

    /// <summary>
    /// Adds the custom date of birth validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>Date of birth validator.</returns>
    public static ValidatorBuilder AddCustomDateOfBirthValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddDateOfBirthValidator(new DateOfBirthValidator(DateTime.Now.AddYears(-100), DateTime.Now));
    }

    /// <summary>
    /// Adds the default age validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>Age validator.</returns>
    public static ValidatorBuilder AddDefaultAgeValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddAgeValidator(new AgeValidator());
    }

    /// <summary>
    /// Adds the custom age validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>Age validator.</returns>
    public static ValidatorBuilder AddCustomAgeValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddAgeValidator(new AgeValidator(18, 110));
    }

    /// <summary>
    /// Adds the default salary validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>Salary validator.</returns>
    public static ValidatorBuilder AddDefaultSalaryValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddSalaryValidator(new SalaryValidator());
    }

    /// <summary>
    /// Adds the custom salary validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>Salary validator.</returns>
    public static ValidatorBuilder AddCustomSalaryValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddSalaryValidator(new SalaryValidator(500, 5000000));
    }

    /// <summary>
    /// Adds the default gender validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>Gender validator.</returns>
    public static ValidatorBuilder AddDefaultGenderValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddGenderValidator(new GenderValidator());
    }

    /// <summary>
    /// Adds the custom gender validator.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>Gender validator.</returns>
    public static ValidatorBuilder AddCustomGenderValidator(this ValidatorBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddGenderValidator(new GenderValidator(new[] { 'M', 'F', 'O' }));
    }

    /// <summary>
    /// Adds the default validators.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder with default validators.</returns>
    public static ValidatorBuilder AddDefaultValidators(this ValidatorBuilder builder)
    {
        return builder
            .AddDefaultFirstNameValidator()
            .AddDefaultLastNameValidator()
            .AddDefaultDateOfBirthValidator()
            .AddDefaultAgeValidator()
            .AddDefaultSalaryValidator()
            .AddDefaultGenderValidator();
    }

    /// <summary>
    /// Adds the custom validators.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder with custom validators.</returns>
    public static ValidatorBuilder AddCustomValidators(this ValidatorBuilder builder)
    {
        return builder
            .AddCustomFirstNameValidator()
            .AddCustomLastNameValidator()
            .AddCustomDateOfBirthValidator()
            .AddCustomAgeValidator()
            .AddCustomSalaryValidator()
            .AddCustomGenderValidator();
    }
}