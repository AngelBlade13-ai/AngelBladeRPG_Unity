using System;
using System.IO;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class GameSaveServiceTests
    {
        private string testDirectory;
        private SaveFileStorage storage;
        private double clock;
        private string timestamp;
        private GameSaveService service;

        [SetUp]
        public void SetUp()
        {
            testDirectory = Path.Combine(
                Path.GetTempPath(),
                "PetalsInTheDuskTests",
                Guid.NewGuid().ToString("N"));
            storage = new SaveFileStorage(testDirectory);
            clock = 100;
            timestamp = "2026-07-23T10:00:00Z";
            service = CreateService();
            GameSessionStore.BeginNewSession();
        }

        [TearDown]
        public void TearDown()
        {
            GameSessionStore.BeginNewSession();
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, recursive: true);
            }
        }

        [Test]
        public void EmptyStorageHasNoContinue()
        {
            Assert.That(service.HasContinue(), Is.False);
            Assert.That(
                service.Continue(out LocationSaveData location),
                Is.EqualTo(PlayerContinueStatus.NoSave));
            Assert.That(location, Is.Null);
        }

        [Test]
        public void ManualSaveRequiresAStartedGame()
        {
            service.RememberLocation("SuncrestGuildHallScene", "Entrance");

            Assert.That(
                service.SaveManual(),
                Is.EqualTo(PlayerSaveStatus.NoStartedGame));
        }

        [Test]
        public void ManualSaveRequiresAKnownSafeLocation()
        {
            StartGame("Angel");

            Assert.That(
                service.SaveManual(),
                Is.EqualTo(PlayerSaveStatus.NoSafeLocation));
        }

        [Test]
        public void ManualSaveCreatesAContinueCandidate()
        {
            StartGame("Angel");
            service.RememberLocation("SuncrestGuildHallScene", "Entrance");

            Assert.That(
                service.SaveManual(),
                Is.EqualTo(PlayerSaveStatus.Success));
            Assert.That(service.HasContinue(), Is.True);
            Assert.That(
                storage.TryRead(
                    GameSaveService.ManualSlotId,
                    out GameSaveData data),
                Is.EqualTo(SaveFileReadStatus.Success));
            Assert.That(data.player.name, Is.EqualTo("Angel"));
        }

        [Test]
        public void ManualSaveIsBlockedDuringBattle()
        {
            GameSession session = StartGame("Angel");
            service.RememberLocation("SuncrestGuildHallScene", "Entrance");
            session.StartBattle(MonsterCatalog.Get("monster_goblin")
                .CreateMonster());

            Assert.That(
                service.SaveManual(),
                Is.EqualTo(PlayerSaveStatus.BattleActive));
        }

        [Test]
        public void AutosaveRemembersItsDestination()
        {
            StartGame("Angel");

            Assert.That(
                service.SaveAutosave("SuncrestWhisperMarketScene", "West"),
                Is.EqualTo(PlayerSaveStatus.Success));
            Assert.That(
                storage.TryRead(
                    GameSaveService.AutosaveSlotId,
                    out GameSaveData data),
                Is.EqualTo(SaveFileReadStatus.Success));
            Assert.That(
                data.location.sceneName,
                Is.EqualTo("SuncrestWhisperMarketScene"));
            Assert.That(data.location.spawnId, Is.EqualTo("West"));
        }

        [Test]
        public void ContinueLoadsTheNewestValidSlot()
        {
            GameSession session = StartGame("Angel");
            session.Player.Gold = 10;
            service.RememberLocation("ManualScene", "ManualSpawn");
            service.SaveManual();
            timestamp = "2026-07-23T10:05:00Z";
            session.Player.Gold = 25;
            service.SaveAutosave("AutosaveScene", "AutosaveSpawn");
            GameSessionStore.BeginNewSession().TryStartNewGame("Temporary");

            PlayerContinueStatus status =
                service.Continue(out LocationSaveData location);

            Assert.That(status, Is.EqualTo(PlayerContinueStatus.Success));
            Assert.That(GameSessionStore.Current.Player.Gold, Is.EqualTo(25));
            Assert.That(location.sceneName, Is.EqualTo("AutosaveScene"));
            Assert.That(location.spawnId, Is.EqualTo("AutosaveSpawn"));
        }

        [Test]
        public void CorruptNewestSlotFallsBackToOlderValidSlot()
        {
            StartGame("Angel");
            service.RememberLocation("ManualScene", "ManualSpawn");
            service.SaveManual();
            Directory.CreateDirectory(testDirectory);
            storage.TryGetSavePath(
                GameSaveService.AutosaveSlotId,
                out string autosavePath);
            File.WriteAllText(autosavePath, "{ broken");

            PlayerContinueStatus status =
                service.Continue(out LocationSaveData location);

            Assert.That(status, Is.EqualTo(PlayerContinueStatus.Success));
            Assert.That(location.sceneName, Is.EqualTo("ManualScene"));
        }

        [Test]
        public void ContinueCarriesPlayTimeIntoTheNextSave()
        {
            StartGame("Angel");
            service.RememberLocation("FirstScene", "FirstSpawn");
            clock = 130;
            service.SaveManual();

            clock = 500;
            GameSaveService resumedService = CreateService();
            resumedService.Continue(out _);
            clock = 510;
            resumedService.RememberLocation("SecondScene", "SecondSpawn");
            resumedService.SaveManual();
            storage.TryRead(
                GameSaveService.ManualSlotId,
                out GameSaveData data);

            Assert.That(data.playTimeSeconds, Is.EqualTo(40).Within(0.001));
        }

        [Test]
        public void InvalidSaveIsIgnoredWithoutReplacingCurrentSession()
        {
            Directory.CreateDirectory(testDirectory);
            storage.TryGetSavePath(
                GameSaveService.ManualSlotId,
                out string savePath);
            File.WriteAllText(savePath, "{\"schemaVersion\":1}");
            GameSession current = StartGame("Keep Me");

            Assert.That(service.HasContinue(), Is.False);
            Assert.That(
                service.Continue(out _),
                Is.EqualTo(PlayerContinueStatus.NoSave));
            Assert.That(GameSessionStore.Current, Is.SameAs(current));
        }

        private GameSession StartGame(string playerName)
        {
            GameSession session = GameSessionStore.BeginNewSession();
            session.TryStartNewGame(playerName);
            service.BeginNewGame("look_01");
            return session;
        }

        private GameSaveService CreateService()
        {
            return new GameSaveService(
                storage,
                () => clock,
                () => timestamp,
                () => "0.1.0");
        }
    }
}
