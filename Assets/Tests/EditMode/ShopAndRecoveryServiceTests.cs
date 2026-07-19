using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class ShopAndRecoveryServiceTests
    {
        [Test]
        public void PurchaseMovesGoldAndItemsAtomically()
        {
            ShopContext context = CreateShop();
            context.Player.Gold = 100;

            ShopTransactionStatus status = context.Service.TryBuy(
                ItemCatalog.MinorPotionId,
                2);

            Assert.That(status, Is.EqualTo(ShopTransactionStatus.Completed));
            Assert.That(context.Player.Gold, Is.EqualTo(40));
            Assert.That(
                context.Inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.EqualTo(2));
        }

        [Test]
        public void FailedPurchaseChangesNothing()
        {
            ShopContext context = CreateShop();
            context.Player.Gold = 20;

            ShopTransactionStatus status = context.Service.TryBuy(
                ItemCatalog.MinorPotionId,
                1);

            Assert.That(
                status,
                Is.EqualTo(ShopTransactionStatus.InsufficientGold));
            Assert.That(context.Player.Gold, Is.EqualTo(20));
            Assert.That(context.Inventory.Quantities, Is.Empty);
        }

        [Test]
        public void FullStackPurchaseDoesNotChargeGold()
        {
            ShopContext context = CreateShop();
            context.Player.Gold = 100;
            context.Inventory.TryAdd(ItemCatalog.MinorPotionId, 99);

            ShopTransactionStatus status = context.Service.TryBuy(
                ItemCatalog.MinorPotionId,
                1);

            Assert.That(status, Is.EqualTo(ShopTransactionStatus.InventoryFull));
            Assert.That(context.Player.Gold, Is.EqualTo(100));
        }

        [Test]
        public void ShopRejectsItemsItDoesNotStock()
        {
            ShopContext context = CreateShop();
            context.Player.Gold = 1000;

            ShopTransactionStatus status = context.Service.TryBuy(
                ItemCatalog.IronHeavyBladeId,
                1);

            Assert.That(status, Is.EqualTo(ShopTransactionStatus.ItemNotStocked));
            Assert.That(context.Player.Gold, Is.EqualTo(1000));
        }

        [Test]
        public void SaleRemovesItemsAndAddsGold()
        {
            ShopContext context = CreateShop();
            context.Inventory.TryAdd(ItemCatalog.MinorPotionId, 2);

            ShopTransactionStatus status = context.Service.TrySell(
                ItemCatalog.MinorPotionId,
                2);

            Assert.That(status, Is.EqualTo(ShopTransactionStatus.Completed));
            Assert.That(context.Player.Gold, Is.EqualTo(20));
            Assert.That(
                context.Inventory.GetQuantity(ItemCatalog.MinorPotionId),
                Is.Zero);
        }

        [Test]
        public void ProtectedEquipmentCannotBeSold()
        {
            ShopContext context = CreateShop();
            context.Inventory.TryAdd(ItemCatalog.EnoraScytheId);

            ShopTransactionStatus status = context.Service.TrySell(
                ItemCatalog.EnoraScytheId,
                1);

            Assert.That(status, Is.EqualTo(ShopTransactionStatus.CannotSell));
            Assert.That(context.Player.Gold, Is.Zero);
            Assert.That(
                context.Inventory.GetQuantity(ItemCatalog.EnoraScytheId),
                Is.EqualTo(1));
        }

        [Test]
        public void TownRecoveryChargesOnceAndRestoresTheWholeRoster()
        {
            RecoveryContext context = CreateRecovery();
            context.Player.Gold = 100;
            foreach (PlayableCharacterData character in context.Roster.Characters)
            {
                character.Stats.ApplyDamage(20);
                character.Stats.CurrentMp = 0;
            }

            TownRecoveryStatus status = context.Service
                .TryPurchaseFullRecovery(25, true);

            Assert.That(status, Is.EqualTo(TownRecoveryStatus.Recovered));
            Assert.That(context.Player.Gold, Is.EqualTo(75));
            foreach (PlayableCharacterData character in context.Roster.Characters)
            {
                Assert.That(character.Stats.CurrentHp, Is.EqualTo(character.Stats.MaxHp));
                Assert.That(character.Stats.CurrentMp, Is.EqualTo(character.Stats.MaxMp));
            }
        }

        [Test]
        public void CancelledTownRecoveryDoesNotChargeGold()
        {
            RecoveryContext context = CreateRecovery();
            context.Player.Gold = 100;
            context.Roster.GetCharacter("hero").Stats.ApplyDamage(10);

            TownRecoveryStatus status = context.Service
                .TryPurchaseFullRecovery(25, false);

            Assert.That(status, Is.EqualTo(TownRecoveryStatus.Cancelled));
            Assert.That(context.Player.Gold, Is.EqualTo(100));
        }

        [Test]
        public void FullyRecoveredTownPartyIsNotCharged()
        {
            RecoveryContext context = CreateRecovery();
            context.Player.Gold = 100;

            TownRecoveryStatus status = context.Service
                .TryPurchaseFullRecovery(25, true);

            Assert.That(status, Is.EqualTo(TownRecoveryStatus.AlreadyRecovered));
            Assert.That(context.Player.Gold, Is.EqualTo(100));
        }

        private static ShopContext CreateShop()
        {
            var player = new PlayerData("Angel");
            var inventory = new Inventory();
            return new ShopContext(
                player,
                inventory,
                new ShopService(
                    ShopCatalog.Get(ShopCatalog.WhisperMarketId),
                    player,
                    inventory));
        }

        private static RecoveryContext CreateRecovery()
        {
            var player = new PlayerData("Angel");
            var roster = new PartyRoster();
            roster.TryAddCharacter(new PlayableCharacterData(
                "hero", "Angel", JobId.Mercenary, player.Stats));
            roster.TryAddCharacter(new PlayableCharacterData(
                "ally", "Iona", JobId.WhiteMage));
            roster.TrySetActiveParty(new[] { "hero" });
            return new RecoveryContext(player, roster);
        }

        private sealed class ShopContext
        {
            public PlayerData Player { get; }
            public Inventory Inventory { get; }
            public ShopService Service { get; }

            public ShopContext(
                PlayerData player,
                Inventory inventory,
                ShopService service)
            {
                Player = player;
                Inventory = inventory;
                Service = service;
            }
        }

        private sealed class RecoveryContext
        {
            public PlayerData Player { get; }
            public PartyRoster Roster { get; }
            public TownRecoveryService Service { get; }

            public RecoveryContext(PlayerData player, PartyRoster roster)
            {
                Player = player;
                Roster = roster;
                Service = new TownRecoveryService(Player, Roster);
            }
        }
    }
}
