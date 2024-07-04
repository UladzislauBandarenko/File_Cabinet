namespace FileCabinetApp.Validators;

public interface IRecordValidator
{
    bool ValidateFirstName(string firstName, out string errorMessage);
    bool ValidateLastName(string lastName, out string errorMessage);
    bool ValidateDateOfBirth(DateTime dateOfBirth, out string errorMessage);
    bool ValidateAge(short age, out string errorMessage);
    bool ValidateSalary(decimal salary, out string errorMessage);
    bool ValidateGender(char gender, out string errorMessage);
}