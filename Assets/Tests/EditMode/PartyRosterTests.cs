using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class PartyRosterTests
    {
        [Test]
        public void RosterCanHoldMoreCharactersThanTheActiveParty()
        {
            PartyRoster roster = CreateRoster(6);

            bool assigned = roster.TrySetActiveParty(
                new[] { "member-1", "member-2", "member-3", "member-4" });

            Assert.That(assigned, Is.True);
            Assert.That(roster.Characters.Count, Is.EqualTo(6));
            Assert.That(roster.ActiveCharacterIds.Count, Is.EqualTo(4));
        }

        [Test]
        public void ActivePartyRejectsMoreThanFourCharacters()
        {
            PartyRoster roster = CreateRoster(5);

            bool assigned = roster.TrySetActiveParty(
                new[]
                {
                    "member-1",
                    "member-2",
                    "member-3",
                    "member-4",
                    "member-5"
                });

            Assert.That(assigned, Is.False);
            Assert.That(roster.ActiveCharacterIds, Is.Empty);
        }

        [Test]
        public void InvalidPartyChangeDoesNotReplaceExistingParty()
        {
            PartyRoster roster = CreateRoster(4);
            roster.TrySetActiveParty(new[] { "member-1", "member-2" });

            bool assigned = roster.TrySetActiveParty(
                new[] { "member-1", "missing" });

            Assert.That(assigned, Is.False);
            Assert.That(
                roster.ActiveCharacterIds,
                Is.EqualTo(new[] { "member-1", "member-2" }));
        }

        [Test]
        public void PermanentRemovalAlsoRemovesCharacterFromActiveParty()
        {
            PartyRoster roster = CreateRoster(2);
            roster.TrySetActiveParty(new[] { "member-1", "member-2" });

            bool removed = roster.TryRemovePermanently("member-1");

            Assert.That(removed, Is.True);
            Assert.That(
                roster.GetCharacter("member-1").IsAvailable,
                Is.False);
            Assert.That(
                roster.ActiveCharacterIds,
                Is.EqualTo(new[] { "member-2" }));
        }

        private static PartyRoster CreateRoster(int characterCount)
        {
            PartyRoster roster = new PartyRoster();

            for (int index = 1; index <= characterCount; index++)
            {
                roster.TryAddCharacter(
                    new PlayableCharacterData(
                        $"member-{index}",
                        $"Member {index}",
                        JobId.Knight));
            }

            return roster;
        }
    }
}
