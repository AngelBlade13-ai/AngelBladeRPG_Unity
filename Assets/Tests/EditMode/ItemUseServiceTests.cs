using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class ItemUseServiceTests
    {
        [Test]
        public void PotionRestoresHpAndConsumesOneItem()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId, 2);
            PlayableCharacterData target = CreateCharacter();
            target.Stats.ApplyDamage(60);

            ItemUseResult result =
                new ItemUseService(inventory).TryUse(
                    ItemCatalog.MinorPotionId,
                    target);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.RestoredHp, Is.EqualTo(40));
            Assert.That(target.Stats.CurrentHp, Is.EqualTo(80));
            Assert.That(
                inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(1));
        }

        [Test]
        public void PotionAtFullHpDoesNotConsumeTheItem()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId);

            ItemUseResult result = new ItemUseService(inventory).TryUse(
                ItemCatalog.MinorPotionId,
                CreateCharacter());

            Assert.That(result.Status, Is.EqualTo(ItemUseStatus.NoEffect));
            Assert.That(
                inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(1));
        }

        [Test]
        public void PotionDoesNotReviveAnIncapacitatedCharacter()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId);
            PlayableCharacterData target = CreateCharacter();
            target.Stats.CurrentHp = 0;

            ItemUseResult result = new ItemUseService(inventory).TryUse(
                ItemCatalog.MinorPotionId,
                target);

            Assert.That(result.Status, Is.EqualTo(ItemUseStatus.InvalidTarget));
            Assert.That(target.Stats.CurrentHp, Is.Zero);
            Assert.That(
                inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(1));
        }

        [Test]
        public void CampRationCannotBeSpentThroughNormalItemUse()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.CampRationId);
            PlayableCharacterData target = CreateCharacter();
            target.Stats.ApplyDamage(10);

            ItemUseResult result = new ItemUseService(inventory).TryUse(
                ItemCatalog.CampRationId,
                target);

            Assert.That(
                result.Status,
                Is.EqualTo(ItemUseStatus.RequiresRestService));
            Assert.That(
                inventory.GetQuantity(ItemCatalog.CampRationId),
                Is.EqualTo(1));
        }

        [Test]
        public void UnsupportedRemedyEffectDoesNotConsumeTheItem()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.FieldRemedyId);

            ItemUseResult result = new ItemUseService(inventory).TryUse(
                ItemCatalog.FieldRemedyId,
                CreateCharacter());

            Assert.That(
                result.Status,
                Is.EqualTo(ItemUseStatus.UnsupportedEffect));
            Assert.That(
                inventory.GetQuantity(ItemCatalog.FieldRemedyId),
                Is.EqualTo(1));
        }

        private static PlayableCharacterData CreateCharacter()
        {
            return new PlayableCharacterData("hero", "Angel", JobId.Mercenary);
        }
    }
}
