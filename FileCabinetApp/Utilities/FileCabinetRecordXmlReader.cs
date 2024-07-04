using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a reader for XML files.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly TextReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        public FileCabinetRecordXmlReader(TextReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads all records from the reader.
        /// </summary>
        /// <returns>The snapshot of the records.</returns>
        public FileCabinetServiceSnapshot ReadAll()
        {
            var serializer = new XmlSerializer(typeof(List<FileCabinetRecord>), new XmlRootAttribute("records"));
            var records = (List<FileCabinetRecord>)serializer.Deserialize(this.reader);
            return new FileCabinetServiceSnapshot(new ReadOnlyCollection<FileCabinetRecord>(records));
        }
    }
}