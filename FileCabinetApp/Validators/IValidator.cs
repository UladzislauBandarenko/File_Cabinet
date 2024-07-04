namespace FileCabinetApp.Validators;

public interface IValidator<T>
{
    bool Validate(T value, out string errorMessage);
}