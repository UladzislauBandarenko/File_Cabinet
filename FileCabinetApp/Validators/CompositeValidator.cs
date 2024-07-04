namespace FileCabinetApp.Validators;

public class CompositeValidator<T> : IValidator<T>
{
    private readonly List<IValidator<T>> validators = new List<IValidator<T>>();

    public void AddValidator(IValidator<T> validator)
    {
        validators.Add(validator);
    }

    public bool Validate(T value, out string errorMessage)
    {
        foreach (var validator in validators)
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