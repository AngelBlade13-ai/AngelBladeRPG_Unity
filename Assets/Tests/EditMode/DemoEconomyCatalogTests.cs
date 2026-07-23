using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class DemoEconomyCatalogTests
    {
        [Test]
        public void RewardCatalogContainsTwelveUniqueDefinitions()
        {
            DemoRewardDefinition[] rewards =
                DemoEconomyCatalog.Rewards.ToArray();

            Assert.That(rewards, Has.Length.EqualTo(12));
            Assert.That(
                rewards.Select(reward => reward.Id).Distinct().Count(),
                Is.EqualTo(rewards.Length));
            Assert.That(
                rewards.All(reward =>
                    DemoEconomyCatalog.GetReward(reward.Id) == reward),
                Is.True);
        }

        [Test]
        public void EveryRewardItemUsesAValidStackQuantity()
        {
            foreach (DemoRewardDefinition reward in
                DemoEconomyCatalog.Rewards)
            {
                foreach (ItemGrant itemGrant in reward.Items)
                {
                    ItemDefinition item =
                        ItemCatalog.Get(itemGrant.ItemId);

                    Assert.That(item, Is.Not.Null, reward.Id);
                    Assert.That(
                        itemGrant.Quantity,
                        Is.InRange(1, item.MaximumStack),
                        reward.Id);
                }
            }
        }

        [Test]
        public void LootTablesAreOptionalAndUseValidPercentages()
        {
            MonsterLootTable[] tables =
                DemoEconomyCatalog.LootTables.ToArray();

            Assert.That(tables, Has.Length.EqualTo(6));
            Assert.That(
                tables.Select(table => table.MonsterId).Distinct().Count(),
                Is.EqualTo(tables.Length));
            foreach (MonsterLootTable table in tables)
            {
                Assert.That(MonsterCatalog.Get(table.MonsterId), Is.Not.Null);
                Assert.That(table.Entries, Is.Not.Empty);
                foreach (MonsterLootEntry entry in table.Entries)
                {
                    Assert.That(ItemCatalog.Get(entry.ItemId), Is.Not.Null);
                    Assert.That(entry.ChancePercent, Is.InRange(1, 100));
                    Assert.That(entry.Quantity, Is.GreaterThan(0));
                }
            }

            Assert.That(
                DemoEconomyCatalog.GetLootTable(
                    "boss_grassland_goblin"),
                Is.Null);
        }

        [Test]
        public void MinimumCriticalPathGuaranteesSixHundredTwentyNineGold()
        {
            Assert.That(
                DemoEconomyCatalog.CalculateMinimumCriticalPathGold(),
                Is.EqualTo(629));
        }

        [Test]
        public void GuaranteedGoldCoversRecommendedSpendWithoutRandomDrops()
        {
            int guaranteedGold =
                DemoEconomyCatalog.CalculateMinimumCriticalPathGold();
            int recommendedSpend =
                DemoEconomyCatalog.CalculateRecommendedCriticalPathSpend();

            Assert.That(recommendedSpend, Is.EqualTo(595));
            Assert.That(guaranteedGold, Is.GreaterThanOrEqualTo(recommendedSpend));
        }

        [Test]
        public void TutorialAndGoblinBossGoldIncludeTheirFullAuthoredGroups()
        {
            Assert.That(
                DemoEconomyCatalog.CalculateEncounterGold(
                    BattleEncounterCatalog.CaravanTutorialId),
                Is.EqualTo(41));
            Assert.That(
                DemoEconomyCatalog.CalculateEncounterGold(
                    BattleEncounterCatalog.GoblinBossId),
                Is.EqualTo(104));
        }

        [Test]
        public void NamedQuestEquipmentUsesProvisionalSupportiveStats()
        {
            ItemDefinition marlow =
                ItemCatalog.Get(ItemCatalog.MarlowsTradeCharmId);
            ItemDefinition fieldguard =
                ItemCatalog.Get(ItemCatalog.IronforgeFieldguardId);
            ItemDefinition insignia =
                ItemCatalog.Get(ItemCatalog.SuncrestWatchInsigniaId);
            ItemDefinition cord =
                ItemCatalog.Get(ItemCatalog.NomadsWovenCordId);

            Assert.That(marlow.StatBonuses.Accuracy, Is.EqualTo(2));
            Assert.That(marlow.StatBonuses.Evasion, Is.EqualTo(2));
            Assert.That(fieldguard.StatBonuses.MaxHp, Is.EqualTo(10));
            Assert.That(fieldguard.StatBonuses.Defense, Is.EqualTo(2));
            Assert.That(insignia.StatBonuses.Defense, Is.EqualTo(1));
            Assert.That(insignia.StatBonuses.Speed, Is.EqualTo(1));
            Assert.That(cord.StatBonuses.Speed, Is.EqualTo(2));
            Assert.That(cord.StatBonuses.Evasion, Is.EqualTo(2));
        }

        [Test]
        public void TrailKnifeEvidenceCannotBeSoldOrDiscarded()
        {
            ItemDefinition evidence =
                ItemCatalog.Get(ItemCatalog.TrailKnifeFragmentId);

            Assert.That(evidence.Kind, Is.EqualTo(ItemKind.KeyItem));
            Assert.That(evidence.CanSell, Is.False);
            Assert.That(evidence.CanDiscard, Is.False);
        }

        [Test]
        public void RewardServiceGrantsGoldAndItemsAtomically()
        {
            var player = new PlayerData("Angel");
            var inventory = new Inventory();
            var claims = new DemoRewardClaimState();

            DemoRewardStatus status = DemoRewardService.TryGrant(
                DemoEconomyCatalog.Quest3BaseRewardId,
                player,
                inventory,
                claims,
                out DemoRewardGrantResult result);

            Assert.That(status, Is.EqualTo(DemoRewardStatus.Granted));
            Assert.That(result.Gold, Is.EqualTo(120));
            Assert.That(player.Gold, Is.EqualTo(120));
            Assert.That(
                inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(2));
            Assert.That(
                inventory.GetQuantity(ItemCatalog.FieldRemedyId),
                Is.EqualTo(1));
            Assert.That(
                inventory.GetQuantity(ItemCatalog.CampRationId),
                Is.EqualTo(1));
        }

        [Test]
        public void RewardServiceGrantsEachStableRewardOnlyOnce()
        {
            var session = new GameSession();
            session.TryStartNewGame("Angel");

            DemoRewardStatus first = session.TryGrantDemoReward(
                DemoEconomyCatalog.Quest2RewardId,
                out _);
            DemoRewardStatus duplicate = session.TryGrantDemoReward(
                DemoEconomyCatalog.Quest2RewardId,
                out DemoRewardGrantResult duplicateResult);

            Assert.That(first, Is.EqualTo(DemoRewardStatus.Granted));
            Assert.That(
                duplicate,
                Is.EqualTo(DemoRewardStatus.AlreadyGranted));
            Assert.That(duplicateResult, Is.Null);
            Assert.That(session.Player.Gold, Is.EqualTo(90));
            Assert.That(
                session.Inventory.GetQuantity(
                    ItemCatalog.IronforgeFieldguardId),
                Is.EqualTo(1));
        }

        [Test]
        public void FullInventoryRejectsRewardWithoutGrantingPartialGold()
        {
            var player = new PlayerData("Angel") { Gold = 5 };
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId, 98);
            var claims = new DemoRewardClaimState();

            DemoRewardStatus status = DemoRewardService.TryGrant(
                DemoEconomyCatalog.Quest1BaseRewardId,
                player,
                inventory,
                claims,
                out DemoRewardGrantResult result);

            Assert.That(status, Is.EqualTo(DemoRewardStatus.InventoryFull));
            Assert.That(result, Is.Null);
            Assert.That(player.Gold, Is.EqualTo(5));
            Assert.That(
                inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(98));
            Assert.That(
                claims.IsClaimed(DemoEconomyCatalog.Quest1BaseRewardId),
                Is.False);
        }

        [Test]
        public void InvalidRewardRequestsDoNotMutateState()
        {
            var player = new PlayerData("Angel");
            var inventory = new Inventory();
            var claims = new DemoRewardClaimState();

            DemoRewardStatus unknown = DemoRewardService.TryGrant(
                "reward_unknown",
                player,
                inventory,
                claims,
                out _);
            DemoRewardStatus invalid = DemoRewardService.TryGrant(
                DemoEconomyCatalog.Quest1BaseRewardId,
                null,
                inventory,
                claims,
                out _);

            Assert.That(unknown, Is.EqualTo(DemoRewardStatus.UnknownReward));
            Assert.That(invalid, Is.EqualTo(DemoRewardStatus.InvalidState));
            Assert.That(player.Gold, Is.Zero);
            Assert.That(inventory.Quantities, Is.Empty);
        }

        [Test]
        public void EconomyDefinitionsRejectInvalidData()
        {
            Assert.Throws<ArgumentException>(
                () => new ItemGrant("item_unknown", 1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new ItemGrant(ItemCatalog.MinorPotionId, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new DemoRewardDefinition("reward_test", -1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new MonsterLootEntry(
                    ItemCatalog.MinorPotionId,
                    101));
            Assert.Throws<ArgumentException>(
                () => new MonsterLootTable(
                    "monster_unknown",
                    Array.Empty<MonsterLootEntry>()));
        }

        [Test]
        public void OptionalObjectiveRewardsAreNotRequiredByCriticalPathBudget()
        {
            var criticalIds = new HashSet<string>(
                DemoEconomyCatalog.CriticalPathRewardIds,
                StringComparer.Ordinal);

            Assert.That(
                criticalIds.Contains(
                    DemoEconomyCatalog.Quest1FullClearRewardId),
                Is.False);
            Assert.That(
                criticalIds.Contains(
                    DemoEconomyCatalog.Quest3FullRescueRewardId),
                Is.False);
            Assert.That(
                criticalIds.Contains(
                    DemoEconomyCatalog.PaintersViewRewardId),
                Is.False);
        }

        [Test]
        public void ProvisionalQuestGoldMatchesThePacingContract()
        {
            Assert.That(
                DemoEconomyCatalog.GetReward(
                    DemoEconomyCatalog.Quest1BaseRewardId).Gold,
                Is.EqualTo(60));
            Assert.That(
                DemoEconomyCatalog.GetReward(
                    DemoEconomyCatalog.Quest2RewardId).Gold,
                Is.EqualTo(90));
            Assert.That(
                DemoEconomyCatalog.GetReward(
                    DemoEconomyCatalog.Quest3BaseRewardId).Gold,
                Is.EqualTo(120));
            Assert.That(
                DemoEconomyCatalog.GetReward(
                    DemoEconomyCatalog.GoblinBossReturnRewardId).Gold,
                Is.EqualTo(120));
        }
    }
}
