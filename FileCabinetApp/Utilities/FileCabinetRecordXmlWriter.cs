using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FileCabinetApp
{
    public class FileCabinetRecordXmlWriter
    {
        private readonly TextWriter writer;

        public FileCabinetRecordXmlWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Write(ReadOnlyCollection<FileCabinetRecord> records)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
            };

            using (var xmlWriter = XmlWriter.Create(this.writer, settings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("records");

                foreach (var record in records)
                {
                    xmlWriter.WriteStartElement("record");
                    xmlWriter.WriteAttributeString("id", record.Id.ToString());

                    xmlWriter.WriteStartElement("name");
                    xmlWriter.WriteAttributeString("first", record.FirstName);
                    xmlWriter.WriteAttributeString("last", record.LastName);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    xmlWriter.WriteElementString("age", record.Age.ToString());
                    xmlWriter.WriteElementString("salary", record.Salary.ToString("F2", CultureInfo.InvariantCulture));
                    xmlWriter.WriteElementString("gender", record.Gender.ToString());

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }
    }
}