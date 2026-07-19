using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class CampRestServiceTests
    {
        [Test]
        public void TutorialRestIsFreeExactlyOnce()
        {
            TestContext context = CreateContext();
            DamageAll(context.Roster);

            CampRestResult first = context.Service.TryFullRest(true, true);
            DamageAll(context.Roster);
            CampRestResult second = context.Service.TryFullRest(true, true);

            Assert.That(first.Succeeded, Is.True);
            Assert.That(first.ConsumedRation, Is.False);
            Assert.That(context.State.TutorialRestUsed, Is.True);
            Assert.That(
                second.Status,
                Is.EqualTo(CampRestStatus.MissingRation));
            Assert.That(context.State.CompletedRestCount, Is.EqualTo(1));
        }

        [Test]
        public void LaterRestConsumesExactlyOneRation()
        {
            TestContext context = CreateContext();
            context.Inventory.TryAdd(ItemCatalog.CampRationId, 2);
            DamageAll(context.Roster);

            CampRestResult result = context.Service.TryFullRest(true, false);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.ConsumedRation, Is.True);
            Assert.That(
                context.Inventory.GetQuantity(ItemCatalog.CampRationId),
                Is.EqualTo(1));
        }

        [Test]
        public void CancelledRestConsumesNothing()
        {
            TestContext context = CreateContext();
            context.Inventory.TryAdd(ItemCatalog.CampRationId);
            DamageAll(context.Roster);

            CampRestResult result = context.Service.TryFullRest(false, false);

            Assert.That(result.Status, Is.EqualTo(CampRestStatus.Cancelled));
            Assert.That(
                context.Inventory.GetQuantity(ItemCatalog.CampRationId),
                Is.EqualTo(1));
            Assert.That(context.State.CompletedRestCount, Is.Zero);
        }

        [Test]
        public void FullyRecoveredPartyDoesNotSpendOrMarkTutorialRest()
        {
            TestContext context = CreateContext();
            context.Inventory.TryAdd(ItemCatalog.CampRationId);

            CampRestResult result = context.Service.TryFullRest(true, true);

            Assert.That(
                result.Status,
                Is.EqualTo(CampRestStatus.AlreadyRecovered));
            Assert.That(context.State.TutorialRestUsed, Is.False);
            Assert.That(
                context.Inventory.GetQuantity(ItemCatalog.CampRationId),
                Is.EqualTo(1));
        }

        [Test]
        public void FullRestRestoresAndRevivesActiveAndBenchedCharacters()
        {
            TestContext context = CreateContext();
            PlayableCharacterData active =
                context.Roster.GetCharacter("member-1");
            PlayableCharacterData benched =
                context.Roster.GetCharacter("member-2");
            active.Stats.CurrentHp = 0;
            active.Stats.CurrentMp = 0;
            benched.Stats.ApplyDamage(35);
            benched.Stats.CurrentMp = 1;

            CampRestResult result = context.Service.TryFullRest(true, true);

            Assert.That(result.RestoredCharacterCount, Is.EqualTo(2));
            Assert.That(active.Stats.CurrentHp, Is.EqualTo(active.Stats.MaxHp));
            Assert.That(active.Stats.CurrentMp, Is.EqualTo(active.Stats.MaxMp));
            Assert.That(benched.Stats.CurrentHp, Is.EqualTo(benched.Stats.MaxHp));
            Assert.That(benched.Stats.CurrentMp, Is.EqualTo(benched.Stats.MaxMp));
        }

        [Test]
        public void FullRestDoesNotRestorePermanentlyRemovedCharacters()
        {
            TestContext context = CreateContext();
            PlayableCharacterData removed =
                context.Roster.GetCharacter("member-2");
            removed.Stats.ApplyDamage(40);
            int hpBeforeRemoval = removed.Stats.CurrentHp;
            context.Roster.TryRemovePermanently(removed.Id);
            context.Roster.GetCharacter("member-1").Stats.ApplyDamage(20);

            context.Service.TryFullRest(true, true);

            Assert.That(removed.Stats.CurrentHp, Is.EqualTo(hpBeforeRemoval));
        }

        private static TestContext CreateContext()
        {
            var roster = new PartyRoster();
            roster.TryAddCharacter(new PlayableCharacterData(
                "member-1", "One", JobId.Knight));
            roster.TryAddCharacter(new PlayableCharacterData(
                "member-2", "Two", JobId.WhiteMage));
            roster.TrySetActiveParty(new[] { "member-1" });
            return new TestContext(roster);
        }

        private static void DamageAll(PartyRoster roster)
        {
            foreach (PlayableCharacterData character in roster.Characters)
            {
                if (character.IsAvailable)
                {
                    character.Stats.ApplyDamage(10);
                }
            }
        }

        private sealed class TestContext
        {
            public PartyRoster Roster { get; }
            public Inventory Inventory { get; } = new Inventory();
            public CampRestState State { get; } = new CampRestState();
            public CampRestService Service { get; }

            public TestContext(PartyRoster roster)
            {
                Roster = roster;
                Service = new CampRestService(Roster, Inventory, State);
            }
        }
    }
}
