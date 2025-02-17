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
                    writer.Write(record.Gender); // Gender (2 bytes)
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
                writer.Write(personalInfo.Gender); // Gender (2 bytes)
            }

            this.fileStream.Seek(0, SeekOrigin.End);
            this.fileStream.Write(record, 0, RecordSize);
            this.fileStream.Flush();

            return nextId;
        }

        /// <inheritdoc/>
        public int InsertRecord(int id, PersonalInfo personalInfo)
        {
            if (personalInfo is null)
            {
                throw new ArgumentNullException(nameof(personalInfo));
            }

            this.ValidatePersonalInfo(personalInfo);

            long fileLength = this.fileStream.Length;
            byte[] buffer = new byte[6]; // Read status (2 bytes) and id (4 bytes)

            for (long position = 0; position < fileLength; position += RecordSize)
            {
                this.fileStream.Seek(position, SeekOrigin.Begin);
                this.fileStream.Read(buffer, 0, 6);

                short status = BitConverter.ToInt16(buffer, 0);
                int recordId = BitConverter.ToInt32(buffer, 2);

                if (status == ActiveStatus && recordId == id)
                {
                    throw new ArgumentException($"Record with id {id} already exists.", nameof(id));
                }
            }

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
        public ReadOnlyCollection<int> DeleteRecords(string field, string value)
        {
            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            var deletedIds = new List<int>();
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
                    if (status != ActiveStatus)
                    {
                        continue;
                    }

                    int id = reader.ReadInt32();
                    string firstName = Encoding.ASCII.GetString(reader.ReadBytes(60)).TrimEnd('\0', ' ');
                    string lastName = Encoding.ASCII.GetString(reader.ReadBytes(60)).TrimEnd('\0', ' ');
                    int year = reader.ReadInt32();
                    int month = reader.ReadInt32();
                    int day = reader.ReadInt32();
                    short age = reader.ReadInt16();
                    decimal salary = reader.ReadDecimal();
                    char gender = (char)reader.ReadInt16();

                    bool match = false;
                    switch (field.ToLowerInvariant())
                    {
                        case "id":
                            match = id.ToString(CultureInfo.InvariantCulture) == value;
                            break;
                        case "firstname":
                            match = firstName.Equals(value, StringComparison.OrdinalIgnoreCase);
                            break;
                        case "lastname":
                            match = lastName.Equals(value, StringComparison.OrdinalIgnoreCase);
                            break;
                        case "dateofbirth":
                            match = new DateTime(year, month, day).ToString("yyyy-MM-dd") == value;
                            break;
                        case "age":
                            match = age.ToString(CultureInfo.InvariantCulture) == value;
                            break;
                        case "salary":
                            match = salary.ToString(CultureInfo.InvariantCulture) == value;
                            break;
                        case "gender":
                            match = gender.ToString().Equals(value, StringComparison.OrdinalIgnoreCase);
                            break;
                    }

                    if (match)
                    {
                        deletedIds.Add(id);
                        this.fileStream.Seek(position, SeekOrigin.Begin);
                        using (var writer = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
                        {
                            writer.Write(InactiveStatus);
                        }
                    }
                }
            }

            this.fileStream.Flush();
            return new ReadOnlyCollection<int>(deletedIds);
        }

        /// <inheritdoc/>
        public int UpdateRecords(Dictionary<string, string> fieldsToUpdate, Dictionary<string, string> conditions)
        {
            if (fieldsToUpdate == null || conditions == null)
            {
                throw new ArgumentNullException(fieldsToUpdate == null ? nameof(fieldsToUpdate) : nameof(conditions));
            }

            int updatedCount = 0;
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
                    if (status != ActiveStatus)
                    {
                        continue;
                    }

                    var record = new FileCabinetRecord
                    {
                        Id = reader.ReadInt32(),
                        FirstName = Encoding.ASCII.GetString(reader.ReadBytes(60)).TrimEnd('\0', ' '),
                        LastName = Encoding.ASCII.GetString(reader.ReadBytes(60)).TrimEnd('\0', ' '),
                        DateOfBirth = new DateTime(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                        Age = reader.ReadInt16(),
                        Salary = reader.ReadDecimal(),
                        Gender = (char)reader.ReadInt16(),
                    };

                    if (MatchesConditions(record, conditions))
                    {
                        if (UpdateRecord(record, fieldsToUpdate))
                        {
                            this.fileStream.Seek(position, SeekOrigin.Begin);
                            this.WriteRecord(record);
                            updatedCount++;
                        }
                    }
                }
            }

            this.fileStream.Flush();
            return updatedCount;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> SelectRecords(List<string> fields, Dictionary<string, string> conditions)
        {
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }

            var result = new List<FileCabinetRecord>();
            byte[] buffer = new byte[RecordSize];

            this.fileStream.Seek(0, SeekOrigin.Begin);

            while (this.fileStream.Read(buffer, 0, RecordSize) == RecordSize)
            {
                using var memoryStream = new MemoryStream(buffer);
                using var reader = new BinaryReader(memoryStream);

                short status = reader.ReadInt16();
                if (status != ActiveStatus)
                {
                    continue;
                }

                var record = new FileCabinetRecord
                {
                    Id = reader.ReadInt32(),
                    FirstName = Encoding.ASCII.GetString(reader.ReadBytes(60)).TrimEnd('\0', ' '),
                    LastName = Encoding.ASCII.GetString(reader.ReadBytes(60)).TrimEnd('\0', ' '),
                };

                int year = reader.ReadInt32();
                int month = reader.ReadInt32();
                int day = reader.ReadInt32();

                try
                {
                    record.DateOfBirth = new DateTime(year, month, day, 0, 0, 0, 0, DateTimeKind.Local);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Skip this record if the date is invalid
                    continue;
                }

                record.Age = reader.ReadInt16();
                record.Salary = reader.ReadDecimal();
                record.Gender = (char)reader.ReadInt16();

                if (conditions == null || MatchesConditions(record, conditions))
                {
                    result.Add(record);
                }
            }

            // If no fields are specified, return all fields
            if (fields.Count == 0)
            {
                return result;
            }

            // Project the records to include only the specified fields
            return result.Select(r => new FileCabinetRecord
            {
                Id = fields.Contains("id", StringComparer.OrdinalIgnoreCase) ? r.Id : 0,
                FirstName = fields.Contains("firstname", StringComparer.OrdinalIgnoreCase) ? r.FirstName : null,
                LastName = fields.Contains("lastname", StringComparer.OrdinalIgnoreCase) ? r.LastName : null,
                DateOfBirth = fields.Contains("dateofbirth", StringComparer.OrdinalIgnoreCase) ? r.DateOfBirth : default,
                Age = fields.Contains("age", StringComparer.OrdinalIgnoreCase) ? r.Age : (short)0,
                Salary = fields.Contains("salary", StringComparer.OrdinalIgnoreCase) ? r.Salary : 0,
                Gender = fields.Contains("gender", StringComparer.OrdinalIgnoreCase) ? r.Gender : '\0',
            });
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
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var result = this.FindByPredicate(record =>
            {
                return !string.IsNullOrEmpty(record.FirstName) &&
                    record.FirstName.Trim().Equals(firstName.Trim(), StringComparison.OrdinalIgnoreCase);
            });
            Console.WriteLine($"Found {result.Count} records");
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            var result = this.FindByPredicate(record =>
            {
                return !string.IsNullOrEmpty(record.LastName) &&
                       record.LastName.Trim().Equals(lastName.Trim(), StringComparison.OrdinalIgnoreCase);
            });
            Console.WriteLine($"Found {result.Count} records");
            return result;
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            Console.WriteLine($"Searching for dateOfBirth: '{dateOfBirth}'");
            if (DateTime.TryParse(dateOfBirth, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                var result = this.FindByPredicate(record =>
                {
                    return record.DateOfBirth.Date == date.Date;
                });
                Console.WriteLine($"Found {result.Count} records");
                return result;
            }

            Console.WriteLine("Invalid date format");
            return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
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

        private static bool MatchesConditions(FileCabinetRecord record, Dictionary<string, string> conditions)
        {
            foreach (var condition in conditions)
            {
                switch (condition.Key.ToLowerInvariant())
                {
                    case "id":
                        if (record.Id.ToString(CultureInfo.InvariantCulture) != condition.Value)
                        {
                            return false;
                        }

                        break;
                    case "firstname":
                        if (record.FirstName is null || !record.FirstName.Equals(condition.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }

                        break;
                    case "lastname":
                        if (record.LastName is null || !record.LastName.Equals(condition.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }

                        break;
                    case "dateofbirth":
                        if (record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) != condition.Value)
                        {
                            return false;
                        }

                        break;
                    case "age":
                        if (record.Age.ToString(CultureInfo.InvariantCulture) != condition.Value)
                        {
                            return false;
                        }

                        break;
                    case "salary":
                        if (record.Salary.ToString(CultureInfo.InvariantCulture) != condition.Value)
                        {
                            return false;
                        }

                        break;
                    case "gender":
                        if (record.Gender.ToString() != condition.Value)
                        {
                            return false;
                        }

                        break;
                }
            }

            return true;
        }

        private static bool UpdateRecord(FileCabinetRecord record, Dictionary<string, string> fieldsToUpdate)
        {
            bool updated = false;
            foreach (var field in fieldsToUpdate)
            {
                switch (field.Key.ToLowerInvariant())
                {
                    case "firstname":
                        record.FirstName = field.Value;
                        updated = true;
                        break;
                    case "lastname":
                        record.LastName = field.Value;
                        updated = true;
                        break;
                    case "dateofbirth":
                        if (DateTime.TryParse(field.Value, out DateTime dateOfBirth))
                        {
                            record.DateOfBirth = dateOfBirth;
                            updated = true;
                        }

                        break;
                    case "age":
                        if (short.TryParse(field.Value, out short age))
                        {
                            record.Age = age;
                            updated = true;
                        }

                        break;
                    case "salary":
                        if (decimal.TryParse(field.Value, out decimal salary))
                        {
                            record.Salary = salary;
                            updated = true;
                        }

                        break;
                    case "gender":
                        if (char.TryParse(field.Value, out char gender))
                        {
                            record.Gender = gender;
                            updated = true;
                        }

                        break;
                }
            }

            return updated;
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

        private ReadOnlyCollection<FileCabinetRecord> FindByPredicate(Func<FileCabinetRecord, bool> predicate)
        {
            var result = new List<FileCabinetRecord>();
            byte[] buffer = new byte[RecordSize];

            this.fileStream.Seek(0, SeekOrigin.Begin);

            while (this.fileStream.Read(buffer, 0, RecordSize) == RecordSize)
            {
                using var memoryStream = new MemoryStream(buffer);
                using var reader = new BinaryReader(memoryStream);

                short status = reader.ReadInt16();
                if (status != ActiveStatus)
                {
                    continue;
                }

                var record = new FileCabinetRecord
                {
                    Id = reader.ReadInt32(),
                    FirstName = Encoding.ASCII.GetString(reader.ReadBytes(60)).TrimEnd('\0', ' '),
                    LastName = Encoding.ASCII.GetString(reader.ReadBytes(60)).TrimEnd('\0', ' '),
                };

                int year = reader.ReadInt32();
                int month = reader.ReadInt32();
                int day = reader.ReadInt32();

                try
                {
                    record.DateOfBirth = new DateTime(year, month, day, 0, 0, 0, 0, DateTimeKind.Local);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Skip this record if the date is invalid
                    continue;
                }

                record.Age = reader.ReadInt16();
                record.Salary = reader.ReadDecimal();
                record.Gender = (char)reader.ReadInt16();

                if (predicate(record))
                {
                    result.Add(record);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(result);
        }

        private void WriteRecord(FileCabinetRecord record)
        {
            using (var writer = new BinaryWriter(this.fileStream, Encoding.ASCII, true))
            {
                writer.Write(ActiveStatus);
                writer.Write(record.Id);
                writer.Write(Encoding.ASCII.GetBytes((record.FirstName ?? string.Empty).PadRight(60)));
                writer.Write(Encoding.ASCII.GetBytes((record.LastName ?? string.Empty).PadRight(60)));
                writer.Write(record.DateOfBirth.Year);
                writer.Write(record.DateOfBirth.Month);
                writer.Write(record.DateOfBirth.Day);
                writer.Write(record.Age);
                writer.Write(record.Salary);
                writer.Write((short)record.Gender);
            }
        }
    }
}