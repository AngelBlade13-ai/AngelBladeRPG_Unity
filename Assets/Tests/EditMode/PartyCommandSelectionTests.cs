using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class PartyCommandSelectionTests
    {
        [Test]
        public void SelectionBeginsWithFirstLivingPartyMember()
        {
            PlayableCharacterData fallen = Member("fallen");
            fallen.Stats.CurrentHp = 0;
            PlayableCharacterData active = Member("active");

            PartyCommandSelection selection = Selection(
                new[] { fallen, active },
                new[] { Enemy("enemy") });

            Assert.That(selection.CurrentActor, Is.SameAs(active));
            Assert.That(selection.SelectedTarget.CombatantId, Is.EqualTo("enemy"));
            Assert.That(selection.IsComplete, Is.False);
        }

        [Test]
        public void QueuedCommandsAdvanceThroughLivingFormationOrder()
        {
            PlayableCharacterData first = Member("first");
            PlayableCharacterData second = Member("second");
            PartyCommandSelection selection = Selection(
                new[] { first, second },
                new[] { Enemy("enemy") });

            bool firstQueued = selection.TryQueueAttack();
            bool secondQueued = selection.TryQueueDefend();

            Assert.That(firstQueued, Is.True);
            Assert.That(secondQueued, Is.True);
            Assert.That(selection.IsComplete, Is.True);
            Assert.That(
                selection.Commands.Select(command => command.ActorId),
                Is.EqualTo(new[] { "first", "second" }));
        }

        [Test]
        public void TargetCyclingWrapsInBothDirections()
        {
            PartyCommandSelection selection = Selection(
                new[] { Member("hero") },
                new[]
                {
                    Enemy("first"),
                    Enemy("second"),
                    Enemy("third")
                });

            selection.CycleTarget(-1);
            Assert.That(selection.SelectedTarget.CombatantId, Is.EqualTo("third"));

            selection.CycleTarget(1);
            Assert.That(selection.SelectedTarget.CombatantId, Is.EqualTo("first"));
        }

        [Test]
        public void AttackStoresCurrentlySelectedTarget()
        {
            PartyCommandSelection selection = Selection(
                new[] { Member("hero") },
                new[] { Enemy("first"), Enemy("second") });
            selection.CycleTarget(1);

            selection.TryQueueAttack();

            Assert.That(selection.Commands[0].TargetId, Is.EqualTo("second"));
            Assert.That(
                selection.Commands[0].Type,
                Is.EqualTo(PartyBattleCommandType.PhysicalAttack));
        }

        [Test]
        public void DefendDoesNotRequireAnEnemyTarget()
        {
            MonsterData enemy = Enemy("enemy");
            enemy.CurrentHp = 0;
            PartyCommandSelection selection = Selection(
                new[] { Member("hero") },
                new[] { enemy });

            bool queued = selection.TryQueueDefend();

            Assert.That(queued, Is.True);
            Assert.That(selection.Commands[0].TargetId, Is.Empty);
        }

        [Test]
        public void CompletedSelectionRejectsAdditionalCommands()
        {
            PartyCommandSelection selection = Selection(
                new[] { Member("hero") },
                new[] { Enemy("enemy") });
            selection.TryQueueDefend();

            Assert.That(selection.TryQueueAttack(), Is.False);
            Assert.That(selection.TryQueueDefend(), Is.False);
            Assert.That(selection.Commands, Has.Count.EqualTo(1));
        }

        [Test]
        public void SelectionReportsWhetherAnyCommandsHaveBeenQueued()
        {
            PartyCommandSelection selection = Selection(
                new[] { Member("hero") },
                new[] { Enemy("enemy") });

            Assert.That(selection.HasQueuedCommands, Is.False);

            selection.TryQueueDefend();
            Assert.That(selection.HasQueuedCommands, Is.True);
        }

        private static PartyCommandSelection Selection(
            PlayableCharacterData[] party,
            MonsterData[] enemies)
        {
            return new PartyCommandSelection(
                new PartyBattleState(party, enemies));
        }

        private static PlayableCharacterData Member(string id)
        {
            return new PlayableCharacterData(id, id, JobId.Mercenary);
        }

        private static MonsterData Enemy(string id)
        {
            return new MonsterData(
                id,
                id,
                30,
                8,
                1,
                5,
                5,
                8,
                0,
                0,
                0);
        }
    }
}
