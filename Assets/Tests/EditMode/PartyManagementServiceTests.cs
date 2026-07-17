using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class PartyManagementServiceTests
    {
        [Test]
        public void OrderedCharactersShowFormationBeforeSortedReserve()
        {
            PartyRoster roster = CreateRoster(5);
            roster.TrySetActiveParty(new[] { "member-3", "member-1" });
            PartyManagementService service = new PartyManagementService(roster);

            Assert.That(
                service.GetOrderedCharacters().Select(character => character.Id),
                Is.EqualTo(new[]
                {
                    "member-3",
                    "member-1",
                    "member-2",
                    "member-4",
                    "member-5"
                }));
        }

        [Test]
        public void ReserveCharacterCanJoinAtEndOfFormation()
        {
            PartyRoster roster = CreateRoster(3);
            roster.TrySetActiveParty(new[] { "member-2" });
            PartyManagementService service = new PartyManagementService(roster);

            PartyManagementResult result =
                service.AddToActiveParty("member-1");

            Assert.That(result, Is.EqualTo(PartyManagementResult.Success));
            Assert.That(
                roster.ActiveCharacterIds,
                Is.EqualTo(new[] { "member-2", "member-1" }));
        }

        [Test]
        public void FullFormationRejectsAnotherCharacterWithoutMutation()
        {
            PartyRoster roster = CreateRoster(5);
            roster.TrySetActiveParty(new[]
            {
                "member-1",
                "member-2",
                "member-3",
                "member-4"
            });
            PartyManagementService service = new PartyManagementService(roster);

            PartyManagementResult result =
                service.AddToActiveParty("member-5");

            Assert.That(
                result,
                Is.EqualTo(PartyManagementResult.ActivePartyFull));
            Assert.That(
                roster.ActiveCharacterIds,
                Is.EqualTo(new[]
                {
                    "member-1",
                    "member-2",
                    "member-3",
                    "member-4"
                }));
        }

        [Test]
        public void LastActiveCharacterCannotBeMovedToReserve()
        {
            PartyRoster roster = CreateRoster(2);
            roster.TrySetActiveParty(new[] { "member-1" });
            PartyManagementService service = new PartyManagementService(roster);

            PartyManagementResult result =
                service.MoveToReserve("member-1");

            Assert.That(
                result,
                Is.EqualTo(PartyManagementResult.LastActiveMember));
            Assert.That(roster.ActiveCharacterIds, Is.EqualTo(new[] { "member-1" }));
        }

        [Test]
        public void ActiveCharacterCanMoveToReserveWhenAnotherRemains()
        {
            PartyRoster roster = CreateRoster(3);
            roster.TrySetActiveParty(new[] { "member-1", "member-2" });
            PartyManagementService service = new PartyManagementService(roster);

            PartyManagementResult result =
                service.MoveToReserve("member-1");

            Assert.That(result, Is.EqualTo(PartyManagementResult.Success));
            Assert.That(roster.ActiveCharacterIds, Is.EqualTo(new[] { "member-2" }));
        }

        [Test]
        public void ActiveFormationCanBeReorderedOnePositionAtATime()
        {
            PartyRoster roster = CreateRoster(3);
            roster.TrySetActiveParty(new[]
            {
                "member-1",
                "member-2",
                "member-3"
            });
            PartyManagementService service = new PartyManagementService(roster);

            PartyManagementResult result =
                service.MoveActiveCharacter("member-2", 1);

            Assert.That(result, Is.EqualTo(PartyManagementResult.Success));
            Assert.That(
                roster.ActiveCharacterIds,
                Is.EqualTo(new[]
                {
                    "member-1",
                    "member-3",
                    "member-2"
                }));
        }

        [TestCase("member-1", -1)]
        [TestCase("member-3", 1)]
        [TestCase("member-2", 2)]
        [TestCase("member-4", 1)]
        public void InvalidFormationMoveDoesNotChangeOrder(
            string characterId,
            int direction)
        {
            PartyRoster roster = CreateRoster(4);
            roster.TrySetActiveParty(new[]
            {
                "member-1",
                "member-2",
                "member-3"
            });
            PartyManagementService service = new PartyManagementService(roster);

            PartyManagementResult result =
                service.MoveActiveCharacter(characterId, direction);

            Assert.That(result, Is.EqualTo(PartyManagementResult.InvalidMove));
            Assert.That(
                roster.ActiveCharacterIds,
                Is.EqualTo(new[]
                {
                    "member-1",
                    "member-2",
                    "member-3"
                }));
        }

        [Test]
        public void AnyCatalogJobCanBeAssignedRegardlessOfAffinity()
        {
            PartyRoster roster = CreateRoster(1);
            PlayableCharacterData character = roster.GetCharacter("member-1");
            character.SetJobAffinity(JobId.BloodMage, JobAffinity.Low);
            PartyManagementService service = new PartyManagementService(roster);

            PartyManagementResult result = service.AssignJob(
                character.Id,
                JobId.BloodMage);

            Assert.That(result, Is.EqualTo(PartyManagementResult.Success));
            Assert.That(character.CurrentJob, Is.EqualTo(JobId.BloodMage));
        }

        [Test]
        public void RemovedAndUnknownCharactersAreRejected()
        {
            PartyRoster roster = CreateRoster(2);
            roster.TryRemovePermanently("member-2");
            PartyManagementService service = new PartyManagementService(roster);

            Assert.That(
                service.AssignJob("member-2", JobId.Mage),
                Is.EqualTo(PartyManagementResult.CharacterUnavailable));
            Assert.That(
                service.AddToActiveParty("missing"),
                Is.EqualTo(PartyManagementResult.CharacterNotFound));
        }

        private static PartyRoster CreateRoster(int characterCount)
        {
            PartyRoster roster = new PartyRoster();
            for (int index = 1; index <= characterCount; index++)
            {
                roster.TryAddCharacter(new PlayableCharacterData(
                    $"member-{index}",
                    $"Member {index}",
                    JobId.Mercenary));
            }

            return roster;
        }
    }
}
