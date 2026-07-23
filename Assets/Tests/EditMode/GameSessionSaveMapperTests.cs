using System;
using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class GameSessionSaveMapperTests
    {
        [Test]
        public void CaptureRequiresAStartedSession()
        {
            var session = new GameSession();

            Assert.Throws<ArgumentException>(() =>
                GameSessionSaveMapper.Capture(session, Context()));
        }

        [Test]
        public void CaptureRequiresAnExplicitSceneAndSpawn()
        {
            GameSession session = CreatePopulatedSession();

            Assert.Throws<ArgumentException>(() =>
                GameSessionSaveMapper.Capture(
                    session,
                    new SaveCaptureContext(
                        "manual_01",
                        "0.1.0",
                        "2026-07-23T12:00:00Z",
                        50,
                        "look_01",
                        "",
                        "")));
        }

        [Test]
        public void CaptureUsesStableCatalogIds()
        {
            GameSession session = CreatePopulatedSession();

            GameSaveData save =
                GameSessionSaveMapper.Capture(session, Context());
            CharacterSaveData protagonist = save.party.characters.Single(
                character => character.characterId ==
                    PlayableCharacterData.ProtagonistId);

            Assert.That(protagonist.currentJobId,
                Is.EqualTo("job_mercenary"));
            Assert.That(protagonist.equipment.Single().slotId,
                Is.EqualTo("slot_weapon"));
            Assert.That(protagonist.equipment.Single().itemId,
                Is.EqualTo(ItemCatalog.IronHeavyBladeId));
            Assert.That(protagonist.jobAffinities.Single().jobId,
                Is.EqualTo("job_mercenary"));
            Assert.That(protagonist.jobAffinities.Single().affinityId,
                Is.EqualTo("affinity_high"));
        }

        [Test]
        public void JsonRoundTripRestoresCurrentRuntimeState()
        {
            GameSession original = CreatePopulatedSession();
            GameSaveData captured =
                GameSessionSaveMapper.Capture(original, Context());
            string json = SaveDataJson.Serialize(captured);
            Assert.That(
                SaveDataJson.TryDeserialize(json, out GameSaveData parsed),
                Is.EqualTo(SaveDataReadStatus.Success));

            GameSessionRestoreStatus status =
                GameSessionSaveMapper.TryRestore(
                    parsed,
                    out GameSession restored,
                    out LocationSaveData location);

            Assert.That(status, Is.EqualTo(GameSessionRestoreStatus.Success));
            Assert.That(restored.Player.Name, Is.EqualTo("Angel"));
            Assert.That(restored.Player.Gold, Is.EqualTo(original.Player.Gold));
            Assert.That(
                restored.Party.Characters.Count,
                Is.EqualTo(original.Party.Characters.Count));
            Assert.That(
                restored.Party.ActiveCharacterIds,
                Is.EqualTo(original.Party.ActiveCharacterIds));
            Assert.That(
                restored.Inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(original.Inventory.GetQuantity(
                    ItemCatalog.MinorPotionId)));
            Assert.That(
                restored.IsEncounterCompleted(
                    BattleEncounterCatalog.GoblinBossId),
                Is.True);
            Assert.That(
                restored.RewardClaims.IsClaimed(
                    DemoEconomyCatalog.Quest1BaseRewardId),
                Is.True);
            Assert.That(restored.CampRestState.TutorialRestUsed, Is.True);
            Assert.That(restored.CampRestState.CompletedRestCount, Is.EqualTo(1));
            Assert.That(location.sceneName,
                Is.EqualTo("SuncrestGuildHallScene"));
            Assert.That(location.spawnId, Is.EqualTo("GuildHallEntrance"));
        }

        [Test]
        public void RestoreKeepsProtagonistAndPlayerProgressionShared()
        {
            GameSaveData save = GameSessionSaveMapper.Capture(
                CreatePopulatedSession(),
                Context());

            GameSessionSaveMapper.TryRestore(
                save,
                out GameSession restored,
                out _);
            PlayableCharacterData protagonist = restored.Party.GetCharacter(
                PlayableCharacterData.ProtagonistId);

            Assert.That(protagonist.Stats, Is.SameAs(restored.Player.Stats));
            int previousLevel = restored.Player.Level;
            protagonist.GainXP(protagonist.XPToNextLevel);
            Assert.That(restored.Player.Level, Is.EqualTo(previousLevel + 1));
        }

        [Test]
        public void RestoreClearsTransientBattleState()
        {
            GameSaveData save = GameSessionSaveMapper.Capture(
                CreatePopulatedSession(),
                Context());

            GameSessionSaveMapper.TryRestore(
                save,
                out GameSession restored,
                out _);

            Assert.That(restored.HasActiveBattle, Is.False);
            Assert.That(restored.Monster, Is.Null);
            Assert.That(restored.PartyBattle, Is.Null);
            Assert.That(restored.BattleIsOver, Is.True);
            Assert.That(restored.BattleOutcome, Is.EqualTo(BattleOutcome.None));
        }

        [Test]
        public void RestoreDoesNotDuplicateEquipmentIntoInventory()
        {
            GameSaveData save = GameSessionSaveMapper.Capture(
                CreatePopulatedSession(),
                Context());

            GameSessionSaveMapper.TryRestore(
                save,
                out GameSession restored,
                out _);
            PlayableCharacterData protagonist = restored.Party.GetCharacter(
                PlayableCharacterData.ProtagonistId);

            Assert.That(
                protagonist.Equipment.GetItemId(EquipmentSlot.Weapon),
                Is.EqualTo(ItemCatalog.IronHeavyBladeId));
            Assert.That(
                restored.Inventory.GetQuantity(ItemCatalog.IronHeavyBladeId),
                Is.Zero);
        }

        [Test]
        public void RecalculationAfterRestoreDoesNotStackBonuses()
        {
            GameSession original = CreatePopulatedSession();
            PlayableCharacterData originalProtagonist =
                original.Party.GetCharacter(PlayableCharacterData.ProtagonistId);
            int expectedAttack = originalProtagonist.Stats.Attack;
            int expectedMaxHp = originalProtagonist.Stats.MaxHp;
            GameSaveData save =
                GameSessionSaveMapper.Capture(original, Context());
            GameSessionSaveMapper.TryRestore(
                save,
                out GameSession restored,
                out _);
            PlayableCharacterData protagonist = restored.Party.GetCharacter(
                PlayableCharacterData.ProtagonistId);

            protagonist.SetJobAffinity(JobId.Mercenary, JobAffinity.Neutral);
            protagonist.SetJobAffinity(JobId.Mercenary, JobAffinity.High);

            Assert.That(protagonist.Stats.Attack, Is.EqualTo(expectedAttack));
            Assert.That(protagonist.Stats.MaxHp, Is.EqualTo(expectedMaxHp));
        }

        [Test]
        public void InvalidCatalogReferenceFailsWithoutReturningSession()
        {
            GameSaveData save = GameSessionSaveMapper.Capture(
                CreatePopulatedSession(),
                Context());
            save.inventory.entries.Add(new InventoryEntrySaveData
            {
                itemId = "item_missing",
                quantity = 1
            });

            GameSessionRestoreStatus status =
                GameSessionSaveMapper.TryRestore(
                    save,
                    out GameSession restored,
                    out LocationSaveData location);

            Assert.That(status,
                Is.EqualTo(GameSessionRestoreStatus.InvalidSave));
            Assert.That(restored, Is.Null);
            Assert.That(location, Is.Null);
        }

        [Test]
        public void FutureQuestContentIsRejectedUntilRuntimeSupportExists()
        {
            GameSaveData save = GameSessionSaveMapper.Capture(
                CreatePopulatedSession(),
                Context());
            save.quests.entries.Add(new QuestEntrySaveData
            {
                questId = "quest_future",
                stateId = "active"
            });

            GameSessionRestoreStatus status =
                GameSessionSaveMapper.TryRestore(
                    save,
                    out GameSession restored,
                    out _);

            Assert.That(status,
                Is.EqualTo(GameSessionRestoreStatus.UnsupportedContent));
            Assert.That(restored, Is.Null);
        }

        private static GameSession CreatePopulatedSession()
        {
            var session = new GameSession();
            session.TryStartNewGame("Angel");
            PlayableCharacterData protagonist = session.Party.GetCharacter(
                PlayableCharacterData.ProtagonistId);
            PlayableCharacterData iona =
                PartyMemberCatalog.Get("pc_01").CreateCharacter();
            session.Party.TryAddCharacter(iona);
            session.Party.TrySetActiveParty(new[]
            {
                protagonist.Id,
                iona.Id
            });
            session.Party.TryAddBondPoints(protagonist.Id, iona.Id, 3);

            protagonist.SetJobAffinity(
                JobId.Mercenary,
                JobAffinity.High);
            JobNodeDefinition node = JobNodeCatalog
                .GetForJob(JobId.Mercenary)
                .First(definition =>
                    definition.PrerequisiteIds.Count == 0);
            protagonist.TryAddJobPoints(node.Cost + 5);
            protagonist.TryPurchaseJobNode(node.StableId);

            session.Inventory.TryAdd(ItemCatalog.IronHeavyBladeId);
            protagonist.TryEquipItem(
                EquipmentSlot.Weapon,
                ItemCatalog.IronHeavyBladeId,
                session.Inventory);
            session.Inventory.TryAdd(ItemCatalog.MinorPotionId, 2);
            session.TryGrantDemoReward(
                DemoEconomyCatalog.Quest1BaseRewardId,
                out _);

            session.StartEncounter(BattleEncounterCatalog.Get(
                BattleEncounterCatalog.GoblinBossId));
            foreach (ICombatant enemy in session.PartyBattle.Enemies)
            {
                enemy.Stats.CurrentHp = 0;
            }

            session.TryCompleteVictory(out _);
            protagonist.Stats.CurrentHp -= 10;
            new CampRestService(
                session.Party,
                session.Inventory,
                session.CampRestState).TryFullRest(
                    confirmed: true,
                    allowFreeTutorialRest: true);
            return session;
        }

        private static SaveCaptureContext Context()
        {
            return new SaveCaptureContext(
                "manual_01",
                "0.1.0",
                "2026-07-23T12:00:00Z",
                1234.5,
                "look_01",
                "SuncrestGuildHallScene",
                "GuildHallEntrance");
        }
    }
}
