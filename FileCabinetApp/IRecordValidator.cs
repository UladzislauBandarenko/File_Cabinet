using System;

namespace FileCabinetApp
{
    public interface IRecordValidator
    {
        void ValidateFirstName(string firstName);

        void ValidateLastName(string lastName);

        void ValidateDateOfBirth(DateTime dateOfBirth);

        void ValidateAge(short age);

        void ValidateSalary(decimal salary);

        void ValidateGender(char gender);
    }
}