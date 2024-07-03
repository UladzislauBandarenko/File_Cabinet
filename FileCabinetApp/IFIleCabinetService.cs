using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    public interface IFileCabinetService
    {
        int CreateRecord(PersonalInfo personalInfo);

        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        int GetStat();

        void EditRecord(int id, PersonalInfo personalInfo);

        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

        FileCabinetServiceSnapshot MakeSnapshot();
    }
}