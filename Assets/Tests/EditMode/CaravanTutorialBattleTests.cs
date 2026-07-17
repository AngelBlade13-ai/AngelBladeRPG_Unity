using System;
using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class CaravanTutorialBattleTests
    {
        [Test]
        public void TutorialAdvancesInAuthoredOrderAndLocksInParty()
        {
            GameSession session = StartTutorial();
            Assert.That(
                session.CaravanTutorial.Stage,
                Is.EqualTo(CaravanTutorialStage.BasicGoblins));

            DefeatCurrentEnemies(session);
            var ionaMessages = session.AdvanceTutorialAfterRound(
                Round(enemiesDefeated: true));

            Assert.That(
                session.CaravanTutorial.Stage,
                Is.EqualTo(CaravanTutorialStage.HobgoblinPressure));
            Assert.That(session.Player.CurrentHp, Is.EqualTo(25));
            Assert.That(
                session.PartyBattle.PartyMembers.Select(member => member.CombatantId),
                Is.EqualTo(new[] { "pc_protagonist", "pc_01" }));
            Assert.That(session.Monster.CombatantId,
                Is.EqualTo(CaravanTutorialBattle.HobgoblinCombatantId));
            Assert.That(ionaMessages.Count, Is.EqualTo(3));

            var reinforcementMessages = session.AdvanceTutorialAfterRound(
                Round(enemiesDefeated: false));

            Assert.That(
                session.CaravanTutorial.Stage,
                Is.EqualTo(CaravanTutorialStage.FullParty));
            Assert.That(session.Party.GetCharacter("pc_01").Stats.CurrentHp,
                Is.EqualTo(25));
            Assert.That(
                session.PartyBattle.PartyMembers.Select(member => member.CombatantId),
                Is.EqualTo(new[]
                {
                    "pc_protagonist", "pc_01", "pc_02", "pc_03"
                }));
            Assert.That(reinforcementMessages.Count, Is.EqualTo(3));

            DefeatCurrentEnemies(session);
            session.AdvanceTutorialAfterRound(Round(enemiesDefeated: true));

            Assert.That(
                session.CaravanTutorial.Stage,
                Is.EqualTo(CaravanTutorialStage.Completed));
            Assert.That(
                session.Party.ActiveCharacterIds,
                Is.EqualTo(new[]
                {
                    "pc_protagonist", "pc_01", "pc_02", "pc_03"
                }));
        }

        [Test]
        public void TutorialSafetyRulesProtectScriptedCombatants()
        {
            GameSession session = StartTutorial();
            ICombatant protagonist = session.PartyBattle.PartyMembers[0];

            Assert.That(
                session.CaravanTutorial.GetMinimumHp(protagonist),
                Is.EqualTo(1));

            DefeatCurrentEnemies(session);
            session.AdvanceTutorialAfterRound(Round(enemiesDefeated: true));
            ICombatant iona = session.Party.GetCharacter("pc_01");
            ICombatant hobgoblin = session.Monster;

            Assert.That(session.CaravanTutorial.GetMinimumHp(iona), Is.EqualTo(1));
            Assert.That(
                session.CaravanTutorial.GetMinimumHp(hobgoblin),
                Is.EqualTo(1));

            session.AdvanceTutorialAfterRound(Round(enemiesDefeated: false));

            Assert.That(session.CaravanTutorial.GetMinimumHp(iona), Is.Zero);
            Assert.That(session.CaravanTutorial.GetMinimumHp(hobgoblin), Is.Zero);
        }

        [Test]
        public void EnemyFocusMovesFromIonaToDamariAfterTauntBeat()
        {
            GameSession session = StartTutorial();
            DefeatCurrentEnemies(session);
            session.AdvanceTutorialAfterRound(Round(enemiesDefeated: true));

            PartyBattleCommand pressureCommand =
                session.CaravanTutorial.CreateCommand(
                    session.Monster,
                    session.PartyBattle);

            session.AdvanceTutorialAfterRound(Round(enemiesDefeated: false));
            PartyBattleCommand tauntedCommand =
                session.CaravanTutorial.CreateCommand(
                    session.Monster,
                    session.PartyBattle);

            Assert.That(pressureCommand.TargetId, Is.EqualTo("pc_01"));
            Assert.That(tauntedCommand.TargetId, Is.EqualTo("pc_02"));
        }

        [Test]
        public void TutorialResolverProtectsHobgoblinDuringPressureRound()
        {
            GameSession session = StartTutorial();
            DefeatCurrentEnemies(session);
            session.AdvanceTutorialAfterRound(Round(enemiesDefeated: true));
            PlayableCharacterData protagonist = session.Party.GetCharacter(
                PlayableCharacterData.ProtagonistId);
            PlayableCharacterData iona = session.Party.GetCharacter("pc_01");
            session.Monster.CurrentHp = 2;

            PartyBattleRoundResult round = session.CreatePartyRoundResolver(
                combatRandom: new AlwaysHitCombatRandom()).ResolveRound(
                    session.PartyBattle,
                    new[]
                    {
                        PartyBattleCommand.Attack(
                            protagonist.Id,
                            session.Monster.CombatantId),
                        PartyBattleCommand.Defend(iona.Id)
                    });

            Assert.That(session.Monster.CurrentHp, Is.EqualTo(1));
            Assert.That(iona.Stats.CurrentHp, Is.GreaterThanOrEqualTo(1));
            Assert.That(
                round.Actions.Any(action =>
                    action.ActorId == session.Monster.CombatantId &&
                    action.TargetId == iona.Id),
                Is.True);
            Assert.That(round.EnemiesWereDefeated, Is.False);
        }

        [Test]
        public void TutorialDisablesEscapeAndCannotPayRewardsBeforeFinalStage()
        {
            GameSession session = StartTutorial();
            DefeatCurrentEnemies(session);

            bool escaped = session.CompleteEscape();
            bool rewarded = session.TryCompleteVictory(out var rewards);

            Assert.That(escaped, Is.False);
            Assert.That(rewarded, Is.False);
            Assert.That(rewards, Is.Null);
            Assert.That(session.HasActiveBattle, Is.True);
        }

        [Test]
        public void TutorialRewardsAllWavesOnceAndCannotReplay()
        {
            GameSession session = StartTutorial();
            CompleteTutorial(session);

            bool completed = session.TryCompleteVictory(out var rewards);
            bool duplicate = session.TryCompleteVictory(out var duplicateRewards);
            bool replayed = session.StartEncounter(BattleEncounterCatalog.Get(
                BattleEncounterCatalog.CaravanTutorialId));

            Assert.That(completed, Is.True);
            Assert.That(rewards.Gold, Is.EqualTo(41));
            Assert.That(rewards.XP, Is.EqualTo(75));
            Assert.That(rewards.JobPoints, Is.EqualTo(4));
            Assert.That(rewards.CharacterRewards, Has.Count.EqualTo(4));
            Assert.That(session.IsEncounterCompleted(
                BattleEncounterCatalog.CaravanTutorialId), Is.True);
            Assert.That(duplicate, Is.False);
            Assert.That(duplicateRewards, Is.Null);
            Assert.That(replayed, Is.False);
        }

        private static GameSession StartTutorial()
        {
            GameSession session = new GameSession();
            session.TryStartNewGame("Angel");
            Assert.That(session.StartEncounter(BattleEncounterCatalog.Get(
                BattleEncounterCatalog.CaravanTutorialId)), Is.True);
            return session;
        }

        private static void CompleteTutorial(GameSession session)
        {
            DefeatCurrentEnemies(session);
            session.AdvanceTutorialAfterRound(Round(enemiesDefeated: true));
            session.AdvanceTutorialAfterRound(Round(enemiesDefeated: false));
            DefeatCurrentEnemies(session);
            session.AdvanceTutorialAfterRound(Round(enemiesDefeated: true));
        }

        private static void DefeatCurrentEnemies(GameSession session)
        {
            foreach (ICombatant enemy in session.PartyBattle.Enemies)
            {
                enemy.Stats.CurrentHp = 0;
            }
        }

        private static PartyBattleRoundResult Round(bool enemiesDefeated)
        {
            return new PartyBattleRoundResult(
                Array.Empty<CombatActionResult>(),
                partyWasDefeated: false,
                enemiesWereDefeated: enemiesDefeated);
        }

        private sealed class AlwaysHitCombatRandom : ICombatRandom
        {
            public bool RollPercent(int chancePercent)
            {
                return chancePercent > 0;
            }
        }
    }
}
