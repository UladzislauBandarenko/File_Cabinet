using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 278;
        private readonly IRecordValidator validator;
        private readonly FileStream fileStream;

        public FileCabinetFilesystemService(IRecordValidator validator, FileStream fileStream)
        {
            this.validator = validator;
            this.fileStream = fileStream;
        }

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

            this.fileStream.Seek(0, SeekOrigin.End);
            this.fileStream.Write(record, 0, RecordSize);
            this.fileStream.Flush();

            return id;
        }

        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        public int GetStat()
        {
            throw new NotImplementedException();
        }

        public void EditRecord(int id, PersonalInfo personalInfo)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            throw new NotImplementedException();
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}