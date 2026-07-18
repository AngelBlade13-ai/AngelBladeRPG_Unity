using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class BattleSceneControllerTests
    {
        [Test]
        public void FormatCombatantStatusIncludesNameAndHp()
        {
            string status = BattleSceneController.FormatCombatantStatus(
                "Goblin",
                12,
                35);

            Assert.That(status, Is.EqualTo("Goblin\nHP 12/35"));
        }

        [Test]
        public void FormatCombatantGroupShowsActorTargetResourcesAndDefeat()
        {
            PlayableCharacterData active = new PlayableCharacterData(
                "active",
                "Angel",
                JobId.Mercenary);
            PlayableCharacterData fallen = new PlayableCharacterData(
                "fallen",
                "Iona",
                JobId.WhiteMage);
            fallen.Stats.CurrentHp = 0;

            string status = BattleSceneController.FormatCombatantGroup(
                new ICombatant[] { active, fallen },
                active.Id,
                fallen.Id);

            Assert.That(status, Does.Contain("> Angel  HP 100/100 MP 20/20"));
            Assert.That(status, Does.Contain("* Iona  INCAPACITATED"));
        }

        [Test]
        public void FormatCommandPromptNamesActorAndTarget()
        {
            PlayableCharacterData actor = new PlayableCharacterData(
                "hero",
                "Angel",
                JobId.Mercenary);
            MonsterData target = new MonsterData(
                "goblin",
                "Goblin",
                30,
                8,
                1,
                5,
                5,
                8,
                0,
                0,
                0);

            string prompt = BattleSceneController.FormatCommandPrompt(
                actor,
                target);

            Assert.That(prompt, Is.EqualTo("Angel -> Goblin"));
        }

        [Test]
        public void FormatAbilityPromptNamesAbilityActorAndTarget()
        {
            PlayableCharacterData actor = new PlayableCharacterData(
                "healer",
                "Iona",
                JobId.WhiteMage);
            PlayableCharacterData target = new PlayableCharacterData(
                "hero",
                "Angel",
                JobId.Mercenary);

            string prompt = BattleSceneController.FormatAbilityPrompt(
                actor,
                target,
                CombatAbilityCatalog.Get(CombatAbilityCatalog.MendId));

            Assert.That(prompt, Is.EqualTo("Mend (4 MP): Iona -> Angel"));
        }

        [Test]
        public void FormatCombatantGroupShowsCombinedSelfTargetMarker()
        {
            PlayableCharacterData actor = new PlayableCharacterData(
                "healer",
                "Iona",
                JobId.WhiteMage);

            string status = BattleSceneController.FormatCombatantGroup(
                new ICombatant[] { actor },
                actor.Id,
                actor.Id);

            Assert.That(status, Does.StartWith(">* Iona"));
        }

        [Test]
        public void GaugeStatusShowsVisibleActionProgress()
        {
            PlayableCharacterData actor = new PlayableCharacterData(
                "hero",
                "Angel",
                JobId.Mercenary);
            MonsterData enemy = new MonsterData(
                "enemy", "Goblin", 30, 8, 1, 0, 0, 10, 0, 0, 0);
            ActionGaugeBattle gauges = new ActionGaugeBattle(
                new PartyBattleState(new[] { actor }, new[] { enemy }));
            gauges.Tick(2.5f, commandMenuIsOpen: false);

            string status = BattleSceneController.FormatCombatantGroupWithGauges(
                new ICombatant[] { actor },
                actor.Id,
                null,
                gauges);

            Assert.That(status, Does.Contain("AT 50%"));
        }
    }
}
