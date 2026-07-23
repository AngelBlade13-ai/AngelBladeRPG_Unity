using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class InventoryEquipmentMenuDataTests
    {
        [Test]
        public void ConsumablesOnlyIncludeOwnedItemsInDisplayOrder()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId);
            inventory.TryAdd(ItemCatalog.CampRationId);
            inventory.TryAdd(ItemCatalog.PaddedArmorId);

            var items = InventoryEquipmentMenuData.GetConsumables(inventory);

            Assert.That(items.Count, Is.EqualTo(2));
            Assert.That(items[0].Id, Is.EqualTo(ItemCatalog.CampRationId));
            Assert.That(items[1].Id, Is.EqualTo(ItemCatalog.MinorPotionId));
        }

        [Test]
        public void EquipmentCandidatesRespectSlotAndCurrentJob()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.IronHeavyBladeId);
            inventory.TryAdd(ItemCatalog.OakStaffId);
            inventory.TryAdd(ItemCatalog.PaddedArmorId);

            var weapons = InventoryEquipmentMenuData
                .GetEquipmentCandidates(
                    inventory,
                    EquipmentSlot.Weapon,
                    JobId.Mercenary);

            Assert.That(weapons.Count, Is.EqualTo(1));
            Assert.That(
                weapons[0].Id,
                Is.EqualTo(ItemCatalog.IronHeavyBladeId));
        }

        [Test]
        public void NonWeaponEquipmentRemainsAvailableAcrossJobs()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.PaddedArmorId);

            var armor = InventoryEquipmentMenuData.GetEquipmentCandidates(
                inventory,
                EquipmentSlot.Armor,
                JobId.WhiteMage);

            Assert.That(armor.Count, Is.EqualTo(1));
            Assert.That(armor[0].Id, Is.EqualTo(ItemCatalog.PaddedArmorId));
        }

        [Test]
        public void EquipmentDescriptionListsItsStatBonuses()
        {
            string description = InventoryEquipmentMenuData.FormatEquipment(
                ItemCatalog.Get(ItemCatalog.PaddedArmorId));

            Assert.That(description, Does.Contain("Padded Armor"));
            Assert.That(description, Does.Contain("HP +10"));
            Assert.That(description, Does.Contain("DEF +2"));
        }

        [Test]
        public void ItemUseMessagesExplainRestAndHealingResults()
        {
            Assert.That(
                InventoryEquipmentMenuData.GetUseMessage(
                    new ItemUseResult(
                        ItemUseStatus.RequiresRestService)),
                Does.Contain("making camp"));
            Assert.That(
                InventoryEquipmentMenuData.GetUseMessage(
                    new ItemUseResult(ItemUseStatus.Used, 25)),
                Is.EqualTo("Restored 25 HP."));
        }
    }
}
