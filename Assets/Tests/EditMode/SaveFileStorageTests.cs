using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace AngelBladeRPG.Tests
{
    public class SaveFileStorageTests
    {
        private string testDirectory;
        private SaveFileStorage storage;

        [SetUp]
        public void SetUp()
        {
            testDirectory = Path.Combine(
                Path.GetTempPath(),
                "PetalsInTheDuskTests",
                Guid.NewGuid().ToString("N"));
            storage = new SaveFileStorage(testDirectory);
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
        public void DefaultStorageUsesApplicationDataSaveFolder()
        {
            SaveFileStorage defaultStorage =
                SaveFileStorage.CreateDefault();

            Assert.That(
                defaultStorage.RootDirectory,
                Is.EqualTo(Path.GetFullPath(Path.Combine(
                    Application.persistentDataPath,
                    "Saves"))));
        }

        [Test]
        public void InvalidSlotCannotEscapeTheSaveDirectory()
        {
            Assert.That(
                storage.TryWrite("../outside", ValidData("Angel")),
                Is.EqualTo(SaveFileWriteStatus.InvalidSlot));
            Assert.That(
                storage.TryRead("../outside", out _),
                Is.EqualTo(SaveFileReadStatus.InvalidSlot));
            Assert.That(
                storage.TryGetSavePath("../outside", out _),
                Is.False);
        }

        [Test]
        public void SuccessfulWriteCreatesJsonAndRemovesTemporaryFile()
        {
            SaveFileWriteStatus status =
                storage.TryWrite("manual_01", ValidData("Angel"));
            storage.TryGetSavePath("manual_01", out string savePath);

            Assert.That(status, Is.EqualTo(SaveFileWriteStatus.Success));
            Assert.That(File.Exists(savePath), Is.True);
            Assert.That(File.Exists(savePath + ".tmp"), Is.False);
            Assert.That(
                storage.TryRead("manual_01", out GameSaveData loaded),
                Is.EqualTo(SaveFileReadStatus.Success));
            Assert.That(loaded.player.name, Is.EqualTo("Angel"));
        }

        [Test]
        public void OverwriteKeepsPreviousFileAsBackup()
        {
            storage.TryWrite("manual_01", ValidData("First"));
            storage.TryWrite("manual_01", ValidData("Second"));
            storage.TryWrite("manual_01", ValidData("Third"));
            storage.TryGetSavePath("manual_01", out string savePath);

            Assert.That(File.Exists(savePath + ".bak"), Is.True);
            Assert.That(
                storage.TryRead("manual_01", out GameSaveData current),
                Is.EqualTo(SaveFileReadStatus.Success));
            Assert.That(current.player.name, Is.EqualTo("Third"));

            string backupJson = File.ReadAllText(savePath + ".bak");
            Assert.That(
                SaveDataJson.TryDeserialize(
                    backupJson,
                    out GameSaveData backup),
                Is.EqualTo(SaveDataReadStatus.Success));
            Assert.That(backup.player.name, Is.EqualTo("Second"));
        }

        [Test]
        public void MissingSlotIsReportedWithoutCreatingFiles()
        {
            SaveFileReadStatus status =
                storage.TryRead("manual_01", out GameSaveData loaded);

            Assert.That(status, Is.EqualTo(SaveFileReadStatus.Missing));
            Assert.That(loaded, Is.Null);
            Assert.That(Directory.Exists(testDirectory), Is.False);
        }

        [Test]
        public void CorruptPrimaryRecoversThePreviousBackup()
        {
            storage.TryWrite("manual_01", ValidData("First"));
            storage.TryWrite("manual_01", ValidData("Second"));
            storage.TryGetSavePath("manual_01", out string savePath);
            File.WriteAllText(savePath, "{ broken");

            SaveFileReadStatus status =
                storage.TryRead("manual_01", out GameSaveData loaded);

            Assert.That(status,
                Is.EqualTo(SaveFileReadStatus.RecoveredBackup));
            Assert.That(loaded.player.name, Is.EqualTo("First"));
        }

        [Test]
        public void CorruptSaveWithoutBackupIsReported()
        {
            Directory.CreateDirectory(testDirectory);
            storage.TryGetSavePath("manual_01", out string savePath);
            File.WriteAllText(savePath, "{ broken");

            SaveFileReadStatus status =
                storage.TryRead("manual_01", out GameSaveData loaded);

            Assert.That(status, Is.EqualTo(SaveFileReadStatus.Corrupt));
            Assert.That(loaded, Is.Null);
        }

        [Test]
        public void FutureSchemaIsNotMistakenForCorruption()
        {
            Directory.CreateDirectory(testDirectory);
            storage.TryGetSavePath("manual_01", out string savePath);
            File.WriteAllText(savePath, "{\"schemaVersion\":999}");

            SaveFileReadStatus status =
                storage.TryRead("manual_01", out GameSaveData loaded);

            Assert.That(
                status,
                Is.EqualTo(SaveFileReadStatus.UnsupportedVersion));
            Assert.That(loaded, Is.Null);
        }

        [Test]
        public void CoordinatorSavesAndLoadsGameSessionStore()
        {
            GameSession original = GameSessionStore.BeginNewSession();
            original.TryStartNewGame("Angel");
            original.Player.Gold = 87;
            original.Inventory.TryAdd(ItemCatalog.MinorPotionId, 2);
            var coordinator = new GameSaveCoordinator(storage);

            Assert.That(
                coordinator.SaveCurrent("manual_01", Context()),
                Is.EqualTo(GameSaveStatus.Success));
            GameSession replacement = GameSessionStore.BeginNewSession();
            replacement.TryStartNewGame("Temporary");

            GameLoadStatus status = coordinator.LoadIntoCurrent(
                "manual_01",
                out LocationSaveData location);

            Assert.That(status, Is.EqualTo(GameLoadStatus.Success));
            Assert.That(GameSessionStore.Current, Is.Not.SameAs(replacement));
            Assert.That(GameSessionStore.Current.Player.Name,
                Is.EqualTo("Angel"));
            Assert.That(GameSessionStore.Current.Player.Gold, Is.EqualTo(87));
            Assert.That(
                GameSessionStore.Current.Inventory.GetQuantity(
                    ItemCatalog.MinorPotionId),
                Is.EqualTo(2));
            Assert.That(location.sceneName,
                Is.EqualTo("SuncrestGuildHallScene"));
            Assert.That(location.spawnId, Is.EqualTo("GuildHallEntrance"));
        }

        [Test]
        public void InvalidSaveNeverReplacesCurrentSession()
        {
            storage.TryWrite("manual_01", ValidData("Broken"));
            GameSession current = GameSessionStore.BeginNewSession();
            current.TryStartNewGame("Keep Me");
            var coordinator = new GameSaveCoordinator(storage);

            GameLoadStatus status = coordinator.LoadIntoCurrent(
                "manual_01",
                out LocationSaveData location);

            Assert.That(status, Is.EqualTo(GameLoadStatus.InvalidData));
            Assert.That(GameSessionStore.Current, Is.SameAs(current));
            Assert.That(GameSessionStore.Current.Player.Name,
                Is.EqualTo("Keep Me"));
            Assert.That(location, Is.Null);
        }

        private static GameSaveData ValidData(string playerName)
        {
            return new GameSaveData
            {
                player = new PlayerSaveData
                {
                    name = playerName
                },
                location = new LocationSaveData
                {
                    sceneName = "SuncrestGuildHallScene",
                    spawnId = "GuildHallEntrance"
                }
            };
        }

        private static SaveCaptureContext Context()
        {
            return new SaveCaptureContext(
                "manual_01",
                "0.1.0",
                "2026-07-23T12:00:00Z",
                120,
                "look_01",
                "SuncrestGuildHallScene",
                "GuildHallEntrance");
        }
    }
}
