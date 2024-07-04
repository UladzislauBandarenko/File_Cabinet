namespace FileCabinetApp.Validators;

public static class ValidatorBuilderExtensions
{
    public static ValidatorBuilder AddDefaultFirstNameValidator(this ValidatorBuilder builder)
    {
        return builder.AddFirstNameValidator(new FirstNameValidator());
    }

    public static ValidatorBuilder AddCustomFirstNameValidator(this ValidatorBuilder builder)
    {
        return builder.AddFirstNameValidator(new FirstNameValidator(2, 30));
    }

    public static ValidatorBuilder AddDefaultLastNameValidator(this ValidatorBuilder builder)
    {
        return builder.AddLastNameValidator(new LastNameValidator());
    }

    public static ValidatorBuilder AddCustomLastNameValidator(this ValidatorBuilder builder)
    {
        return builder.AddLastNameValidator(new LastNameValidator(2, 30));
    }

    public static ValidatorBuilder AddDefaultDateOfBirthValidator(this ValidatorBuilder builder)
    {
        return builder.AddDateOfBirthValidator(new DateOfBirthValidator());
    }

    public static ValidatorBuilder AddCustomDateOfBirthValidator(this ValidatorBuilder builder)
    {
        return builder.AddDateOfBirthValidator(new DateOfBirthValidator(DateTime.Now.AddYears(-100), DateTime.Now));
    }

    public static ValidatorBuilder AddDefaultAgeValidator(this ValidatorBuilder builder)
    {
        return builder.AddAgeValidator(new AgeValidator());
    }

    public static ValidatorBuilder AddCustomAgeValidator(this ValidatorBuilder builder)
    {
        return builder.AddAgeValidator(new AgeValidator(18, 110));
    }

    public static ValidatorBuilder AddDefaultSalaryValidator(this ValidatorBuilder builder)
    {
        return builder.AddSalaryValidator(new SalaryValidator());
    }

    public static ValidatorBuilder AddCustomSalaryValidator(this ValidatorBuilder builder)
    {
        return builder.AddSalaryValidator(new SalaryValidator(500, 5000000));
    }

    public static ValidatorBuilder AddDefaultGenderValidator(this ValidatorBuilder builder)
    {
        return builder.AddGenderValidator(new GenderValidator());
    }

    public static ValidatorBuilder AddCustomGenderValidator(this ValidatorBuilder builder)
    {
        return builder.AddGenderValidator(new GenderValidator(new[] { 'M', 'F', 'O' }));
    }

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