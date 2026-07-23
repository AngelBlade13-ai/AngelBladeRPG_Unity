using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MusicPlaybackSetupTool
{
    private const string MainScenePath =
        "Assets/Scenes/MainGameScene.unity";
    private const string MainMenuThemePath =
        "Assets/ThirdParty/Audio/FreeStockMusic/" +
        "unburdening_feelings_arthur_vyncke.wav";
    private const string SuncrestThemePath =
        "Assets/ThirdParty/Audio/FreeStockMusic/" +
        "village_ambiance_alexander_nakarada.mp3";
    private const string StandardBattleThemePath =
        "Assets/ThirdParty/Audio/FreeStockMusic/" +
        "on_your_six_arthur_vyncke.wav";
    private const string GoblinBossThemePath =
        "Assets/ThirdParty/Audio/FreeStockMusic/" +
        "off_to_war_alexander_nakarada.mp3";
    private const string DirectorObjectName = "MusicDirector";

    [MenuItem(
        "Tools/AngelBlade RPG/Audio/Install Scene Music Playback")]
    public static void InstallMusicPlayback()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        AudioClip mainMenuTheme =
            AssetDatabase.LoadAssetAtPath<AudioClip>(MainMenuThemePath);
        AudioClip suncrestTheme =
            AssetDatabase.LoadAssetAtPath<AudioClip>(SuncrestThemePath);
        AudioClip standardBattleTheme =
            AssetDatabase.LoadAssetAtPath<AudioClip>(
                StandardBattleThemePath);
        AudioClip goblinBossTheme =
            AssetDatabase.LoadAssetAtPath<AudioClip>(GoblinBossThemePath);
        if (mainMenuTheme == null || suncrestTheme == null ||
            standardBattleTheme == null || goblinBossTheme == null)
        {
            EditorUtility.DisplayDialog(
                "Music Playback Setup",
                "One or more licensed music clips could not be loaded. " +
                "Confirm the licensed music assets finished importing.",
                "OK");
            return;
        }

        string previousScenePath =
            SceneManager.GetActiveScene().path;
        Scene mainScene = EditorSceneManager.OpenScene(
            MainScenePath,
            OpenSceneMode.Single);

        MusicDirector[] existingDirectors = mainScene
            .GetRootGameObjects()
            .SelectMany(root =>
                root.GetComponentsInChildren<MusicDirector>(true))
            .ToArray();
        GameObject directorObject;
        if (existingDirectors.Length > 0)
        {
            directorObject = existingDirectors[0].gameObject;
            for (int index = 1; index < existingDirectors.Length; index += 1)
            {
                Object.DestroyImmediate(
                    existingDirectors[index].gameObject);
            }
        }
        else
        {
            directorObject = new GameObject(DirectorObjectName);
            SceneManager.MoveGameObjectToScene(directorObject, mainScene);
        }

        directorObject.name = DirectorObjectName;
        AudioSource audioSource =
            GetOrAddComponent<AudioSource>(directorObject);
        CategorizedAudioSource categorizedSource =
            GetOrAddComponent<CategorizedAudioSource>(directorObject);
        MusicDirector director =
            GetOrAddComponent<MusicDirector>(directorObject);

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        categorizedSource.Configure(GameAudioCategory.Music);
        audioSource.volume = 1f;
        director.Configure(
            mainMenuTheme,
            suncrestTheme,
            standardBattleTheme,
            goblinBossTheme);

        EditorUtility.SetDirty(audioSource);
        EditorUtility.SetDirty(categorizedSource);
        EditorUtility.SetDirty(director);
        EditorSceneManager.MarkSceneDirty(mainScene);
        EditorSceneManager.SaveScene(mainScene);
        AssetDatabase.SaveAssets();

        if (!string.IsNullOrWhiteSpace(previousScenePath) &&
            previousScenePath != MainScenePath)
        {
            EditorSceneManager.OpenScene(
                previousScenePath,
                OpenSceneMode.Single);
        }

        EditorUtility.DisplayDialog(
            "Music Playback Setup",
            "Menu, Suncrest, standard battle, and future Goblin Boss " +
            "music playback is installed. " +
            "Start Play Mode from MainGameScene to test the transition.",
            "OK");
    }

    private static T GetOrAddComponent<T>(GameObject target)
        where T : Component
    {
        T existing = target.GetComponent<T>();
        return existing == null
            ? target.AddComponent<T>()
            : existing;
    }
}
