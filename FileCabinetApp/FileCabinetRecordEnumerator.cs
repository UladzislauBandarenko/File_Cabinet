namespace FileCabinetApp;

using System.Collections;

/// <summary>
/// FileCabinetRecordEnumerator class.
/// </summary>
public class FileCabinetRecordEnumerator : IEnumerator<FileCabinetRecord>
{
    private readonly IEnumerator<FileCabinetRecord> enumerator;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCabinetRecordEnumerator"/> class.
    /// </summary>
    /// <param name="service">The service.</param>
    public FileCabinetRecordEnumerator(IFileCabinetService service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        this.enumerator = service.GetRecords(r => r.ToString() !).GetEnumerator();
    }

    /// <inheritdoc/>
    public FileCabinetRecord Current => this.enumerator.Current;

    /// <inheritdoc/>
    object IEnumerator.Current => this.Current;

        /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        return this.enumerator.MoveNext();
    }

    /// <inheritdoc/>
    public void Reset()
    {
        this.enumerator.Reset();
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
                this.enumerator.Dispose();
            }

            this.disposed = true;
        }
    }
}