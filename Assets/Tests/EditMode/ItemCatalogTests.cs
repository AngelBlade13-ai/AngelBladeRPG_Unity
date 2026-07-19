using System.Collections.Generic;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class ItemCatalogTests
    {
        [Test]
        public void CatalogUsesUniqueStableIds()
        {
            var ids = new HashSet<string>();

            foreach (ItemDefinition item in ItemCatalog.All)
            {
                Assert.That(ids.Add(item.Id), Is.True, item.Id);
                Assert.That(ItemCatalog.Get(item.Id), Is.SameAs(item));
            }

            Assert.That(ids.Count, Is.EqualTo(14));
        }

        [Test]
        public void CatalogCoversAllEightWeaponCategories()
        {
            var categories = new HashSet<WeaponCategory>();

            foreach (ItemDefinition item in ItemCatalog.All)
            {
                if (item.Kind == ItemKind.Weapon)
                {
                    categories.Add(item.WeaponCategory);
                }
            }

            categories.Remove(WeaponCategory.None);
            Assert.That(categories.Count, Is.EqualTo(8));
        }

        [Test]
        public void WeaponCompatibilityMatchesTheEconomyContract()
        {
            ItemDefinition heavyBlade =
                ItemCatalog.Get(ItemCatalog.IronHeavyBladeId);
            ItemDefinition staff = ItemCatalog.Get(ItemCatalog.OakStaffId);
            ItemDefinition scythe = ItemCatalog.Get(ItemCatalog.EnoraScytheId);

            Assert.That(heavyBlade.IsCompatibleWith(JobId.Knight), Is.True);
            Assert.That(heavyBlade.IsCompatibleWith(JobId.Paladin), Is.True);
            Assert.That(heavyBlade.IsCompatibleWith(JobId.Mage), Is.False);
            Assert.That(staff.IsCompatibleWith(JobId.Mage), Is.True);
            Assert.That(staff.IsCompatibleWith(JobId.WhiteMage), Is.True);
            Assert.That(scythe.IsCompatibleWith(JobId.BloodMage), Is.True);
            Assert.That(scythe.CanSell, Is.False);
            Assert.That(scythe.CanDiscard, Is.False);
        }

        [Test]
        public void EquipmentUsesTheFiveConfirmedSlots()
        {
            Assert.That(
                ItemCatalog.Get(ItemCatalog.IronHeavyBladeId)
                    .CanEquipIn(EquipmentSlot.Weapon),
                Is.True);
            Assert.That(
                ItemCatalog.Get(ItemCatalog.PaddedArmorId)
                    .CanEquipIn(EquipmentSlot.Armor),
                Is.True);
            Assert.That(
                ItemCatalog.Get(ItemCatalog.GuardCharmId)
                    .CanEquipIn(EquipmentSlot.Accessory1),
                Is.True);
            Assert.That(
                ItemCatalog.Get(ItemCatalog.GuardCharmId)
                    .CanEquipIn(EquipmentSlot.Accessory2),
                Is.True);
            Assert.That(
                ItemCatalog.Get(ItemCatalog.SunstoneNecklaceId)
                    .CanEquipIn(EquipmentSlot.Necklace),
                Is.True);
        }
    }
}
