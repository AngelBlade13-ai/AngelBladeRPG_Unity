using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class BattleLayoutCatalogTests
    {
        [Test]
        public void CatalogProvidesStableStandardAndBossLayouts()
        {
            Assert.That(
                BattleLayoutCatalog.All.Select(layout => layout.Id),
                Is.EquivalentTo(new[]
                {
                    BattleLayoutCatalog.StandardId,
                    BattleLayoutCatalog.BossId
                }));
        }

        [TestCase(BattleLayoutCatalog.StandardId, 4)]
        [TestCase(BattleLayoutCatalog.BossId, 5)]
        public void LayoutSupportsPartyAndExpectedEnemySlots(
            string layoutId,
            int expectedEnemySlots)
        {
            BattleLayoutDefinition layout = BattleLayoutCatalog.Get(layoutId);

            Assert.That(
                layout.PartySlots,
                Has.Count.EqualTo(PartyRoster.MaximumActiveMembers));
            Assert.That(layout.EnemySlots, Has.Count.EqualTo(expectedEnemySlots));
        }

        [Test]
        public void UnknownLayoutReturnsNull()
        {
            Assert.That(BattleLayoutCatalog.Get("layout_unknown"), Is.Null);
        }
    }
}
