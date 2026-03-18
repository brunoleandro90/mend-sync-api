namespace MendSync.Infrastructure.Data;

/// <summary>
/// Singleton that tracks whether the base data sync has been triggered in this process lifetime.
/// Prevents duplicate concurrent syncs.
/// </summary>
public class SyncState
{
    private volatile bool _isSeeded;
    private volatile bool _isSyncing;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public bool IsSeeded => _isSeeded;
    public bool IsSyncing => _isSyncing;

    public void MarkSeeded() => _isSeeded = true;

    /// <summary>
    /// Returns true if the caller acquired the lock (i.e. sync should proceed).
    /// Returns false if a sync is already in progress.
    /// </summary>
    public async Task<bool> TryBeginSyncAsync()
    {
        if (!await _lock.WaitAsync(0))
            return false;

        _isSyncing = true;
        return true;
    }

    public void EndSync(bool success)
    {
        _isSyncing = false;
        if (success) _isSeeded = true;
        _lock.Release();
    }
}
