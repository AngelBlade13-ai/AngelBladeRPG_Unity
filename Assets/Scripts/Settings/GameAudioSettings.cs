using System;

public sealed class GameAudioSettings
{
    public const float DefaultMusicVolume = 1f;
    public const float DefaultSoundVolume = 1f;

    public float MusicVolume { get; }
    public float SoundVolume { get; }

    public GameAudioSettings(
        float musicVolume = DefaultMusicVolume,
        float soundVolume = DefaultSoundVolume)
    {
        MusicVolume = ClampVolume(musicVolume);
        SoundVolume = ClampVolume(soundVolume);
    }

    public static float ClampVolume(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
        {
            return 0f;
        }

        return Math.Max(0f, Math.Min(1f, value));
    }
}
