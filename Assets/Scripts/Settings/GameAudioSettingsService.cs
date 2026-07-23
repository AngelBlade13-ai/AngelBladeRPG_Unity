using System;

public sealed class GameAudioSettingsService
{
    private readonly IGameAudioSettingsStore store;

    public GameAudioSettings Current { get; private set; }
    public event Action Changed;

    public GameAudioSettingsService(IGameAudioSettingsStore store)
    {
        this.store = store ??
            throw new ArgumentNullException(nameof(store));
        Current = store.TryLoad(out GameAudioSettings loaded)
            ? loaded
            : new GameAudioSettings();
    }

    public bool SetMusicVolume(float value)
    {
        float clamped = GameAudioSettings.ClampVolume(value);
        if (Approximately(Current.MusicVolume, clamped))
        {
            return false;
        }

        Replace(new GameAudioSettings(
            clamped,
            Current.SoundVolume));
        return true;
    }

    public bool SetSoundVolume(float value)
    {
        float clamped = GameAudioSettings.ClampVolume(value);
        if (Approximately(Current.SoundVolume, clamped))
        {
            return false;
        }

        Replace(new GameAudioSettings(
            Current.MusicVolume,
            clamped));
        return true;
    }

    public bool ResetToDefaults()
    {
        var defaults = new GameAudioSettings();
        if (Approximately(
                Current.MusicVolume,
                defaults.MusicVolume) &&
            Approximately(
                Current.SoundVolume,
                defaults.SoundVolume))
        {
            return false;
        }

        Replace(defaults);
        return true;
    }

    private void Replace(GameAudioSettings settings)
    {
        Current = settings;
        store.Save(settings);
        Changed?.Invoke();
    }

    private static bool Approximately(float left, float right)
    {
        return Math.Abs(left - right) <= 0.0001f;
    }
}
