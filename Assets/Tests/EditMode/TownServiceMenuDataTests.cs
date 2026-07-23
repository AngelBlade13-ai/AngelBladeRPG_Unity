using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class TownServiceMenuDataTests
    {
        [Test]
        public void MarketStockIsPresentedInDisplayOrder()
        {
            var items = TownServiceMenuData.GetBuyItems(
                ShopCatalog.Get(ShopCatalog.WhisperMarketId));

            Assert.That(items.Count, Is.EqualTo(3));
            Assert.That(items[0].Id, Is.EqualTo(ItemCatalog.CampRationId));
            Assert.That(items[1].Id, Is.EqualTo(ItemCatalog.FieldRemedyId));
            Assert.That(items[2].Id, Is.EqualTo(ItemCatalog.MinorPotionId));
        }

        [Test]
        public void SellListOnlyIncludesOwnedSellableItems()
        {
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.PaddedArmorId);
            inventory.TryAdd(ItemCatalog.MinorPotionId);
            inventory.TryAdd(ItemCatalog.EnoraScytheId);

            var items = TownServiceMenuData.GetSellItems(inventory);

            Assert.That(items.Count, Is.EqualTo(2));
            Assert.That(items[0].Id, Is.EqualTo(ItemCatalog.MinorPotionId));
            Assert.That(items[1].Id, Is.EqualTo(ItemCatalog.PaddedArmorId));
        }

        [Test]
        public void BuyDescriptionShowsOwnedQuantityAndTotalPrice()
        {
            string description = TownServiceMenuData.FormatShopItem(
                ItemCatalog.Get(ItemCatalog.MinorPotionId),
                3,
                2,
                false);

            Assert.That(description, Does.Contain("Owned 3"));
            Assert.That(description, Does.Contain("Buy 2"));
            Assert.That(description, Does.Contain("Total 60 gold"));
            Assert.That(description, Does.Contain("Restores 40 HP"));
        }

        [Test]
        public void SellDescriptionUsesTheSellPrice()
        {
            string description = TownServiceMenuData.FormatShopItem(
                ItemCatalog.Get(ItemCatalog.PaddedArmorId),
                2,
                2,
                true);

            Assert.That(description, Does.Contain("Sell 2"));
            Assert.That(description, Does.Contain("Total 70 gold"));
            Assert.That(description, Does.Contain("DEF +2"));
        }

        [Test]
        public void ShopMessagesExplainRejectedTransactions()
        {
            Assert.That(
                TownServiceMenuData.GetShopMessage(
                    ShopTransactionStatus.InsufficientGold),
                Is.EqualTo("Not enough gold."));
            Assert.That(
                TownServiceMenuData.GetShopMessage(
                    ShopTransactionStatus.InventoryFull),
                Is.EqualTo("That item stack is full."));
        }

        [Test]
        public void RecoveryMessagesExplainSuccessAndNoEffect()
        {
            Assert.That(
                TownServiceMenuData.GetRecoveryMessage(
                    TownRecoveryStatus.Recovered),
                Is.EqualTo("The party is fully recovered."));
            Assert.That(
                TownServiceMenuData.GetRecoveryMessage(
                    TownRecoveryStatus.AlreadyRecovered),
                Is.EqualTo("The party is already fully recovered."));
        }

        [Test]
        public void ExplorationModalStateRejectsASecondMenuOwner()
        {
            object first = new object();
            object second = new object();

            Assert.That(
                ExplorationModalState.TryAcquire(first),
                Is.True);
            Assert.That(
                ExplorationModalState.TryAcquire(second),
                Is.False);

            ExplorationModalState.Release(first);
        }

        [Test]
        public void ExplorationModalStateAllowsAnotherOwnerAfterRelease()
        {
            object first = new object();
            object second = new object();
            ExplorationModalState.TryAcquire(first);

            ExplorationModalState.Release(first);

            Assert.That(
                ExplorationModalState.TryAcquire(second),
                Is.True);
            ExplorationModalState.Release(second);
        }
    }
}
