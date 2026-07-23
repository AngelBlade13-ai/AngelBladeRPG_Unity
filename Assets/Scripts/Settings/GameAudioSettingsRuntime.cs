public static class GameAudioSettingsRuntime
{
    private static GameAudioSettingsService service;

    public static GameAudioSettings Current => Service.Current;

    public static event System.Action Changed
    {
        add => Service.Changed += value;
        remove => Service.Changed -= value;
    }

    public static bool SetMusicVolume(float value)
    {
        return Service.SetMusicVolume(value);
    }

    public static bool SetSoundVolume(float value)
    {
        return Service.SetSoundVolume(value);
    }

    public static bool ResetToDefaults()
    {
        return Service.ResetToDefaults();
    }

    private static GameAudioSettingsService Service
    {
        get
        {
            if (service == null)
            {
                service = new GameAudioSettingsService(
                    new PlayerPrefsGameAudioSettingsStore());
            }

            return service;
        }
    }
}
