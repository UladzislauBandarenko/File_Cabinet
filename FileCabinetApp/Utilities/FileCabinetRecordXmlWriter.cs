using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet record XML writer.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public FileCabinetRecordXmlWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes the specified records.
        /// </summary>
        /// <param name="records">The records.</param>
        public void Write(ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

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
                    xmlWriter.WriteAttributeString("id", record.Id.ToString(CultureInfo.InvariantCulture));

                    xmlWriter.WriteStartElement("name");
                    xmlWriter.WriteAttributeString("first", record.FirstName);
                    xmlWriter.WriteAttributeString("last", record.LastName);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    xmlWriter.WriteElementString("age", record.Age.ToString(CultureInfo.InvariantCulture));
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