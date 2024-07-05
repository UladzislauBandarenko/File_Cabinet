using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using FileCabinetApp.Validators;

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
        private readonly Dictionary<string, List<long>> firstNameIndex = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<long>> lastNameIndex = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<long>> dateOfBirthIndex = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="validatorBuilder">The validator builder to use.</param>
        /// <param name="fileStream">The file stream to use.</param>
        public FileCabinetFilesystemService(ValidatorBuilder validatorBuilder, FileStream fileStream)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder));
            }

            this.validator = validatorBuilder.Build();
            this.fileStream = fileStream;
            this.InitializeIndices();
        }

        /// <inheritdoc/>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new FileCabinetRecordEnumerator(this);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        public bool RecordExists(int id)
        {
            long fileLength = this.fileStream.Length;
            byte[] buffer = new byte[RecordSize];

            for (long position = 0; position < fileLength; position += RecordSize)
            {
                this.fileStream.Seek(position, SeekOrigin.Begin);
                this.fileStream.Read(buffer, 0, RecordSize);

                using (var memoryStream = new MemoryStream(buffer))
                using (var reader = new BinaryReader(memoryStream))
                {
                    short status = reader.ReadInt16();
                    int recordId = reader.ReadInt32();

                    if (status == ActiveStatus && recordId == id)
                    {
                        return true;
                    }
                }
            }

            return false;
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

                using MemoryStream memoryStream = new MemoryStream(buffer);
                using BinaryReader reader = new BinaryReader(memoryStream);
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
                if (record.FirstName is null || record.LastName is null)
                {
                    throw new ArgumentNullException(nameof(snapshot));
                }

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
                    writer.Write((short)record.Gender); // Gender (2 bytes)
                }

                this.fileStream.Write(buffer, 0, RecordSize);
            }

            this.fileStream.Flush();
        }

        /// <inheritdoc/>
        public int CreateRecord(PersonalInfo personalInfo)
        {
            if (personalInfo is null)
            {
                throw new ArgumentNullException(nameof(personalInfo));
            }

            this.ValidatePersonalInfo(personalInfo);

            int nextId = 1;
            long fileLength = this.fileStream.Length;
            byte[] buffer = new byte[6]; // Read status (2 bytes) and id (4 bytes)

            for (long position = 0; position < fileLength; position += RecordSize)
            {
                this.fileStream.Seek(position, SeekOrigin.Begin);
                this.fileStream.Read(buffer, 0, 6);

                short status = BitConverter.ToInt16(buffer, 0);
                int recordId = BitConverter.ToInt32(buffer, 2);

                if (status == ActiveStatus && recordId >= nextId)
                {
                    nextId = recordId + 1;
                }
            }

            byte[] record = new byte[RecordSize];
            using (MemoryStream memoryStream = new MemoryStream(record))
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Write(ActiveStatus); // Status (2 bytes)
                writer.Write(nextId); // Id (4 bytes)
                writer.Write(Encoding.ASCII.GetBytes(personalInfo.FirstName.PadRight(60))); // FirstName (120 bytes)
                writer.Write(Encoding.ASCII.GetBytes(personalInfo.LastName.PadRight(60))); // LastName (120 bytes)
                writer.Write(personalInfo.DateOfBirth.Year); // Year (4 bytes)
                writer.Write(personalInfo.DateOfBirth.Month); // Month (4 bytes)
                writer.Write(personalInfo.DateOfBirth.Day); // Day (4 bytes)
                writer.Write(personalInfo.Age); // Age (2 bytes)
                writer.Write(personalInfo.Salary); // Salary (8 bytes)
                writer.Write((short)personalInfo.Gender); // Gender (2 bytes)
            }

            long recordPosition = this.fileStream.Position;
            this.fileStream.Write(record, 0, RecordSize);
            this.fileStream.Flush();

            AddToIndex(this.firstNameIndex, personalInfo.FirstName, recordPosition);
            AddToIndex(this.lastNameIndex, personalInfo.LastName, recordPosition);
            AddToIndex(this.dateOfBirthIndex, personalInfo.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), recordPosition);

            return nextId;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords(RecordPrinter printer)
        {
            if (printer is null)
            {
                throw new ArgumentNullException(nameof(printer));
            }

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
            if (personalInfo is null)
            {
                throw new ArgumentNullException(nameof(personalInfo));
            }

            this.ValidatePersonalInfo(personalInfo);

            long fileLength = this.fileStream.Length;
            byte[] buffer = new byte[RecordSize];

            for (long position = 0; position < fileLength; position += RecordSize)
            {
                this.fileStream.Seek(position, SeekOrigin.Begin);
                this.fileStream.Read(buffer, 0, RecordSize);

                using (var memoryStream = new MemoryStream(buffer))
                using (var reader = new BinaryReader(memoryStream))
                {
                    short status = reader.ReadInt16();
                    int recordId = reader.ReadInt32();

                    if (status == ActiveStatus && recordId == id)
                    {
                        string oldFirstName = new string(reader.ReadChars(60)).TrimEnd('\0');
                        string oldLastName = new string(reader.ReadChars(60)).TrimEnd('\0');
                        DateTime oldDateOfBirth = new DateTime(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), 0, 0, 0, 0, DateTimeKind.Local);

                        this.fileStream.Seek(position, SeekOrigin.Begin);
                        using (var writer = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
                        {
                            writer.Write(ActiveStatus);
                            writer.Write(id);
                            writer.Write(Encoding.ASCII.GetBytes(personalInfo.FirstName.PadRight(60)));
                            writer.Write(Encoding.ASCII.GetBytes(personalInfo.LastName.PadRight(60)));
                            writer.Write(personalInfo.DateOfBirth.Year);
                            writer.Write(personalInfo.DateOfBirth.Month);
                            writer.Write(personalInfo.DateOfBirth.Day);
                            writer.Write(personalInfo.Age);
                            writer.Write(personalInfo.Salary);
                            writer.Write((short)personalInfo.Gender);
                        }

                        RemoveFromIndex(this.firstNameIndex, oldFirstName, position);
                        RemoveFromIndex(this.lastNameIndex, oldLastName, position);
                        RemoveFromIndex(this.dateOfBirthIndex, oldDateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), position);

                        AddToIndex(this.firstNameIndex, personalInfo.FirstName, position);
                        AddToIndex(this.lastNameIndex, personalInfo.LastName, position);
                        AddToIndex(this.dateOfBirthIndex, personalInfo.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), position);

                        this.fileStream.Flush();
                        return;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            Console.WriteLine($"Searching for firstName: '{firstName}'");
            if (this.firstNameIndex.TryGetValue(firstName, out var positions))
            {
                Console.WriteLine($"Found {positions.Count} positions in index");
                foreach (var position in positions)
                {
                    var record = this.ReadRecordAtPosition(position);
                    if (record != null)
                    {
                        yield return record;
                    }
                }
            }

            Console.WriteLine("No positions found in index");
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            Console.WriteLine($"Searching for lastName: '{lastName}'");
            if (this.lastNameIndex.TryGetValue(lastName, out var positions))
            {
                Console.WriteLine($"Found {positions.Count} positions in index");
                foreach (var position in positions)
                {
                    var record = this.ReadRecordAtPosition(position);
                    if (record != null)
                    {
                        yield return record;
                    }
                }
            }

            Console.WriteLine("No positions found in index");
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            Console.WriteLine($"Searching for dateOfBirth: '{dateOfBirth}'");
            if (this.dateOfBirthIndex.TryGetValue(dateOfBirth, out var positions))
            {
                Console.WriteLine($"Found {positions.Count} positions in index");
                foreach (var position in positions)
                {
                    var record = this.ReadRecordAtPosition(position);
                    if (record != null)
                    {
                        yield return record;
                    }
                }
            }

            Console.WriteLine("No positions found in index");
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords(DefaultRecordPrinter));
        }

        private static string DefaultRecordPrinter(FileCabinetRecord record)
        {
            return $"#{record.Id}, {record.FirstName?.TrimEnd()}, {record.LastName?.TrimEnd()}, {record.DateOfBirth:yyyy-MMM-dd}, {record.Age}, {record.Salary:C2}, {record.Gender}";
        }

        private static void AddToIndex(Dictionary<string, List<long>> index, string key, long position)
        {
            string trimmedKey = key.Trim();
            if (!index.TryGetValue(trimmedKey, out var positions))
            {
                positions = new List<long>();
                index[trimmedKey] = positions;
            }

            positions.Add(position);
        }

        private static void RemoveFromIndex(Dictionary<string, List<long>> index, string key, long position)
        {
            if (index.TryGetValue(key, out var positions))
            {
                positions.Remove(position);
                if (positions.Count == 0)
                {
                    index.Remove(key);
                }
            }
        }

        private void ValidatePersonalInfo(PersonalInfo personalInfo)
        {
            if (!this.validator.ValidateFirstName(personalInfo.FirstName, out string errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }

            if (!this.validator.ValidateLastName(personalInfo.LastName, out errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }

            if (!this.validator.ValidateDateOfBirth(personalInfo.DateOfBirth, out errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }

            if (!this.validator.ValidateAge(personalInfo.Age, out errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }

            if (!this.validator.ValidateSalary(personalInfo.Salary, out errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }

            if (!this.validator.ValidateGender(personalInfo.Gender, out errorMessage))
            {
                throw new ArgumentException(errorMessage, nameof(personalInfo));
            }
        }

        private FileCabinetRecord? ReadRecordAtPosition(long position)
        {
            this.fileStream.Seek(position, SeekOrigin.Begin);
            byte[] buffer = new byte[RecordSize];
            this.fileStream.Read(buffer, 0, RecordSize);

            using var memoryStream = new MemoryStream(buffer);
            using var reader = new BinaryReader(memoryStream);

            short status = reader.ReadInt16();
            if (status != ActiveStatus)
            {
                return null;
            }

            return new FileCabinetRecord
            {
                Id = reader.ReadInt32(),
                FirstName = new string(reader.ReadChars(60)).TrimEnd('\0'),
                LastName = new string(reader.ReadChars(60)).TrimEnd('\0'),
                DateOfBirth = new DateTime(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), 0, 0, 0, 0, DateTimeKind.Local),
                Age = reader.ReadInt16(),
                Salary = reader.ReadDecimal(),
                Gender = reader.ReadChar(),
            };
        }

        private void InitializeIndices()
        {
            Console.WriteLine("Initializing indices...");
            long position = 0;
            while (position < this.fileStream.Length)
            {
                var record = this.ReadRecordAtPosition(position) ?? throw new InvalidDataException();
                if (record.FirstName == null || record.LastName == null)
                {
                    continue;
                }

                AddToIndex(this.firstNameIndex, record.FirstName.Trim(), position);
                AddToIndex(this.lastNameIndex, record.LastName.Trim(), position);
                AddToIndex(this.dateOfBirthIndex, record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), position);

                position += RecordSize;
            }

            Console.WriteLine("Indices initialized");
        }
    }
}
