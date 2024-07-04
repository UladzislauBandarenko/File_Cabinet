namespace FileCabinetApp.Validators;

/// <summary>
/// Composite validator.
/// </summary>
/// <typeparam name="T">The type of the value to validate.</typeparam>
public class CompositeValidator<T> : IValidator<T>
{
    private readonly List<IValidator<T>> validators = new List<IValidator<T>>();

    /// <summary>
    /// Adds a validator to the composite validator.
    /// </summary>
    /// <param name="validator">The validator to add.</param>
    public void AddValidator(IValidator<T> validator)
    {
        this.validators.Add(validator);
    }

    /// <inheritdoc/>
    public bool Validate(T value, out string errorMessage)
    {
        foreach (var validator in this.validators)
        {
            if (!validator.Validate(value, out errorMessage))
            {
                return false;
            }
        }

        errorMessage = string.Empty;
        return true;
    }
}