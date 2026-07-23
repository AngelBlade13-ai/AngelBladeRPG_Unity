public enum GameSaveStatus
{
    Success,
    InvalidSession,
    InvalidSlot,
    InvalidData,
    UnsupportedVersion,
    IoError
}

public enum GameLoadStatus
{
    Success,
    RecoveredBackup,
    Missing,
    InvalidSlot,
    Corrupt,
    UnsupportedVersion,
    UnsupportedContent,
    InvalidData,
    IoError
}

public sealed class GameSaveCoordinator
{
    private readonly SaveFileStorage storage;

    public GameSaveCoordinator(SaveFileStorage storage)
    {
        this.storage = storage ??
            throw new System.ArgumentNullException(nameof(storage));
    }

    public GameSaveStatus SaveCurrent(
        string slotId,
        SaveCaptureContext context)
    {
        GameSaveData data;
        try
        {
            data = GameSessionSaveMapper.Capture(
                GameSessionStore.Current,
                context);
        }
        catch (System.ArgumentException)
        {
            return GameSaveStatus.InvalidSession;
        }

        switch (storage.TryWrite(slotId, data))
        {
            case SaveFileWriteStatus.Success:
                return GameSaveStatus.Success;
            case SaveFileWriteStatus.InvalidSlot:
                return GameSaveStatus.InvalidSlot;
            case SaveFileWriteStatus.InvalidData:
                return GameSaveStatus.InvalidData;
            case SaveFileWriteStatus.UnsupportedVersion:
                return GameSaveStatus.UnsupportedVersion;
            default:
                return GameSaveStatus.IoError;
        }
    }

    public GameLoadStatus LoadIntoCurrent(
        string slotId,
        out LocationSaveData location)
    {
        location = null;
        SaveFileReadStatus readStatus =
            storage.TryRead(slotId, out GameSaveData data);
        if (readStatus != SaveFileReadStatus.Success &&
            readStatus != SaveFileReadStatus.RecoveredBackup)
        {
            return MapReadFailure(readStatus);
        }

        GameSessionRestoreStatus restoreStatus =
            GameSessionSaveMapper.TryRestore(
                data,
                out GameSession restored,
                out LocationSaveData restoredLocation);
        if (restoreStatus == GameSessionRestoreStatus.UnsupportedContent)
        {
            return GameLoadStatus.UnsupportedContent;
        }

        if (restoreStatus != GameSessionRestoreStatus.Success)
        {
            return GameLoadStatus.InvalidData;
        }

        GameSessionStore.UseRestoredSession(restored);
        location = restoredLocation;
        return readStatus == SaveFileReadStatus.RecoveredBackup
            ? GameLoadStatus.RecoveredBackup
            : GameLoadStatus.Success;
    }

    private static GameLoadStatus MapReadFailure(
        SaveFileReadStatus status)
    {
        switch (status)
        {
            case SaveFileReadStatus.Missing:
                return GameLoadStatus.Missing;
            case SaveFileReadStatus.InvalidSlot:
                return GameLoadStatus.InvalidSlot;
            case SaveFileReadStatus.Corrupt:
                return GameLoadStatus.Corrupt;
            case SaveFileReadStatus.UnsupportedVersion:
                return GameLoadStatus.UnsupportedVersion;
            default:
                return GameLoadStatus.IoError;
        }
    }
}
