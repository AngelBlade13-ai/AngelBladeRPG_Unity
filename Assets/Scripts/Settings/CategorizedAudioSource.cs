using UnityEngine;

public enum GameAudioCategory
{
    Music,
    Sound
}

[RequireComponent(typeof(AudioSource))]
public sealed class CategorizedAudioSource : MonoBehaviour
{
    [SerializeField] private GameAudioCategory category;
    [SerializeField, Range(0f, 1f)] private float baseVolume = 1f;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        ResolveAudioSource();
    }

    private void OnEnable()
    {
        ResolveAudioSource();
        GameAudioSettingsRuntime.Changed += ApplyVolume;
        ApplyVolume();
    }

    private void OnDisable()
    {
        GameAudioSettingsRuntime.Changed -= ApplyVolume;
    }

    public void Configure(
        GameAudioCategory audioCategory,
        float sourceBaseVolume = 1f)
    {
        category = audioCategory;
        baseVolume = GameAudioSettings.ClampVolume(sourceBaseVolume);
        ResolveAudioSource();
        ApplyVolume();
    }

    public void ApplyVolume()
    {
        if (audioSource == null)
        {
            return;
        }

        float categoryVolume = category == GameAudioCategory.Music
            ? GameAudioSettingsRuntime.Current.MusicVolume
            : GameAudioSettingsRuntime.Current.SoundVolume;
        audioSource.volume = CalculateVolume(
            baseVolume,
            categoryVolume);
    }

    public static float CalculateVolume(
        float sourceBaseVolume,
        float categoryVolume)
    {
        return GameAudioSettings.ClampVolume(sourceBaseVolume) *
            GameAudioSettings.ClampVolume(categoryVolume);
    }

    private void ResolveAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
}
