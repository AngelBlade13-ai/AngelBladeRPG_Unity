using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class SpeedTurnOrderTests
    {
        [Test]
        public void HigherSpeedActsBeforeLowerSpeed()
        {
            BattleTurnParticipant[] participants =
            {
                Participant("slow", 5),
                Participant("fast", 15),
                Participant("middle", 10)
            };

            IReadOnlyList<BattleTurnParticipant> order =
                SpeedTurnOrder.Build(participants, new MinimumIndexRandom());

            Assert.That(
                order.Select(entry => entry.CombatantId),
                Is.EqualTo(new[] { "fast", "middle", "slow" }));
        }

        [Test]
        public void EqualSpeedParticipantsUseRandomTieBreaker()
        {
            BattleTurnParticipant[] participants =
            {
                Participant("first", 10),
                Participant("second", 10),
                Participant("third", 10)
            };

            IReadOnlyList<BattleTurnParticipant> order =
                SpeedTurnOrder.Build(participants, new MinimumIndexRandom());

            Assert.That(
                order.Select(entry => entry.CombatantId),
                Is.EqualTo(new[] { "second", "third", "first" }));
        }

        [Test]
        public void TieShuffleDoesNotCrossSpeedGroups()
        {
            BattleTurnParticipant[] participants =
            {
                Participant("fast-a", 20),
                Participant("slow-a", 5),
                Participant("fast-b", 20),
                Participant("slow-b", 5)
            };

            IReadOnlyList<BattleTurnParticipant> order =
                SpeedTurnOrder.Build(participants, new MinimumIndexRandom());

            Assert.That(order[0].Speed, Is.EqualTo(20));
            Assert.That(order[1].Speed, Is.EqualTo(20));
            Assert.That(order[2].Speed, Is.EqualTo(5));
            Assert.That(order[3].Speed, Is.EqualTo(5));
        }

        [Test]
        public void BuildingOrderDoesNotModifyInputCollection()
        {
            List<BattleTurnParticipant> participants =
                new List<BattleTurnParticipant>
                {
                    Participant("slow", 5),
                    Participant("fast", 15)
                };

            SpeedTurnOrder.Build(participants, new MinimumIndexRandom());

            Assert.That(
                participants.Select(entry => entry.CombatantId),
                Is.EqualTo(new[] { "slow", "fast" }));
        }

        [Test]
        public void DuplicateCombatantIdsAreRejected()
        {
            BattleTurnParticipant[] participants =
            {
                Participant("same-id", 5),
                Participant("same-id", 10)
            };

            Assert.Throws<ArgumentException>(
                () => SpeedTurnOrder.Build(participants));
        }

        private static BattleTurnParticipant Participant(string id, int speed)
        {
            return new BattleTurnParticipant(id, speed);
        }

        private class MinimumIndexRandom : ITurnOrderRandom
        {
            public int NextIndex(int minimumInclusive, int maximumExclusive)
            {
                return minimumInclusive;
            }
        }
    }
}
