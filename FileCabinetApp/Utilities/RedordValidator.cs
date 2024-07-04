using System.Globalization;

namespace FileCabinetApp.Utilities;

/// <summary>
/// Record validator.
/// </summary>
public static class RecordValidator
{
    /// <summary>
    /// Validates the first name.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The result of the validation.</returns>
    public static Tuple<bool, string> ValidateFirstName(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 2 || input.Length > 60)
        {
            return new Tuple<bool, string>(false, "First name must be between 2 and 60 characters and not empty.");
        }

        return new Tuple<bool, string>(true, string.Empty);
    }

    /// <summary>
    /// Validates the last name.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The result of the validation.</returns>
    public static Tuple<bool, string> ValidateLastName(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 2 || input.Length > 60)
        {
            return new Tuple<bool, string>(false, "Last name must be between 2 and 60 characters and not empty.");
        }

        return new Tuple<bool, string>(true, string.Empty);
    }

    /// <summary>
    /// Validates the date of birth.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The result of the validation.</returns>
    public static Tuple<bool, string> ValidateDateOfBirth(string input)
    {
        if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) && date >= new DateTime(1950, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) && date <= DateTime.Now)
        {
            return new Tuple<bool, string>(true, string.Empty);
        }

        return new Tuple<bool, string>(false, "Date of birth must be between 01/01/1950 and the current date.");
    }

    /// <summary>
    /// Validates the age.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The result of the validation.</returns>
    public static Tuple<bool, string> ValidateAge(string input)
    {
        if (short.TryParse(input, out var age) && age >= 0 && age <= 120)
        {
            return new Tuple<bool, string>(true, string.Empty);
        }

        return new Tuple<bool, string>(false, "Age must be between 0 and 120.");
    }

    /// <summary>
    /// Validates the salary.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The result of the validation.</returns>
    public static Tuple<bool, string> ValidateSalary(string input)
    {
        if (decimal.TryParse(input, out var salary) && salary >= 0 && salary <= 1000000)
        {
            return new Tuple<bool, string>(true, string.Empty);
        }

        return new Tuple<bool, string>(false, "Salary must be between $0 and $1,000,000.");
    }

    /// <summary>
    /// Validates the gender.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The result of the validation.</returns>
    public static Tuple<bool, string> ValidateGender(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new Tuple<bool, string>(false, "Gender must be either 'M' or 'F'.");
        }

        if (input.Length == 1 && (input[0] == 'M' || input[0] == 'F'))
        {
            return new Tuple<bool, string>(true, string.Empty);
        }

        return new Tuple<bool, string>(false, "Gender must be either 'M' or 'F'.");
    }
}