using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class PartyMemberCatalogTests
    {
        [Test]
        public void CatalogContainsFourAuthoredMembersWithUniqueIds()
        {
            PartyMemberDefinition[] members = PartyMemberCatalog.All.ToArray();

            Assert.That(members, Has.Length.EqualTo(4));
            Assert.That(
                members.Select(member => member.Id).Distinct().Count(),
                Is.EqualTo(4));
        }

        [TestCase("pc_01", "Iona", JobId.WhiteMage, true)]
        [TestCase("pc_02", "Damari", JobId.Reaver, false)]
        [TestCase("pc_03", "Enora", JobId.BloodMage, false)]
        [TestCase("pc_04", "Lysander", JobId.Paladin, true)]
        public void AuthoredProfileUsesStableIdentityAndNaturalJob(
            string characterId,
            string displayName,
            JobId naturalJob,
            bool hasPlaceholderName)
        {
            PartyMemberDefinition member = PartyMemberCatalog.Get(characterId);

            Assert.That(member, Is.Not.Null);
            Assert.That(member.DisplayName, Is.EqualTo(displayName));
            Assert.That(member.NaturalJob, Is.EqualTo(naturalJob));
            Assert.That(
                member.HasPlaceholderName,
                Is.EqualTo(hasPlaceholderName));
            Assert.That(member.PersonalitySummary, Is.Not.Empty);
            Assert.That(member.CombatPreference, Is.Not.Empty);
        }

        [TestCase("pc_01", JobId.WhiteMage)]
        [TestCase("pc_02", JobId.Reaver)]
        [TestCase("pc_03", JobId.BloodMage)]
        [TestCase("pc_04", JobId.Paladin)]
        public void CreatedCharacterHasHighNaturalJobAffinity(
            string characterId,
            JobId naturalJob)
        {
            PlayableCharacterData character =
                PartyMemberCatalog.Get(characterId).CreateCharacter();

            Assert.That(character.Id, Is.EqualTo(characterId));
            Assert.That(character.CurrentJob, Is.EqualTo(naturalJob));
            Assert.That(
                character.GetJobAffinity(naturalJob),
                Is.EqualTo(JobAffinity.High));
            Assert.That(character.TryAssignJob(JobId.Rogue), Is.True);
        }

        [Test]
        public void UnknownStableIdDoesNotResolveByDisplayName()
        {
            Assert.That(PartyMemberCatalog.Get("Iona"), Is.Null);
            Assert.That(PartyMemberCatalog.Get("pc_01"), Is.Not.Null);
        }
    }
}
