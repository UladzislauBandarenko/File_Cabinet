using System.Collections;

namespace FileCabinetApp;

public class FileCabinetMemoryIterator : IEnumerator<FileCabinetRecord>
{
    private readonly List<FileCabinetRecord> records;
    private int currentIndex = -1;

    public FileCabinetMemoryIterator(List<FileCabinetRecord> records)
    {
        this.records = records;
    }

    public FileCabinetRecord Current => this.currentIndex >= 0 && this.currentIndex < this.records.Count ? this.records[this.currentIndex] : null;

    object IEnumerator.Current => this.Current;

    public bool MoveNext()
    {
        if (this.currentIndex < this.records.Count - 1)
        {
            this.currentIndex++;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        this.currentIndex = -1;
    }

    public void Dispose()
    {
        // Нет необходимости в освобождении ресурсов
    }
}