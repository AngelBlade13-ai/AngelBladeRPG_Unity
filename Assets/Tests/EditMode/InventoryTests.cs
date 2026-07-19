using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class InventoryTests
    {
        [Test]
        public void InventoryAddsAndRemovesKnownItems()
        {
            var inventory = new Inventory();

            Assert.That(
                inventory.TryAdd(ItemCatalog.MinorPotionId, 3),
                Is.True);
            Assert.That(
                inventory.TryRemove(ItemCatalog.MinorPotionId, 2),
                Is.True);
            Assert.That(
                inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(1));
        }

        [Test]
        public void InventoryRejectsUnknownInvalidAndOverStackedChanges()
        {
            var inventory = new Inventory();

            Assert.That(inventory.TryAdd("missing", 1), Is.False);
            Assert.That(
                inventory.TryAdd(ItemCatalog.MinorPotionId, 99),
                Is.True);
            Assert.That(
                inventory.TryAdd(ItemCatalog.MinorPotionId, 1),
                Is.False);
            Assert.That(
                inventory.TryRemove(ItemCatalog.MinorPotionId, 100),
                Is.False);
            Assert.That(
                inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(99));
        }

        [Test]
        public void FailedInventoryChangesAreAtomic()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.CampRationId, 2);

            inventory.TryRemove(ItemCatalog.CampRationId, 3);
            inventory.TryAdd(ItemCatalog.CampRationId, 98);

            Assert.That(
                inventory.GetQuantity(ItemCatalog.CampRationId),
                Is.EqualTo(2));
        }

        [Test]
        public void DiscardUsesTheItemPermission()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId);
            inventory.TryAdd(ItemCatalog.EnoraScytheId);

            Assert.That(
                inventory.TryDiscard(ItemCatalog.MinorPotionId),
                Is.True);
            Assert.That(
                inventory.TryDiscard(ItemCatalog.EnoraScytheId),
                Is.False);
            Assert.That(
                inventory.GetQuantity(ItemCatalog.EnoraScytheId),
                Is.EqualTo(1));
        }
    }
}
