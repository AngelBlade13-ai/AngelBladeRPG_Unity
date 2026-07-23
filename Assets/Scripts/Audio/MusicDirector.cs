using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource), typeof(CategorizedAudioSource))]
public sealed class MusicDirector : MonoBehaviour
{
    [SerializeField] private AudioClip mainMenuTheme;
    [SerializeField] private AudioClip suncrestTheme;
    [SerializeField] private AudioSource musicSource;

    public static MusicDirector Instance { get; private set; }
    public MusicCue CurrentCue { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ResolveSource();
        ConfigureSource();
    }

    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        SceneManager.sceneLoaded += HandleSceneLoaded;
        PlayForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Configure(
        AudioClip configuredMainMenuTheme,
        AudioClip configuredSuncrestTheme)
    {
        mainMenuTheme = configuredMainMenuTheme;
        suncrestTheme = configuredSuncrestTheme;
        ResolveSource();
        ConfigureSource();
    }

    public void PlayForScene(string sceneName)
    {
        ResolveSource();
        if (musicSource == null)
        {
            return;
        }

        MusicCue requestedCue = MusicSceneCatalog.GetCue(sceneName);
        AudioClip requestedClip = GetClip(requestedCue);

        if (requestedClip == null)
        {
            CurrentCue = MusicCue.None;
            musicSource.Stop();
            musicSource.clip = null;
            return;
        }

        CurrentCue = requestedCue;
        if (musicSource.clip == requestedClip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = requestedClip;
        musicSource.Play();
    }

    public AudioClip GetClip(MusicCue cue)
    {
        switch (cue)
        {
            case MusicCue.MainMenu:
                return mainMenuTheme;
            case MusicCue.Suncrest:
                return suncrestTheme;
            default:
                return null;
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayForScene(scene.name);
    }

    private void ResolveSource()
    {
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }
    }

    private void ConfigureSource()
    {
        if (musicSource == null)
        {
            return;
        }

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
    }
}
