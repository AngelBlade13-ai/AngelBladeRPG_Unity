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

        [Test]
        public void BattleParticipationTracksActiveAndBenchedCharacters()
        {
            PartyRoster roster = CreateRoster(3);
            roster.TrySetActiveParty(new[] { "member-1", "member-2" });

            roster.RecordBattleParticipation();

            Assert.That(
                roster.GetCharacter("member-1").RosterHistory.BattlesActive,
                Is.EqualTo(1));
            Assert.That(
                roster.GetCharacter("member-3").RosterHistory.BattlesBenched,
                Is.EqualTo(1));
            Assert.That(
                roster.GetCharacter("member-3")
                    .RosterHistory.ConsecutiveBenchedBattles,
                Is.EqualTo(1));
        }

        [Test]
        public void ReturningToActivePartyResetsConsecutiveBenchCount()
        {
            PartyRoster roster = CreateRoster(2);
            roster.TrySetActiveParty(new[] { "member-1" });
            roster.RecordBattleParticipation();
            roster.RecordBattleParticipation();
            roster.TrySetActiveParty(new[] { "member-2" });

            roster.RecordBattleParticipation();

            CharacterRosterHistory history =
                roster.GetCharacter("member-2").RosterHistory;
            Assert.That(history.BattlesBenched, Is.EqualTo(2));
            Assert.That(history.BattlesActive, Is.EqualTo(1));
            Assert.That(history.ConsecutiveBenchedBattles, Is.Zero);
        }

        [Test]
        public void BondPointsAreStoredForBothCharacters()
        {
            PartyRoster roster = CreateRoster(2);

            bool added = roster.TryAddBondPoints(
                "member-1",
                "member-2",
                3);

            Assert.That(added, Is.True);
            Assert.That(
                roster.GetCharacter("member-1")
                    .RosterHistory.GetBondPoints("member-2"),
                Is.EqualTo(3));
            Assert.That(
                roster.GetCharacter("member-2")
                    .RosterHistory.GetBondPoints("member-1"),
                Is.EqualTo(3));
        }

        [Test]
        public void BondPointsRejectSelfOrUnavailableCharacters()
        {
            PartyRoster roster = CreateRoster(2);
            roster.TryRemovePermanently("member-2");

            Assert.That(
                roster.TryAddBondPoints("member-1", "member-1", 2),
                Is.False);
            Assert.That(
                roster.TryAddBondPoints("member-1", "member-2", 2),
                Is.False);
        }

        [Test]
        public void RemovedCharacterKeepsHistoryButStopsAccumulatingIt()
        {
            PartyRoster roster = CreateRoster(2);
            roster.TrySetActiveParty(new[] { "member-1" });
            roster.RecordBattleParticipation();
            PlayableCharacterData removed = roster.GetCharacter("member-2");
            roster.TryRemovePermanently("member-2");

            roster.RecordBattleParticipation();

            Assert.That(removed.RosterHistory.BattlesBenched, Is.EqualTo(1));
            Assert.That(removed.IsAvailable, Is.False);
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
