using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class ActionGaugeBattleTests
    {
        [Test]
        public void FasterCombatantBecomesReadyFirst()
        {
            PlayableCharacterData hero = Member("hero", speed: 20);
            MonsterData enemy = Enemy("enemy", speed: 10);
            ActionGaugeBattle gauges = Gauges(hero, enemy);

            gauges.Tick(2.5f, commandMenuIsOpen: false);

            Assert.That(gauges.GetNextReadyCombatant(), Is.SameAs(hero));
            Assert.That(gauges.GetGaugePercent(hero.Id), Is.EqualTo(100));
            Assert.That(gauges.GetGaugePercent(enemy.CombatantId), Is.EqualTo(50));
        }

        [Test]
        public void WaitModePausesEveryGaugeWhileCommandMenuIsOpen()
        {
            ActionGaugeBattle gauges = Gauges(
                Member("hero", 10),
                Enemy("enemy", 10),
                BattleTimingMode.Wait);

            gauges.Tick(3f, commandMenuIsOpen: true);

            Assert.That(gauges.GetGauge("hero"), Is.EqualTo(0f));
            Assert.That(gauges.GetGauge("enemy"), Is.EqualTo(0f));
        }

        [Test]
        public void ActiveModeContinuesEveryGaugeWhileCommandMenuIsOpen()
        {
            ActionGaugeBattle gauges = Gauges(
                Member("hero", 10),
                Enemy("enemy", 10),
                BattleTimingMode.Active);

            gauges.Tick(3f, commandMenuIsOpen: true);

            Assert.That(gauges.GetGauge("hero"), Is.EqualTo(60f));
            Assert.That(gauges.GetGauge("enemy"), Is.EqualTo(60f));
        }

        [Test]
        public void ConsumingTurnResetsOnlyThatCombatantsGauge()
        {
            PlayableCharacterData hero = Member("hero", 10);
            MonsterData enemy = Enemy("enemy", 10);
            ActionGaugeBattle gauges = Gauges(hero, enemy);
            gauges.Tick(5f, commandMenuIsOpen: false);

            bool consumed = gauges.ConsumeTurn(hero.Id);

            Assert.That(consumed, Is.True);
            Assert.That(gauges.GetGauge(hero.Id), Is.EqualTo(0f));
            Assert.That(gauges.IsReady(enemy.CombatantId), Is.True);
        }

        [Test]
        public void EqualReadinessUsesStablePartyThenEnemyOrder()
        {
            PlayableCharacterData hero = Member("hero", 10);
            MonsterData enemy = Enemy("enemy", 10);
            ActionGaugeBattle gauges = Gauges(hero, enemy);
            gauges.Tick(5f, commandMenuIsOpen: false);

            Assert.That(gauges.GetNextReadyCombatant(), Is.SameAs(hero));
            gauges.ConsumeTurn(hero.Id);
            Assert.That(gauges.GetNextReadyCombatant(), Is.SameAs(enemy));
        }

        [Test]
        public void SimultaneouslyReadyEnemiesKeepSeparateTurns()
        {
            PlayableCharacterData hero = Member("hero", 5);
            MonsterData first = Enemy("first", 10);
            MonsterData second = Enemy("second", 10);
            PartyBattleState battle = new PartyBattleState(
                new[] { hero },
                new[] { first, second });
            ActionGaugeBattle gauges = new ActionGaugeBattle(battle);
            gauges.Tick(5f, commandMenuIsOpen: false);

            ICombatant firstReady = gauges.GetNextReadyEnemy();
            gauges.ConsumeTurn(firstReady.CombatantId);
            ICombatant secondReady = gauges.GetNextReadyEnemy();

            Assert.That(firstReady, Is.SameAs(first));
            Assert.That(secondReady, Is.SameAs(second));
            Assert.That(gauges.IsReady(second.CombatantId), Is.True);
        }

        [Test]
        public void SynchronizeAddsReinforcementsAndRemovesDepartedEnemies()
        {
            PlayableCharacterData hero = Member("hero", 10);
            MonsterData first = Enemy("first", 10);
            PartyBattleState battle = new PartyBattleState(
                new[] { hero },
                new[] { first });
            ActionGaugeBattle gauges = new ActionGaugeBattle(battle);
            MonsterData replacement = Enemy("replacement", 10);

            Assert.That(battle.TryReplaceEnemies(new[] { replacement }), Is.True);
            gauges.SynchronizeCombatants();

            Assert.That(gauges.GetGauge(first.CombatantId), Is.EqualTo(0f));
            gauges.Tick(1f, commandMenuIsOpen: false);
            Assert.That(gauges.GetGauge(replacement.CombatantId), Is.EqualTo(20f));
        }

        [Test]
        public void ZeroSpeedStillEventuallyReceivesATurn()
        {
            PlayableCharacterData hero = Member("hero", 0);
            ActionGaugeBattle gauges = Gauges(hero, Enemy("enemy", 0));

            gauges.Tick(50f, commandMenuIsOpen: false);

            Assert.That(gauges.IsReady(hero.Id), Is.True);
        }

        [Test]
        public void SessionDefaultsToWaitAndCanSelectActiveMode()
        {
            GameSession session = new GameSession();

            Assert.That(session.BattleTimingMode, Is.EqualTo(BattleTimingMode.Wait));
            session.SetBattleTimingMode(BattleTimingMode.Active);
            Assert.That(session.BattleTimingMode, Is.EqualTo(BattleTimingMode.Active));
        }

        private static ActionGaugeBattle Gauges(
            PlayableCharacterData hero,
            MonsterData enemy,
            BattleTimingMode mode = BattleTimingMode.Wait)
        {
            return new ActionGaugeBattle(
                new PartyBattleState(new[] { hero }, new[] { enemy }),
                mode);
        }

        private static PlayableCharacterData Member(string id, int speed)
        {
            return new PlayableCharacterData(
                id,
                id,
                JobId.Mercenary,
                new CombatantStats(100, 10, 2, speed, 20, 5, 2),
                applyJobModifiers: false);
        }

        private static MonsterData Enemy(string id, int speed)
        {
            return new MonsterData(
                id,
                id,
                100,
                8,
                1,
                0,
                0,
                speed,
                0,
                0,
                0);
        }
    }
}
