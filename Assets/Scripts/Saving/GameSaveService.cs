using System;
using System.Globalization;

public enum PlayerSaveStatus
{
    Success,
    NoStartedGame,
    BattleActive,
    NoSafeLocation,
    InvalidData,
    IoError
}

public enum PlayerContinueStatus
{
    Success,
    RecoveredBackup,
    NoSave,
    InvalidSave,
    IoError
}

public sealed class GameSaveService
{
    public const string ManualSlotId = "manual_01";
    public const string AutosaveSlotId = "autosave_01";

    private readonly SaveFileStorage storage;
    private readonly GameSaveCoordinator coordinator;
    private readonly Func<double> playTimeProvider;
    private readonly Func<string> utcTimestampProvider;
    private readonly Func<string> gameVersionProvider;
    private double carriedPlayTime;
    private double playTimeBaseline;
    private string appearanceId = "";
    private string currentSceneName = "";
    private string currentSpawnId = "";

    public bool HasSafeLocation =>
        !string.IsNullOrWhiteSpace(currentSceneName) &&
        !string.IsNullOrWhiteSpace(currentSpawnId);

    public GameSaveService(
        SaveFileStorage storage,
        Func<double> playTimeProvider,
        Func<string> utcTimestampProvider,
        Func<string> gameVersionProvider)
    {
        this.storage = storage ??
            throw new ArgumentNullException(nameof(storage));
        this.playTimeProvider = playTimeProvider ??
            throw new ArgumentNullException(nameof(playTimeProvider));
        this.utcTimestampProvider = utcTimestampProvider ??
            throw new ArgumentNullException(nameof(utcTimestampProvider));
        this.gameVersionProvider = gameVersionProvider ??
            throw new ArgumentNullException(nameof(gameVersionProvider));
        coordinator = new GameSaveCoordinator(storage);
        playTimeBaseline = playTimeProvider();
    }

    public void BeginNewGame(string protagonistAppearanceId = "")
    {
        carriedPlayTime = 0;
        playTimeBaseline = playTimeProvider();
        appearanceId = protagonistAppearanceId?.Trim() ?? "";
        currentSceneName = "";
        currentSpawnId = "";
    }

    public bool RememberLocation(string sceneName, string spawnId)
    {
        if (string.IsNullOrWhiteSpace(sceneName) ||
            string.IsNullOrWhiteSpace(spawnId))
        {
            return false;
        }

        currentSceneName = sceneName.Trim();
        currentSpawnId = spawnId.Trim();
        return true;
    }

    public bool HasContinue()
    {
        return TryFindNewestLoadableSave(out _);
    }

    public PlayerSaveStatus SaveManual()
    {
        return SaveToSlot(ManualSlotId);
    }

    public PlayerSaveStatus SaveAutosave(
        string sceneName,
        string spawnId)
    {
        if (!RememberLocation(sceneName, spawnId))
        {
            return PlayerSaveStatus.NoSafeLocation;
        }

        return SaveToSlot(AutosaveSlotId);
    }

    public PlayerContinueStatus Continue(
        out LocationSaveData location)
    {
        location = null;
        if (!TryFindNewestLoadableSave(out SaveCandidate candidate))
        {
            return PlayerContinueStatus.NoSave;
        }

        GameLoadStatus loadStatus = coordinator.LoadIntoCurrent(
            candidate.SlotId,
            out location);
        if (loadStatus != GameLoadStatus.Success &&
            loadStatus != GameLoadStatus.RecoveredBackup)
        {
            return MapLoadFailure(loadStatus);
        }

        carriedPlayTime = Math.Max(0, candidate.Data.playTimeSeconds);
        playTimeBaseline = playTimeProvider();
        appearanceId = candidate.Data.player.appearanceId?.Trim() ?? "";
        RememberLocation(location.sceneName, location.spawnId);
        return loadStatus == GameLoadStatus.RecoveredBackup
            ? PlayerContinueStatus.RecoveredBackup
            : PlayerContinueStatus.Success;
    }

    private PlayerSaveStatus SaveToSlot(string slotId)
    {
        GameSession session = GameSessionStore.Current;
        if (session == null || !session.HasPlayer)
        {
            return PlayerSaveStatus.NoStartedGame;
        }

        if (session.HasActiveBattle)
        {
            return PlayerSaveStatus.BattleActive;
        }

        if (!HasSafeLocation)
        {
            return PlayerSaveStatus.NoSafeLocation;
        }

        double elapsed = Math.Max(0, playTimeProvider() - playTimeBaseline);
        var context = new SaveCaptureContext(
            slotId,
            gameVersionProvider(),
            utcTimestampProvider(),
            carriedPlayTime + elapsed,
            appearanceId,
            currentSceneName,
            currentSpawnId);

        switch (coordinator.SaveCurrent(slotId, context))
        {
            case GameSaveStatus.Success:
                return PlayerSaveStatus.Success;
            case GameSaveStatus.IoError:
                return PlayerSaveStatus.IoError;
            default:
                return PlayerSaveStatus.InvalidData;
        }
    }

    private bool TryFindNewestLoadableSave(out SaveCandidate newest)
    {
        newest = null;
        TryConsiderCandidate(ManualSlotId, ref newest);
        TryConsiderCandidate(AutosaveSlotId, ref newest);
        return newest != null;
    }

    private void TryConsiderCandidate(
        string slotId,
        ref SaveCandidate newest)
    {
        SaveFileReadStatus readStatus =
            storage.TryRead(slotId, out GameSaveData data);
        if (readStatus != SaveFileReadStatus.Success &&
            readStatus != SaveFileReadStatus.RecoveredBackup)
        {
            return;
        }

        if (GameSessionSaveMapper.TryRestore(
                data,
                out _,
                out _) != GameSessionRestoreStatus.Success)
        {
            return;
        }

        DateTimeOffset timestamp = ParseTimestamp(data.savedAtUtc);
        if (newest == null || timestamp >= newest.Timestamp)
        {
            newest = new SaveCandidate(slotId, data, timestamp);
        }
    }

    private static DateTimeOffset ParseTimestamp(string value)
    {
        return DateTimeOffset.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out DateTimeOffset timestamp)
            ? timestamp
            : DateTimeOffset.MinValue;
    }

    private static PlayerContinueStatus MapLoadFailure(
        GameLoadStatus status)
    {
        return status == GameLoadStatus.IoError
            ? PlayerContinueStatus.IoError
            : PlayerContinueStatus.InvalidSave;
    }

    private sealed class SaveCandidate
    {
        public string SlotId { get; }
        public GameSaveData Data { get; }
        public DateTimeOffset Timestamp { get; }

        public SaveCandidate(
            string slotId,
            GameSaveData data,
            DateTimeOffset timestamp)
        {
            SlotId = slotId;
            Data = data;
            Timestamp = timestamp;
        }
    }
}
