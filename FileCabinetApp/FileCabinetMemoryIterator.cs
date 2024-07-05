using System.Collections;
using System.Collections.ObjectModel;

namespace FileCabinetApp;

/// <summary>
/// FileCabinetMemoryIterator class.
/// </summary>
public class FileCabinetMemoryIterator : IEnumerator<FileCabinetRecord>
{
    private readonly ReadOnlyCollection<FileCabinetRecord> records;
    private int currentIndex = -1;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCabinetMemoryIterator"/> class.
    /// </summary>
    /// <param name="records">The records.</param>
    public FileCabinetMemoryIterator(ReadOnlyCollection<FileCabinetRecord> records)
    {
        this.records = records;
    }

    /// <inheritdoc/>
    public FileCabinetRecord Current
    {
        get
        {
            if (this.currentIndex < 0 || this.currentIndex >= this.records.Count)
            {
                throw new InvalidOperationException("Enumeration has either not started or has already finished.");
            }

            return this.records[this.currentIndex];
        }
    }

    /// <inheritdoc/>
    object IEnumerator.Current => this.Current;

    /// <inheritdoc/>
    public bool MoveNext()
    {
        if (this.currentIndex < this.records.Count - 1)
        {
            this.currentIndex++;
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        this.currentIndex = -1;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.disposed = true;
        }
    }
}