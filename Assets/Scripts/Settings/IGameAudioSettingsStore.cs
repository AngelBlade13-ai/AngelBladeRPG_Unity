public interface IGameAudioSettingsStore
{
    bool TryLoad(out GameAudioSettings settings);
    void Save(GameAudioSettings settings);
}
