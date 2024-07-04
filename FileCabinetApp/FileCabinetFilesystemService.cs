using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents a service for working with the file cabinet.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 278;
        private const short ActiveStatus = 1;
        private const short InactiveStatus = 0;
        private readonly IRecordValidator validator;
        private readonly FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="validator">The validator to use.</param>
        /// <param name="fileStream">The file stream to use.</param>
        public FileCabinetFilesystemService(IRecordValidator validator, FileStream fileStream)
        {
            this.validator = validator;
            this.fileStream = fileStream;
        }

        /// <inheritdoc/>
        public int PurgeRecords()
        {
            int purgedRecords = 0;
            long newPosition = 0;
            long fileLength = this.fileStream.Length;

            byte[] buffer = new byte[RecordSize];

            for (long position = 0; position < fileLength; position += RecordSize)
            {
                this.fileStream.Seek(position, SeekOrigin.Begin);
                this.fileStream.Read(buffer, 0, RecordSize);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    short status = reader.ReadInt16();
                    if (status == ActiveStatus)
                    {
                        if (position != newPosition)
                        {
                            this.fileStream.Seek(newPosition, SeekOrigin.Begin);
                            this.fileStream.Write(buffer, 0, RecordSize);
                        }
                        newPosition += RecordSize;
                    }
                    else
                    {
                        purgedRecords++;
                    }
                }
            }

            this.fileStream.SetLength(newPosition);
            this.fileStream.Flush();

            return purgedRecords;
        }

        /// <inheritdoc/>
        public bool RemoveRecord(int id)
        {
            long fileLength = this.fileStream.Length;
            for (long position = 0; position < fileLength; position += RecordSize)
            {
                this.fileStream.Seek(position, SeekOrigin.Begin);
                byte[] buffer = new byte[6]; // Read status (2 bytes) and id (4 bytes)
                this.fileStream.Read(buffer, 0, 6);

                short status = BitConverter.ToInt16(buffer, 0);
                int recordId = BitConverter.ToInt32(buffer, 2);

                if (status == ActiveStatus && recordId == id)
                {
                    status = InactiveStatus;
                    buffer = BitConverter.GetBytes(status);

                    this.fileStream.Seek(position, SeekOrigin.Begin);
                    this.fileStream.Write(buffer, 0, 2);
                    this.fileStream.Flush();

                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            // Clear existing records
            this.fileStream.SetLength(0);

            // Write records from the snapshot
            foreach (var record in snapshot.Records)
            {
                byte[] buffer = new byte[RecordSize];
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                using (BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    writer.Write((short)1); // Status (2 bytes)
                    writer.Write(record.Id); // Id (4 bytes)
                    writer.Write(Encoding.ASCII.GetBytes(record.FirstName.PadRight(60))); // FirstName (120 bytes)
                    writer.Write(Encoding.ASCII.GetBytes(record.LastName.PadRight(60))); // LastName (120 bytes)
                    writer.Write(record.DateOfBirth.Year); // Year (4 bytes)
                    writer.Write(record.DateOfBirth.Month); // Month (4 bytes)
                    writer.Write(record.DateOfBirth.Day); // Day (4 bytes)
                    writer.Write(record.Age); // Age (2 bytes)
                    writer.Write(record.Salary); // Salary (8 bytes)
                    writer.Write(record.Gender); // Gender (2 bytes)
                }

                this.fileStream.Write(buffer, 0, RecordSize);
            }

            this.fileStream.Flush();
        }

        /// <inheritdoc/>
        public int CreateRecord(PersonalInfo personalInfo)
        {
            this.validator.ValidateFirstName(personalInfo.FirstName);
            this.validator.ValidateLastName(personalInfo.LastName);
            this.validator.ValidateDateOfBirth(personalInfo.DateOfBirth);
            this.validator.ValidateAge(personalInfo.Age);
            this.validator.ValidateSalary(personalInfo.Salary);
            this.validator.ValidateGender(personalInfo.Gender);

            int id = (int)(this.fileStream.Length / RecordSize) + 1;

            byte[] record = new byte[RecordSize];
            using (MemoryStream memoryStream = new MemoryStream(record))
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Write(ActiveStatus); // Status (2 bytes)
                writer.Write(id); // Id (4 bytes)
                writer.Write(Encoding.ASCII.GetBytes(personalInfo.FirstName.PadRight(60))); // FirstName (120 bytes)
                writer.Write(Encoding.ASCII.GetBytes(personalInfo.LastName.PadRight(60))); // LastName (120 bytes)
                writer.Write(personalInfo.DateOfBirth.Year); // Year (4 bytes)
                writer.Write(personalInfo.DateOfBirth.Month); // Month (4 bytes)
                writer.Write(personalInfo.DateOfBirth.Day); // Day (4 bytes)
                writer.Write(personalInfo.Age); // Age (2 bytes)
                writer.Write(personalInfo.Salary); // Salary (8 bytes)
                writer.Write(personalInfo.Gender); // Gender (2 bytes)
            }

            this.fileStream.Seek(0, SeekOrigin.End);
            this.fileStream.Write(record, 0, RecordSize);
            this.fileStream.Flush();

            return id;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords(RecordPrinter printer)
        {
            var records = new List<FileCabinetRecord>();
            byte[] buffer = new byte[RecordSize];

            this.fileStream.Seek(0, SeekOrigin.Begin);

            while (this.fileStream.Read(buffer, 0, RecordSize) == RecordSize)
            {
                using (var memoryStream = new MemoryStream(buffer))
                using (var reader = new BinaryReader(memoryStream))
                {
                    short status = reader.ReadInt16();
                    if (status != ActiveStatus)
                    {
                        continue;
                    }

                    var record = new FileCabinetRecord
                    {
                        Id = reader.ReadInt32(),
                        FirstName = new string(reader.ReadChars(60)).TrimEnd('\0'),
                        LastName = new string(reader.ReadChars(60)).TrimEnd('\0'),
                        DateOfBirth = new DateTime(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), 0, 0, 0, 0, DateTimeKind.Local),
                        Age = reader.ReadInt16(),
                        Salary = reader.ReadDecimal(),
                        Gender = reader.ReadChar(),
                    };
                    record.PrintedRepresentation = printer(record);
                    records.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <inheritdoc/>
        public int GetStat()
        {
            long fileLength = this.fileStream.Length;
            return (int)(fileLength / RecordSize);
        }

        /// <inheritdoc/>
        public void EditRecord(int id, PersonalInfo personalInfo)
        {
            this.validator.ValidateFirstName(personalInfo.FirstName);
            this.validator.ValidateLastName(personalInfo.LastName);
            this.validator.ValidateDateOfBirth(personalInfo.DateOfBirth);
            this.validator.ValidateAge(personalInfo.Age);
            this.validator.ValidateSalary(personalInfo.Salary);
            this.validator.ValidateGender(personalInfo.Gender);

            long position = (id - 1) * RecordSize;
            if (position >= this.fileStream.Length)
            {
                throw new ArgumentException($"Record with id {id} does not exist.", nameof(id));
            }

            this.fileStream.Seek(position, SeekOrigin.Begin);

            byte[] buffer = new byte[RecordSize];
            this.fileStream.Read(buffer, 0, RecordSize);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                short status = reader.ReadInt16();
                if (status != ActiveStatus)
                {
                    throw new ArgumentException($"Record with id {id} does not exist or has been deleted.", nameof(id));
                }
            }

            this.fileStream.Seek(position, SeekOrigin.Begin);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Write((short)1); // Status (2 bytes)
                writer.Write(id); // Id (4 bytes)
                writer.Write(Encoding.ASCII.GetBytes(personalInfo.FirstName.PadRight(60))); // FirstName (120 bytes)
                writer.Write(Encoding.ASCII.GetBytes(personalInfo.LastName.PadRight(60))); // LastName (120 bytes)
                writer.Write(personalInfo.DateOfBirth.Year); // Year (4 bytes)
                writer.Write(personalInfo.DateOfBirth.Month); // Month (4 bytes)
                writer.Write(personalInfo.DateOfBirth.Day); // Day (4 bytes)
                writer.Write(personalInfo.Age); // Age (2 bytes)
                writer.Write(personalInfo.Salary); // Salary (8 bytes)
                writer.Write(personalInfo.Gender); // Gender (2 bytes)
            }

            this.fileStream.Write(buffer, 0, RecordSize);
            this.fileStream.Flush();
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return FindByPredicate(record => record.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));

        }

        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return FindByPredicate(record => record.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            if (DateTime.TryParse(dateOfBirth, out DateTime date))
            {
                return FindByPredicate(record => record.DateOfBirth.Date == date.Date);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
        }

        private ReadOnlyCollection<FileCabinetRecord> FindByPredicate(Func<FileCabinetRecord, bool> predicate)
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            this.fileStream.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[RecordSize];
            int bytesRead;

            while ((bytesRead = this.fileStream.Read(buffer, 0, RecordSize)) == RecordSize)
            {
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    short status = reader.ReadInt16();
                    if (status == ActiveStatus)
                    {
                        FileCabinetRecord record = new FileCabinetRecord
                        {
                            Id = reader.ReadInt32(),
                            FirstName = Encoding.ASCII.GetString(reader.ReadBytes(120)).TrimEnd('\0'),
                            LastName = Encoding.ASCII.GetString(reader.ReadBytes(120)).TrimEnd('\0'),
                            DateOfBirth = new DateTime(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                            Age = reader.ReadInt16(),
                            Salary = reader.ReadDecimal(),
                            Gender = reader.ReadChar(),
                        };

                        if (predicate(record))
                        {
                            result.Add(record);
                        }
                    }
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(result);
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}