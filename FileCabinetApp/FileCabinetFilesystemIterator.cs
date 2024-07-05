using System.Collections;

namespace FileCabinetApp;

public class FileCabinetFilesystemIterator : IEnumerator<FileCabinetRecord>
{
    private const int RecordSize = 278;
    private const short ActiveStatus = 1;
    private readonly FileStream fileStream;
    private readonly List<long> positions;
    private int currentIndex = -1;

    public FileCabinetFilesystemIterator(FileStream fileStream, List<long> positions)
    {
        this.fileStream = fileStream;
        this.positions = positions;
    }

    public FileCabinetRecord Current { get; private set; }

    object IEnumerator.Current => this.Current;

    public bool MoveNext()
    {
        if (this.currentIndex < this.positions.Count - 1)
        {
            this.currentIndex++;
            this.Current = this.ReadRecordAtPosition(this.positions[this.currentIndex]);
            return true;
        }

        return false;
    }

    public void Reset()
    {
        this.currentIndex = -1;
        this.Current = null;
    }

    public void Dispose()
    {
        // Нет необходимости в освобождении ресурсов
    }

    private FileCabinetRecord ReadRecordAtPosition(long position)
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
}