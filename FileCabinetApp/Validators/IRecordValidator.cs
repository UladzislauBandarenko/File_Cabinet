namespace FileCabinetApp.Validators;

/// <summary>
/// Validates the record.
/// </summary>
public interface IRecordValidator
{
    /// <summary>
    /// Validates the first name.
    /// </summary>
    /// <param name="firstName">The first name.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>True if the first name is valid, false otherwise.</returns>
    bool ValidateFirstName(string firstName, out string errorMessage);

    /// <summary>
    /// Validates the last name.
    /// </summary>
    /// <param name="lastName">The last name.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>True if the last name is valid, false otherwise.</returns>
    bool ValidateLastName(string lastName, out string errorMessage);

    /// <summary>
    /// Validates the date of birth.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>True if the date of birth is valid, false otherwise.</returns>
    bool ValidateDateOfBirth(DateTime dateOfBirth, out string errorMessage);

    /// <summary>
    /// Validates the age.
    /// </summary>
    /// <param name="age">The age.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>True if the age is valid, false otherwise.</returns>
    bool ValidateAge(short age, out string errorMessage);

    /// <summary>
    /// Validates the salary.
    /// </summary>
    /// <param name="salary">The salary.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>True if the salary is valid, false otherwise.</returns>
    bool ValidateSalary(decimal salary, out string errorMessage);

    /// <summary>
    /// Validates the gender.
    /// </summary>
    /// <param name="gender">The gender.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>True if the gender is valid, false otherwise.</returns>
    bool ValidateGender(char gender, out string errorMessage);
}