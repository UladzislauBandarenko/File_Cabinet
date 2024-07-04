using System.Xml.Serialization;
namespace FileCabinetGenerator;

public class FileCabinetRecord
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlElement("firstname")]
    public string? FirstName { get; set; }

    [XmlElement("lastname")]
    public string? LastName { get; set; }

    [XmlElement("dateofbirth")]
    public DateTime DateOfBirth { get; set; }

    [XmlElement("age")]
    public short Age { get; set; }

    [XmlElement("salary")]
    public decimal Salary { get; set; }

    [XmlElement("gender")]
    public char Gender { get; set; }
}