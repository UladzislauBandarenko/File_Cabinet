namespace FileCabinetApp.Validators;

/// <summary>
/// Validation configuration.
/// </summary>
public class ValidationConfig
{
    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    /// <value>
    /// The first name.
    /// </value>
    public FieldConfig FirstName { get; set; } = new FieldConfig();

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    /// <value>
    /// The last name.
    /// </value>
    public FieldConfig LastName { get; set; } = new FieldConfig();

    /// <summary>
    /// Gets or sets the date of birth.
    /// </summary>
    /// <value>
    /// The date of birth.
    /// </value>
    public DateConfig DateOfBirth { get; set; } = new DateConfig();

    /// <summary>
    /// Gets or sets the age.
    /// </summary>
    /// <value>
    /// The age.
    /// </value>
    public FieldConfig Age { get; set; } = new FieldConfig();

    /// <summary>
    /// Gets or sets the salary.
    /// </summary>
    /// <value>
    /// The salary.
    /// </value>
    public FieldConfig Salary { get; set; } = new FieldConfig();

    /// <summary>
    /// Gets or sets the gender.
    /// </summary>
    /// <value>
    /// The gender.
    /// </value>
    public string[] Gender { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Field configuration.
/// </summary>
public class FieldConfig
{
    /// <summary>
    /// Gets or sets the minimum.
    /// </summary>
    /// <value>
    /// The minimum.
    /// </value>
    public int Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum.
    /// </summary>
    /// <value>
    /// The maximum.
    /// </value>
    public int Max { get; set; }
}

/// <summary>
/// Date configuration.
/// </summary>
public class DateConfig
{
    /// <summary>
    /// Gets or sets from.
    /// </summary>
    /// <value>
    /// From.
    /// </value>
    public DateTime From { get; set; }

    /// <summary>
    /// Gets or sets to.
    /// </summary>
    /// <value>
    /// To.
    /// </value>
    public DateTime To { get; set; }
}