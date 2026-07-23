using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class GameAudioSettingsTests
    {
        [Test]
        public void DefaultsStartAtFullVolume()
        {
            var settings = new GameAudioSettings();

            Assert.That(settings.MusicVolume, Is.EqualTo(1f));
            Assert.That(settings.SoundVolume, Is.EqualTo(1f));
        }

        [Test]
        public void ConstructorClampsVolumesToSupportedRange()
        {
            var settings = new GameAudioSettings(1.5f, -0.5f);

            Assert.That(settings.MusicVolume, Is.EqualTo(1f));
            Assert.That(settings.SoundVolume, Is.EqualTo(0f));
        }

        [Test]
        public void InvalidNumericVolumesBecomeMuted()
        {
            var settings = new GameAudioSettings(
                float.NaN,
                float.PositiveInfinity);

            Assert.That(settings.MusicVolume, Is.EqualTo(0f));
            Assert.That(settings.SoundVolume, Is.EqualTo(0f));
        }

        [Test]
        public void ServiceLoadsExistingSettings()
        {
            var store = new MemoryStore(
                new GameAudioSettings(0.25f, 0.75f));
            var service = new GameAudioSettingsService(store);

            Assert.That(service.Current.MusicVolume, Is.EqualTo(0.25f));
            Assert.That(service.Current.SoundVolume, Is.EqualTo(0.75f));
        }

        [Test]
        public void MissingSettingsUseDefaultsWithoutWriting()
        {
            var store = new MemoryStore();
            var service = new GameAudioSettingsService(store);

            Assert.That(service.Current.MusicVolume, Is.EqualTo(1f));
            Assert.That(service.Current.SoundVolume, Is.EqualTo(1f));
            Assert.That(store.SaveCount, Is.EqualTo(0));
        }

        [Test]
        public void ChangingMusicVolumePersistsAndRaisesEvent()
        {
            var store = new MemoryStore();
            var service = new GameAudioSettingsService(store);
            int eventCount = 0;
            service.Changed += () => eventCount += 1;

            bool changed = service.SetMusicVolume(0.4f);

            Assert.That(changed, Is.True);
            Assert.That(service.Current.MusicVolume, Is.EqualTo(0.4f));
            Assert.That(service.Current.SoundVolume, Is.EqualTo(1f));
            Assert.That(store.SaveCount, Is.EqualTo(1));
            Assert.That(eventCount, Is.EqualTo(1));
        }

        [Test]
        public void ChangingSoundVolumePersistsClampedValue()
        {
            var store = new MemoryStore();
            var service = new GameAudioSettingsService(store);

            bool changed = service.SetSoundVolume(-2f);

            Assert.That(changed, Is.True);
            Assert.That(service.Current.SoundVolume, Is.EqualTo(0f));
            Assert.That(store.Saved.SoundVolume, Is.EqualTo(0f));
        }

        [Test]
        public void SettingSameVolumeDoesNotWriteOrRaiseEvent()
        {
            var store = new MemoryStore();
            var service = new GameAudioSettingsService(store);
            int eventCount = 0;
            service.Changed += () => eventCount += 1;

            bool changed = service.SetMusicVolume(1f);

            Assert.That(changed, Is.False);
            Assert.That(store.SaveCount, Is.EqualTo(0));
            Assert.That(eventCount, Is.EqualTo(0));
        }

        [Test]
        public void ResetRestoresAndPersistsDefaults()
        {
            var store = new MemoryStore(
                new GameAudioSettings(0.2f, 0.3f));
            var service = new GameAudioSettingsService(store);

            bool changed = service.ResetToDefaults();

            Assert.That(changed, Is.True);
            Assert.That(service.Current.MusicVolume, Is.EqualTo(1f));
            Assert.That(service.Current.SoundVolume, Is.EqualTo(1f));
            Assert.That(store.SaveCount, Is.EqualTo(1));
        }

        [Test]
        public void CategorizedSourceMultipliesBaseAndCategoryVolumes()
        {
            Assert.That(
                CategorizedAudioSource.CalculateVolume(0.8f, 0.25f),
                Is.EqualTo(0.2f).Within(0.0001f));
            Assert.That(
                CategorizedAudioSource.CalculateVolume(2f, -1f),
                Is.EqualTo(0f));
        }

        private sealed class MemoryStore : IGameAudioSettingsStore
        {
            private readonly GameAudioSettings loaded;

            public int SaveCount { get; private set; }
            public GameAudioSettings Saved { get; private set; }

            public MemoryStore(GameAudioSettings loaded = null)
            {
                this.loaded = loaded;
            }

            public bool TryLoad(out GameAudioSettings settings)
            {
                settings = loaded;
                return settings != null;
            }

            public void Save(GameAudioSettings settings)
            {
                SaveCount += 1;
                Saved = settings;
            }
        }
    }
}
