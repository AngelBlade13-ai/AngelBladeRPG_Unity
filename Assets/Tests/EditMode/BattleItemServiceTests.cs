using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class BattleItemServiceTests
    {
        [Test]
        public void OnlyOwnedHpConsumablesAreBattleUsable()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId);
            inventory.TryAdd(ItemCatalog.FieldRemedyId);
            inventory.TryAdd(ItemCatalog.CampRationId);

            var items = BattleItemService.GetUsableItems(inventory);

            Assert.That(items, Has.Count.EqualTo(1));
            Assert.That(items[0].Id, Is.EqualTo(ItemCatalog.MinorPotionId));
        }

        [Test]
        public void PotionHealsTargetAndConsumesExactlyOne()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId, 2);
            PlayableCharacterData actor = Member("actor");
            PlayableCharacterData target = Member("target");
            target.Stats.CurrentHp = 25;

            CombatActionResult result = new BattleItemService(inventory).Use(
                actor,
                target,
                ItemCatalog.MinorPotionId);

            Assert.That(result.Type, Is.EqualTo(CombatActionType.Item));
            Assert.That(result.Healing, Is.EqualTo(40));
            Assert.That(target.Stats.CurrentHp, Is.EqualTo(65));
            Assert.That(
                inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(1));
        }

        [Test]
        public void FullHealthTargetIsRejectedWithoutConsumption()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId);
            var service = new BattleItemService(inventory);

            Assert.That(
                () => service.Use(
                    Member("actor"),
                    Member("target"),
                    ItemCatalog.MinorPotionId),
                Throws.TypeOf<System.ArgumentException>());
            Assert.That(
                inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(1));
        }

        [Test]
        public void EnemyTargetIsRejectedWithoutConsumption()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId);
            var service = new BattleItemService(inventory);

            Assert.That(
                () => service.Use(
                    Member("actor"),
                    Enemy(),
                    ItemCatalog.MinorPotionId),
                Throws.TypeOf<System.ArgumentException>());
            Assert.That(
                inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(1));
        }

        [Test]
        public void IncapacitatedAllyIsNotABattleItemTarget()
        {
            PlayableCharacterData target = Member("target");
            target.Stats.CurrentHp = 0;

            Assert.That(
                BattleItemService.CanTarget(
                    ItemCatalog.Get(ItemCatalog.MinorPotionId),
                    target),
                Is.False);
        }

        private static PlayableCharacterData Member(string id)
        {
            return new PlayableCharacterData(id, id, JobId.Mercenary);
        }

        private static MonsterData Enemy()
        {
            return new MonsterData(
                "enemy", "Goblin", 30, 8, 1, 0, 0, 8, 0, 0, 0);
        }
    }
}
