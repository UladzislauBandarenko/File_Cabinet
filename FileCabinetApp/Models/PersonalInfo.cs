namespace FileCabinetApp;

/// <summary>
/// Represents personal information about a person.
/// </summary>
public class PersonalInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalInfo"/> class.
    /// </summary>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    /// <param name="dateOfBirth">The date of birth.</param>
    /// <param name="age">The age.</param>
    /// <param name="salary">The salary.</param>
    /// <param name="gender">The gender.</param>
    public PersonalInfo(string firstName, string lastName, DateTime dateOfBirth, short age, decimal salary, char gender)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.DateOfBirth = dateOfBirth;
        this.Age = age;
        this.Salary = salary;
        this.Gender = gender;
    }

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    /// <value>
    /// The first name.
    /// </value>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    /// <value>
    /// The last name.
    /// </value>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the date of birth.
    /// </summary>
    /// <value>
    /// The date of birth.
    /// </value>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the age.
    /// </summary>
    /// <value>
    /// The age.
    /// </value>
    public short Age { get; set; }

    /// <summary>
    /// Gets or sets the salary.
    /// </summary>
    /// <value>
    /// The salary.
    /// </value>
    public decimal Salary { get; set; }

    /// <summary>
    /// Gets or sets the gender.
    /// </summary>
    /// <value>
    /// The gender.
    /// </value>
    public char Gender { get; set; }
}