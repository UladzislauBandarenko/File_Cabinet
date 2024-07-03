using System;

namespace FileCabinetApp
{
    public class DefaultValidator : IRecordValidator
    {
        public void ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("First name must be between 2 and 60 characters and not empty.", nameof(firstName));
            }
        }

        public void ValidateLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Last name must be between 2 and 60 characters and not empty.", nameof(lastName));
            }
        }

        public void ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth must be between 01/01/1950 and the current date.", nameof(dateOfBirth));
            }
        }

        public void ValidateAge(short age)
        {
            if (age < 0 || age > 120)
            {
                throw new ArgumentException("Age must be between 0 and 120.", nameof(age));
            }
        }

        public void ValidateSalary(decimal salary)
        {
            if (salary < 0 || salary > 1000000)
            {
                throw new ArgumentException("Salary must be between $0 and $1,000,000.", nameof(salary));
            }
        }

        public void ValidateGender(char gender)
        {
            if (gender != 'M' && gender != 'F')
            {
                throw new ArgumentException("Gender must be either 'M' or 'F'.", nameof(gender));
            }
        }
    }
}