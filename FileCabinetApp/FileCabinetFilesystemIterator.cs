using System.Collections;

namespace FileCabinetApp;

/// <summary>
/// FileCabinetFilesystemIterator class.
/// </summary>
public class FileCabinetFilesystemIterator : IEnumerator<FileCabinetRecord>
{
    private const int RecordSize = 278;
    private const short ActiveStatus = 1;
    private readonly FileStream fileStream;
    private readonly IReadOnlyList<long> positions;
    private int currentIndex = -1;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCabinetFilesystemIterator"/> class.
    /// </summary>
    /// <param name="fileStream">The file stream.</param>
    /// <param name="positions">The positions.</param>
    public FileCabinetFilesystemIterator(FileStream fileStream, IReadOnlyList<long> positions)
    {
        this.fileStream = fileStream;
        this.positions = positions;
    }

    /// <summary>
    /// Gets the current record.
    /// </summary>
    /// <value>
    /// The current record.
    /// </value>
    public FileCabinetRecord Current
    {
        get
        {
            if (this.currentIndex < 0 || this.currentIndex >= this.positions.Count)
            {
                throw new InvalidOperationException("Enumeration has either not started or has already finished.");
            }

            return this.ReadRecordAtPosition(this.positions[this.currentIndex])
                ?? throw new InvalidOperationException("Current record is null.");
        }
    }

    /// <inheritdoc/>
    object IEnumerator.Current => this.Current;

    /// <inheritdoc/>
    public bool MoveNext()
    {
        if (this.currentIndex < this.positions.Count - 1)
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
    /// Disposes the specified disposing.
    /// </summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                this.fileStream?.Dispose();
            }

            this.disposed = true;
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
}