using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class GameSessionTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void TryStartNewGameRejectsBlankNames(string name)
        {
            GameSession session = new GameSession();

            bool started = session.TryStartNewGame(name);

            Assert.That(started, Is.False);
            Assert.That(session.HasPlayer, Is.False);
            Assert.That(session.BattleIsOver, Is.True);
            Assert.That(session.BattleOutcome, Is.EqualTo(BattleOutcome.None));
        }

        [Test]
        public void TryStartNewGameTrimsNameAndResetsBattleState()
        {
            GameSession session = new GameSession();

            bool started = session.TryStartNewGame("  Angel  ");

            Assert.That(started, Is.True);
            Assert.That(session.Player.Name, Is.EqualTo("Angel"));
            Assert.That(session.Monster, Is.Null);
            Assert.That(session.BattleIsOver, Is.True);
            Assert.That(session.HasActiveBattle, Is.False);
            Assert.That(session.BattleOutcome, Is.EqualTo(BattleOutcome.None));
        }

        [Test]
        public void NewGameCreatesStableActiveProtagonistCombatant()
        {
            GameSession session = new GameSession();

            session.TryStartNewGame("Angel");

            PlayableCharacterData protagonist = session.Party.GetCharacter(
                PlayableCharacterData.ProtagonistId);
            Assert.That(protagonist, Is.Not.Null);
            Assert.That(protagonist.Name, Is.EqualTo("Angel"));
            Assert.That(protagonist.Stats, Is.SameAs(session.Player.Stats));
            Assert.That(
                session.Party.ActiveCharacterIds,
                Is.EqualTo(new[] { PlayableCharacterData.ProtagonistId }));
            Assert.That(PartyMemberCatalog.Get(protagonist.Id), Is.Null);
        }

        [Test]
        public void StartBattleRequiresLivingPlayerAndMonster()
        {
            GameSession session = new GameSession();
            MonsterData monster = CreateGoblin();

            Assert.That(session.StartBattle(monster), Is.False);

            session.TryStartNewGame("Angel");
            Assert.That(session.StartBattle(null), Is.False);

            session.Player.CurrentHp = 0;
            Assert.That(session.StartBattle(monster), Is.False);
        }

        [Test]
        public void StartBattleCreatesActiveBattle()
        {
            GameSession session = CreateSessionWithPlayer();
            MonsterData monster = CreateGoblin();

            bool started = session.StartBattle(monster);

            Assert.That(started, Is.True);
            Assert.That(session.Monster, Is.SameAs(monster));
            Assert.That(session.BattleIsOver, Is.False);
            Assert.That(session.HasActiveBattle, Is.True);
            Assert.That(
                session.BattleOutcome,
                Is.EqualTo(BattleOutcome.InProgress));
            Assert.That(session.PartyBattle, Is.Not.Null);
            Assert.That(session.PartyBattle.PartyMembers, Has.Count.EqualTo(1));
            Assert.That(session.PartyBattle.Enemies, Is.EqualTo(new[] { monster }));
        }

        [Test]
        public void StartBattleUsesAllActivePartyMembers()
        {
            GameSession session = CreateSessionWithPlayer();
            PlayableCharacterData companion =
                PartyMemberCatalog.Get("pc_01").CreateCharacter();
            session.Party.TryAddCharacter(companion);
            session.Party.TrySetActiveParty(new[]
            {
                PlayableCharacterData.ProtagonistId,
                companion.Id
            });

            bool started = session.StartBattle(CreateGoblin());

            Assert.That(started, Is.True);
            Assert.That(
                session.PartyBattle.PartyMembers.Select(
                    member => member.CombatantId),
                Is.EqualTo(new[]
                {
                    PlayableCharacterData.ProtagonistId,
                    "pc_01"
                }));
        }

        [Test]
        public void TryCompleteVictoryRejectsLivingMonster()
        {
            GameSession session = CreateSessionWithPlayer();
            session.StartBattle(CreateGoblin());

            bool completed = session.TryCompleteVictory(out BattleRewardResult rewards);

            Assert.That(completed, Is.False);
            Assert.That(rewards, Is.Null);
            Assert.That(session.HasActiveBattle, Is.True);
            Assert.That(session.Player.Gold, Is.Zero);
            Assert.That(session.Player.XP, Is.Zero);
        }

        [Test]
        public void TryCompleteVictoryGrantsRewardsExactlyOnce()
        {
            GameSession session = CreateSessionWithPlayer();
            session.StartBattle(CreateGoblin());
            session.Monster.CurrentHp = 0;

            bool firstCompletion = session.TryCompleteVictory(out BattleRewardResult rewards);
            bool secondCompletion = session.TryCompleteVictory(out BattleRewardResult duplicateRewards);

            Assert.That(firstCompletion, Is.True);
            Assert.That(rewards.Gold, Is.EqualTo(10));
            Assert.That(rewards.XP, Is.EqualTo(15));
            Assert.That(rewards.PlayerLeveledUp, Is.False);
            Assert.That(session.Player.Gold, Is.EqualTo(10));
            Assert.That(session.Player.XP, Is.EqualTo(15));
            Assert.That(session.BattleIsOver, Is.True);
            Assert.That(session.HasActiveBattle, Is.False);
            Assert.That(
                session.BattleOutcome,
                Is.EqualTo(BattleOutcome.Victory));

            Assert.That(secondCompletion, Is.False);
            Assert.That(duplicateRewards, Is.Null);
            Assert.That(session.Player.Gold, Is.EqualTo(10));
            Assert.That(session.Player.XP, Is.EqualTo(15));
        }

        [Test]
        public void TryCompleteVictoryReportsLevelUp()
        {
            GameSession session = CreateSessionWithPlayer();
            MonsterData monster = new MonsterData("Ogre", 1, 1, 0, 20, 50);
            session.StartBattle(monster);
            monster.CurrentHp = 0;

            bool completed = session.TryCompleteVictory(out BattleRewardResult rewards);

            Assert.That(completed, Is.True);
            Assert.That(rewards.PlayerLeveledUp, Is.True);
            Assert.That(session.Player.Level, Is.EqualTo(2));
            Assert.That(session.Player.XP, Is.Zero);
        }

        [Test]
        public void CompleteDefeatEndsBattleOnlyWhenPlayerIsDefeated()
        {
            GameSession session = CreateSessionWithPlayer();
            session.StartBattle(CreateGoblin());

            session.CompleteDefeat();
            Assert.That(session.HasActiveBattle, Is.True);

            session.Player.CurrentHp = 0;
            session.CompleteDefeat();

            Assert.That(session.BattleIsOver, Is.True);
            Assert.That(session.HasActiveBattle, Is.False);
            Assert.That(
                session.BattleOutcome,
                Is.EqualTo(BattleOutcome.Defeat));
        }

        [Test]
        public void LivingCompanionPreventsPartyDefeat()
        {
            GameSession session = CreateSessionWithPlayer();
            PlayableCharacterData companion =
                PartyMemberCatalog.Get("pc_01").CreateCharacter();
            session.Party.TryAddCharacter(companion);
            session.Party.TrySetActiveParty(new[]
            {
                PlayableCharacterData.ProtagonistId,
                companion.Id
            });
            session.StartBattle(CreateGoblin());
            session.Player.CurrentHp = 0;

            session.CompleteDefeat();

            Assert.That(session.HasActiveBattle, Is.True);

            companion.Stats.CurrentHp = 0;
            session.CompleteDefeat();
            Assert.That(session.BattleOutcome, Is.EqualTo(BattleOutcome.Defeat));
        }

        [Test]
        public void CompleteEscapeEndsActiveBattleWithoutRewards()
        {
            GameSession session = CreateSessionWithPlayer();
            session.StartBattle(CreateGoblin());

            bool escaped = session.CompleteEscape();

            Assert.That(escaped, Is.True);
            Assert.That(session.HasActiveBattle, Is.False);
            Assert.That(session.BattleIsOver, Is.True);
            Assert.That(
                session.BattleOutcome,
                Is.EqualTo(BattleOutcome.Escaped));
            Assert.That(session.Player.Gold, Is.Zero);
            Assert.That(session.Player.XP, Is.Zero);
        }

        [Test]
        public void StartBattleDoesNotReplaceActiveMonster()
        {
            GameSession session = CreateSessionWithPlayer();
            MonsterData goblin = CreateGoblin();
            MonsterData replacement =
                new MonsterData("Ogre", 50, 12, 3, 20, 25);
            session.StartBattle(goblin);

            bool replaced = session.StartBattle(replacement);

            Assert.That(replaced, Is.False);
            Assert.That(session.Monster, Is.SameAs(goblin));
        }

        private static GameSession CreateSessionWithPlayer()
        {
            GameSession session = new GameSession();
            session.TryStartNewGame("Angel");
            return session;
        }

        private static MonsterData CreateGoblin()
        {
            return new MonsterData("Goblin", 35, 8, 1, 10, 15);
        }
    }
}
