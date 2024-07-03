using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    public class FileCabinetServiceSnapshot
    {
        private readonly ReadOnlyCollection<FileCabinetRecord> records;

        public FileCabinetServiceSnapshot(ReadOnlyCollection<FileCabinetRecord> records)
        {
            this.records = records;
        }

        public ReadOnlyCollection<FileCabinetRecord> Records => this.records;
    }
}