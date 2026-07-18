using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class PartyBattleActionResolverTests
    {
        [Test]
        public void PlayerCommandResolvesImmediatelyWithoutEnemyAction()
        {
            PlayableCharacterData hero = Member("hero", attack: 12);
            MonsterData enemy = Enemy("enemy", attack: 20);
            PartyBattleState battle = Battle(hero, enemy);

            CombatActionResult result = Resolver().ResolveCommand(
                battle,
                PartyBattleCommand.Attack(hero.Id, enemy.CombatantId));

            Assert.That(result.ActorId, Is.EqualTo(hero.Id));
            Assert.That(enemy.CurrentHp, Is.LessThan(enemy.MaxHp));
            Assert.That(
                hero.Stats.CurrentHp,
                Is.EqualTo(hero.Stats.MaxHp));
        }

        [Test]
        public void EnemyReadyActionResolvesWithoutPartyCommandBatch()
        {
            PlayableCharacterData hero = Member("hero");
            MonsterData enemy = Enemy("enemy", attack: 12);
            PartyBattleState battle = Battle(hero, enemy);

            CombatActionResult result = Resolver().ResolveEnemyAction(
                battle,
                enemy);

            Assert.That(result.ActorId, Is.EqualTo(enemy.CombatantId));
            Assert.That(
                hero.Stats.CurrentHp,
                Is.LessThan(hero.Stats.MaxHp));
        }

        [Test]
        public void DefendLastsUntilDefendersNextActionBegins()
        {
            PlayableCharacterData hero = Member("hero");
            MonsterData enemy = Enemy("enemy", attack: 12, hp: 100);
            PartyBattleState battle = Battle(hero, enemy);
            PartyBattleActionResolver resolver = Resolver();

            resolver.ResolveCommand(battle, PartyBattleCommand.Defend(hero.Id));
            CombatActionResult guardedHit = resolver.ResolveEnemyAction(
                battle,
                enemy);
            resolver.ResolveCommand(
                battle,
                PartyBattleCommand.Attack(hero.Id, enemy.CombatantId));
            CombatActionResult normalHit = resolver.ResolveEnemyAction(
                battle,
                enemy);

            Assert.That(guardedHit.WasGuarded, Is.True);
            Assert.That(normalHit.WasGuarded, Is.False);
            Assert.That(normalHit.Damage, Is.GreaterThan(guardedHit.Damage));
        }

        [Test]
        public void ReadyHealerCanResolveAbilityImmediately()
        {
            PlayableCharacterData healer = Member(
                "healer",
                job: JobId.WhiteMage);
            PlayableCharacterData ally = Member("ally");
            ally.Stats.CurrentHp = 20;
            MonsterData enemy = Enemy("enemy");
            PartyBattleState battle = new PartyBattleState(
                new[] { healer, ally },
                new[] { enemy });

            CombatActionResult result = Resolver().ResolveCommand(
                battle,
                PartyBattleCommand.Ability(
                    healer.Id,
                    CombatAbilityCatalog.MendId,
                    ally.Id));

            Assert.That(result.Healing, Is.GreaterThan(0));
            Assert.That(ally.Stats.CurrentHp, Is.GreaterThan(20));
        }

        [Test]
        public void DefeatedRequestedTargetRetargetsToLivingOpponent()
        {
            PlayableCharacterData hero = Member("hero");
            MonsterData fallen = Enemy("fallen");
            MonsterData living = Enemy("living");
            fallen.CurrentHp = 0;
            PartyBattleState battle = new PartyBattleState(
                new[] { hero },
                new[] { fallen, living });

            CombatActionResult result = Resolver().ResolveCommand(
                battle,
                PartyBattleCommand.Attack(hero.Id, fallen.CombatantId));

            Assert.That(result.TargetId, Is.EqualTo(living.CombatantId));
        }

        [Test]
        public void FixedSelectionUsesOnlyTheGaugeReadyPartyMember()
        {
            PlayableCharacterData first = Member("first");
            PlayableCharacterData second = Member("second");
            PartyBattleState battle = new PartyBattleState(
                new[] { first, second },
                new[] { Enemy("enemy") });
            PartyCommandSelection selection =
                new PartyCommandSelection(battle, second);

            selection.TryQueueDefend();

            Assert.That(selection.Commands.Count, Is.EqualTo(1));
            Assert.That(selection.Commands[0].ActorId, Is.EqualTo(second.Id));
            Assert.That(selection.IsComplete, Is.True);
        }

        [Test]
        public void FixedSelectionRejectsEnemyActor()
        {
            PlayableCharacterData hero = Member("hero");
            MonsterData enemy = Enemy("enemy");
            PartyBattleState battle = Battle(hero, enemy);

            Assert.That(
                () => new PartyCommandSelection(battle, enemy),
                Throws.TypeOf<System.ArgumentException>());
        }

        [Test]
        public void AttackStillRejectsLivingAllyAsTarget()
        {
            PlayableCharacterData hero = Member("hero");
            PlayableCharacterData ally = Member("ally");
            PartyBattleState battle = new PartyBattleState(
                new[] { hero, ally },
                new[] { Enemy("enemy") });

            Assert.That(
                () => Resolver().ResolveCommand(
                    battle,
                    PartyBattleCommand.Attack(hero.Id, ally.Id)),
                Throws.TypeOf<System.ArgumentException>());
        }

        [Test]
        public void TauntRedirectsEnemiesUntilReaversNextAction()
        {
            PlayableCharacterData hero = Member("hero");
            PlayableCharacterData reaver = Member(
                "reaver",
                job: JobId.Reaver);
            MonsterData enemy = Enemy("enemy", attack: 12, hp: 100);
            PartyBattleState battle = new PartyBattleState(
                new[] { hero, reaver },
                new[] { enemy });
            PartyBattleActionResolver resolver = Resolver();

            resolver.ResolveCommand(
                battle,
                PartyBattleCommand.Ability(
                    reaver.Id,
                    CombatAbilityCatalog.TauntId,
                    reaver.Id));
            CombatActionResult redirected = resolver.ResolveEnemyAction(
                battle,
                enemy);
            resolver.ResolveCommand(
                battle,
                PartyBattleCommand.Attack(reaver.Id, enemy.CombatantId));
            CombatActionResult afterExpiry = resolver.ResolveEnemyAction(
                battle,
                enemy);

            Assert.That(redirected.TargetId, Is.EqualTo(reaver.Id));
            Assert.That(afterExpiry.TargetId, Is.EqualTo(hero.Id));
            Assert.That(resolver.IsTaunting(reaver.Id), Is.False);
        }

        private static PartyBattleActionResolver Resolver()
        {
            return new PartyBattleActionResolver(new NormalHitRandom());
        }

        private static PartyBattleState Battle(
            PlayableCharacterData hero,
            MonsterData enemy)
        {
            return new PartyBattleState(new[] { hero }, new[] { enemy });
        }

        private static PlayableCharacterData Member(
            string id,
            int attack = 8,
            JobId job = JobId.Mercenary)
        {
            return new PlayableCharacterData(
                id,
                id,
                job,
                new CombatantStats(100, attack, 2, 10, 20, 10, 2),
                applyJobModifiers: false);
        }

        private static MonsterData Enemy(
            string id,
            int attack = 8,
            int hp = 30)
        {
            return new MonsterData(
                id,
                id,
                hp,
                attack,
                0,
                0,
                0,
                8,
                0,
                0,
                0);
        }

        private sealed class NormalHitRandom : ICombatRandom
        {
            public bool RollPercent(int chancePercent)
            {
                return chancePercent > 50;
            }
        }
    }
}
