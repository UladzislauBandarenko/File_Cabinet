namespace FileCabinetApp.Validators;

/// <summary>
/// Validates the value.
/// </summary>
/// <typeparam name="T">The type of the value to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the value.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>True if the value is valid, false otherwise.</returns>
    bool Validate(T value, out string errorMessage);
}