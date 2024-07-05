using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet service snapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly ReadOnlyCollection<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">The records.</param>
        public FileCabinetServiceSnapshot(ReadOnlyCollection<FileCabinetRecord> records)
        {
            this.records = records;
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <value>
        /// The records.
        /// </value>
        public ReadOnlyCollection<FileCabinetRecord> Records => this.records;
    }
}