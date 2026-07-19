using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class CharacterEquipmentTests
    {
        [Test]
        public void EquippingConsumesInventoryAndAppliesStats()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.IronHeavyBladeId);
            var character = CreateCharacter(JobId.Mercenary);

            bool equipped = character.TryEquipItem(
                EquipmentSlot.Weapon,
                ItemCatalog.IronHeavyBladeId,
                inventory);

            Assert.That(equipped, Is.True);
            Assert.That(
                inventory.GetQuantity(ItemCatalog.IronHeavyBladeId),
                Is.Zero);
            Assert.That(character.Stats.Attack, Is.EqualTo(21));
        }

        [Test]
        public void ReplacingEquipmentReturnsThePreviousItem()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.GuardCharmId);
            inventory.TryAdd(ItemCatalog.SunstoneNecklaceId);
            var character = CreateCharacter(JobId.Knight);
            character.TryEquipItem(
                EquipmentSlot.Accessory1,
                ItemCatalog.GuardCharmId,
                inventory);

            bool invalidReplacement = character.TryEquipItem(
                EquipmentSlot.Accessory1,
                ItemCatalog.SunstoneNecklaceId,
                inventory);

            Assert.That(invalidReplacement, Is.False);
            Assert.That(
                character.Equipment.GetItemId(EquipmentSlot.Accessory1),
                Is.EqualTo(ItemCatalog.GuardCharmId));
            Assert.That(
                inventory.GetQuantity(ItemCatalog.SunstoneNecklaceId),
                Is.EqualTo(1));
        }

        [Test]
        public void TwoAccessoryCopiesCanFillBothAccessorySlots()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.GuardCharmId, 2);
            var character = CreateCharacter(JobId.Knight);

            Assert.That(
                character.TryEquipItem(
                    EquipmentSlot.Accessory1,
                    ItemCatalog.GuardCharmId,
                    inventory),
                Is.True);
            Assert.That(
                character.TryEquipItem(
                    EquipmentSlot.Accessory2,
                    ItemCatalog.GuardCharmId,
                    inventory),
                Is.True);
            Assert.That(
                inventory.GetQuantity(ItemCatalog.GuardCharmId),
                Is.Zero);
            Assert.That(character.Stats.Defense, Is.EqualTo(10));
        }

        [Test]
        public void MaximumHpEquipmentPreservesMissingHp()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.PaddedArmorId);
            var character = CreateCharacter(JobId.Mercenary);
            character.Stats.ApplyDamage(30);

            character.TryEquipItem(
                EquipmentSlot.Armor,
                ItemCatalog.PaddedArmorId,
                inventory);

            Assert.That(character.Stats.MaxHp, Is.EqualTo(110));
            Assert.That(character.Stats.CurrentHp, Is.EqualTo(80));

            character.TryUnequipItem(EquipmentSlot.Armor, inventory);

            Assert.That(character.Stats.MaxHp, Is.EqualTo(100));
            Assert.That(character.Stats.CurrentHp, Is.EqualTo(70));
        }

        [Test]
        public void IncompatibleWeaponCannotBeEquipped()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.OakStaffId);
            var character = CreateCharacter(JobId.Mercenary);

            bool equipped = character.TryEquipItem(
                EquipmentSlot.Weapon,
                ItemCatalog.OakStaffId,
                inventory);

            Assert.That(equipped, Is.False);
            Assert.That(
                inventory.GetQuantity(ItemCatalog.OakStaffId),
                Is.EqualTo(1));
        }

        [Test]
        public void JobSwitchReturnsAnIncompatibleWeaponToInventory()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.IronHeavyBladeId);
            var character = CreateCharacter(JobId.Mercenary);
            character.TryEquipItem(
                EquipmentSlot.Weapon,
                ItemCatalog.IronHeavyBladeId,
                inventory);

            Assert.That(character.TryAssignJob(JobId.Mage), Is.False);
            Assert.That(
                character.TryAssignJob(JobId.Mage, inventory),
                Is.True);
            Assert.That(character.CurrentJob, Is.EqualTo(JobId.Mage));
            Assert.That(
                character.Equipment.GetItemId(EquipmentSlot.Weapon),
                Is.Null);
            Assert.That(
                inventory.GetQuantity(ItemCatalog.IronHeavyBladeId),
                Is.EqualTo(1));
        }

        [Test]
        public void PermanentRemovalDestroysEquippedItemsWithoutReturningThem()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.IronHeavyBladeId);
            inventory.TryAdd(ItemCatalog.PaddedArmorId);
            var character = CreateCharacter(JobId.Reaver);
            character.TryEquipItem(
                EquipmentSlot.Weapon,
                ItemCatalog.IronHeavyBladeId,
                inventory);
            character.TryEquipItem(
                EquipmentSlot.Armor,
                ItemCatalog.PaddedArmorId,
                inventory);

            character.RemovePermanently();
            character.RemovePermanently();

            Assert.That(character.Equipment.EquippedItemIds, Is.Empty);
            Assert.That(
                inventory.GetQuantity(ItemCatalog.IronHeavyBladeId),
                Is.Zero);
            Assert.That(
                inventory.GetQuantity(ItemCatalog.PaddedArmorId),
                Is.Zero);
        }

        [Test]
        public void NewGameCreatesAnEmptySharedInventory()
        {
            var session = new GameSession();

            Assert.That(session.TryStartNewGame("Angel"), Is.True);
            Assert.That(session.Inventory, Is.Not.Null);
            Assert.That(session.Inventory.Quantities, Is.Empty);
        }

        private static PlayableCharacterData CreateCharacter(JobId jobId)
        {
            return new PlayableCharacterData("hero", "Angel", jobId);
        }
    }
}
