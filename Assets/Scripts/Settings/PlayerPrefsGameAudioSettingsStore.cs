using UnityEngine;

public sealed class PlayerPrefsGameAudioSettingsStore :
    IGameAudioSettingsStore
{
    private const string MusicVolumeKey =
        "petals_in_the_dusk.settings.music_volume";
    private const string SoundVolumeKey =
        "petals_in_the_dusk.settings.sound_volume";

    public bool TryLoad(out GameAudioSettings settings)
    {
        if (!PlayerPrefs.HasKey(MusicVolumeKey) ||
            !PlayerPrefs.HasKey(SoundVolumeKey))
        {
            settings = null;
            return false;
        }

        settings = new GameAudioSettings(
            PlayerPrefs.GetFloat(
                MusicVolumeKey,
                GameAudioSettings.DefaultMusicVolume),
            PlayerPrefs.GetFloat(
                SoundVolumeKey,
                GameAudioSettings.DefaultSoundVolume));
        return true;
    }

    public void Save(GameAudioSettings settings)
    {
        if (settings == null)
        {
            throw new System.ArgumentNullException(nameof(settings));
        }

        PlayerPrefs.SetFloat(MusicVolumeKey, settings.MusicVolume);
        PlayerPrefs.SetFloat(SoundVolumeKey, settings.SoundVolume);
        PlayerPrefs.Save();
    }
}
