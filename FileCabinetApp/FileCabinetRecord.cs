namespace FileCabinetApp;

using System.Xml.Serialization;

/// <summary>
/// Represents a record in the file cabinet.
/// </summary>
public class FileCabinetRecord
{
    /// <summary>
    /// Gets or sets the id of the record.
    /// </summary>
    /// <value>The id of the record.</value>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the first name of the record.
    /// </summary>
    /// <value>The first name of the record.</value>
    [XmlElement("firstname")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the record.
    /// </summary>
    /// <value>The last name of the record.</value>
    [XmlElement("lastname")]
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the date of birth of the record.
    /// </summary>
    /// <value>The date of birth of the record.</value>
    [XmlElement("dateofbirth")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the age of the record.
    /// </summary>
    /// <value>The age of the record.</value>
    [XmlElement("age")]
    public short Age { get; set; }

    /// <summary>
    /// Gets or sets the salary of the record.
    /// </summary>
    /// <value>The salary of the record.</value>
    [XmlElement("salary")]
    public decimal Salary { get; set; }

    /// <summary>
    /// Gets or sets the gender of the record.
    /// </summary>
    /// <value>The gender of the record.</value>
    [XmlElement("gender")]
    public char Gender { get; set; }
}