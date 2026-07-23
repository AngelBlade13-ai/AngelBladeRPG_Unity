using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace AngelBladeRPG.Tests
{
    public class SaveDataJsonTests
    {
        [Test]
        public void NewSaveUsesCurrentSchemaAndInitializedSections()
        {
            var save = new GameSaveData();

            Assert.That(save.schemaVersion, Is.EqualTo(SaveSchema.CurrentVersion));
            Assert.That(save.player, Is.Not.Null);
            Assert.That(save.party.characters, Is.Empty);
            Assert.That(save.inventory.entries, Is.Empty);
            Assert.That(save.quests.entries, Is.Empty);
            Assert.That(save.world.flags, Is.Empty);
            Assert.That(save.camp.consumedEventIds, Is.Empty);
            Assert.That(save.location, Is.Not.Null);
        }

        [Test]
        public void RoundTripPreservesEveryTopLevelSaveArea()
        {
            GameSaveData source = CreatePopulatedSave();

            string json = SaveDataJson.Serialize(source, true);
            SaveDataReadStatus status =
                SaveDataJson.TryDeserialize(json, out GameSaveData loaded);

            Assert.That(status, Is.EqualTo(SaveDataReadStatus.Success));
            Assert.That(loaded.player.name, Is.EqualTo("Angel"));
            Assert.That(loaded.player.appearanceId, Is.EqualTo("look_02"));
            Assert.That(loaded.player.gold, Is.EqualTo(321));
            Assert.That(loaded.party.characters[0].characterId,
                Is.EqualTo(PlayableCharacterData.ProtagonistId));
            Assert.That(loaded.party.characters[0].currentJobId,
                Is.EqualTo("Mercenary"));
            Assert.That(loaded.party.characters[0].stats.currentHp,
                Is.EqualTo(73));
            Assert.That(loaded.party.characters[0].learnedJobNodeIds,
                Does.Contain("mercenary_t1_power"));
            Assert.That(loaded.party.characters[0].equipment[0].itemId,
                Is.EqualTo(ItemCatalog.IronHeavyBladeId));
            Assert.That(loaded.party.activeCharacterIds,
                Does.Contain(PlayableCharacterData.ProtagonistId));
            Assert.That(loaded.inventory.entries[0].quantity, Is.EqualTo(4));
            Assert.That(loaded.quests.entries[0].stateId,
                Is.EqualTo("active"));
            Assert.That(loaded.world.completedEncounterIds,
                Does.Contain(BattleEncounterCatalog.CaravanTutorialId));
            Assert.That(loaded.world.claimedRewardIds,
                Does.Contain(DemoEconomyCatalog.Quest1BaseRewardId));
            Assert.That(loaded.camp.consumedEventIds,
                Does.Contain("camp_first_fire"));
            Assert.That(loaded.camp.returnLocation.sceneName,
                Is.EqualTo("SuncrestGuildHallScene"));
            Assert.That(loaded.location.spawnId, Is.EqualTo("GuildHallEntrance"));
        }

        [Test]
        public void SerializationRejectsNullOrIncompleteData()
        {
            Assert.Throws<ArgumentException>(
                () => SaveDataJson.Serialize(null));

            var incomplete = new GameSaveData
            {
                party = null
            };
            Assert.Throws<ArgumentException>(
                () => SaveDataJson.Serialize(incomplete));
        }

        [Test]
        public void SerializationRejectsUnsupportedSchema()
        {
            var save = new GameSaveData
            {
                schemaVersion = SaveSchema.CurrentVersion + 1
            };

            Assert.Throws<NotSupportedException>(
                () => SaveDataJson.Serialize(save));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void EmptyInputIsReportedWithoutThrowing(string json)
        {
            SaveDataReadStatus status =
                SaveDataJson.TryDeserialize(json, out GameSaveData loaded);

            Assert.That(status, Is.EqualTo(SaveDataReadStatus.Empty));
            Assert.That(loaded, Is.Null);
        }

        [Test]
        public void MalformedJsonIsReportedWithoutThrowing()
        {
            SaveDataReadStatus status = SaveDataJson.TryDeserialize(
                "{ definitely-not-json",
                out GameSaveData loaded);

            Assert.That(status, Is.EqualTo(SaveDataReadStatus.Malformed));
            Assert.That(loaded, Is.Null);
        }

        [Test]
        public void UnsupportedSchemaIsReportedBeforeRuntimeLoading()
        {
            SaveDataReadStatus status = SaveDataJson.TryDeserialize(
                "{\"schemaVersion\":999}",
                out GameSaveData loaded);

            Assert.That(
                status,
                Is.EqualTo(SaveDataReadStatus.UnsupportedVersion));
            Assert.That(loaded, Is.Null);
        }

        [Test]
        public void SaveRecordsDoNotReferenceSceneObjects()
        {
            Type[] recordTypes =
            {
                typeof(GameSaveData),
                typeof(PlayerSaveData),
                typeof(PartySaveData),
                typeof(CharacterSaveData),
                typeof(CharacterStatsSaveData),
                typeof(JobAffinitySaveData),
                typeof(EquipmentSaveData),
                typeof(RosterHistorySaveData),
                typeof(BondSaveData),
                typeof(InventorySaveData),
                typeof(InventoryEntrySaveData),
                typeof(QuestSaveData),
                typeof(QuestEntrySaveData),
                typeof(WorldSaveData),
                typeof(WorldFlagSaveData),
                typeof(CampSaveData),
                typeof(LocationSaveData)
            };

            foreach (Type recordType in recordTypes)
            {
                Assert.That(
                    typeof(UnityEngine.Object).IsAssignableFrom(recordType),
                    Is.False,
                    recordType.Name);

                foreach (FieldInfo field in recordType.GetFields(
                    BindingFlags.Instance | BindingFlags.Public))
                {
                    Assert.That(
                        typeof(UnityEngine.Object).IsAssignableFrom(
                            field.FieldType),
                        Is.False,
                        $"{recordType.Name}.{field.Name}");
                }
            }
        }

        private static GameSaveData CreatePopulatedSave()
        {
            var character = new CharacterSaveData
            {
                characterId = PlayableCharacterData.ProtagonistId,
                displayName = "Angel",
                currentJobId = "Mercenary",
                level = 3,
                xp = 12,
                xpToNextLevel = 100,
                jobPoints = 7,
                stats = new CharacterStatsSaveData
                {
                    maxHp = 140,
                    currentHp = 73,
                    attack = 19,
                    defense = 7,
                    speed = 10,
                    maxMp = 20,
                    currentMp = 14,
                    magicPower = 8,
                    magicDefense = 3,
                    accuracy = 95,
                    evasion = 5,
                    criticalChance = 5
                },
                learnedJobNodeIds = new List<string>
                {
                    "mercenary_t1_power"
                },
                jobAffinities = new List<JobAffinitySaveData>
                {
                    new JobAffinitySaveData
                    {
                        jobId = "Mercenary",
                        affinityId = "High"
                    }
                },
                equipment = new List<EquipmentSaveData>
                {
                    new EquipmentSaveData
                    {
                        slotId = "Weapon",
                        itemId = ItemCatalog.IronHeavyBladeId
                    }
                }
            };
            character.rosterHistory.battlesActive = 3;
            character.rosterHistory.bonds.Add(new BondSaveData
            {
                otherCharacterId = "pc_01",
                points = 2
            });

            var save = new GameSaveData
            {
                saveId = "manual_01",
                gameVersion = "0.1.0",
                savedAtUtc = "2026-07-23T12:00:00Z",
                playTimeSeconds = 1234.5,
                player = new PlayerSaveData
                {
                    name = "Angel",
                    appearanceId = "look_02",
                    gold = 321
                },
                location = new LocationSaveData
                {
                    sceneName = "SuncrestGuildHallScene",
                    spawnId = "GuildHallEntrance"
                }
            };

            save.party.characters.Add(character);
            save.party.activeCharacterIds.Add(character.characterId);
            save.inventory.entries.Add(new InventoryEntrySaveData
            {
                itemId = ItemCatalog.MinorPotionId,
                quantity = 4
            });
            save.quests.entries.Add(new QuestEntrySaveData
            {
                questId = "quest_delayed_caravan",
                stateId = "active",
                objectiveIndex = 1,
                completedObjectiveIds = new List<string>
                {
                    "reach_grassland"
                }
            });
            save.quests.completedQuestCount = 2;
            save.world.completedEncounterIds.Add(
                BattleEncounterCatalog.CaravanTutorialId);
            save.world.claimedRewardIds.Add(
                DemoEconomyCatalog.Quest1BaseRewardId);
            save.world.flags.Add(new WorldFlagSaveData
            {
                flagId = "lysander_recruited",
                value = "false"
            });
            save.camp.activeCampId = "camp_grassland";
            save.camp.tutorialRestUsed = true;
            save.camp.completedRestCount = 1;
            save.camp.consumedEventIds.Add("camp_first_fire");
            save.camp.returnLocation = new LocationSaveData
            {
                sceneName = "SuncrestGuildHallScene",
                spawnId = "GuildHallEntrance"
            };
            return save;
        }
    }
}
