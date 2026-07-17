using System;
using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class PartyBattleRoundResolverTests
    {
        [Test]
        public void EveryLivingCombatantActsInSpeedOrder()
        {
            PlayableCharacterData fastHero = Member("fast-hero", 20);
            PlayableCharacterData slowHero = Member("slow-hero", 10);
            MonsterData fastEnemy = Enemy("fast-enemy", 15, hp: 100);
            MonsterData slowEnemy = Enemy("slow-enemy", 5, hp: 100);
            PartyBattleState battle = Battle(
                new[] { fastHero, slowHero },
                new[] { fastEnemy, slowEnemy });

            PartyBattleRoundResult result = Resolver().ResolveRound(
                battle,
                new[]
                {
                    PartyBattleCommand.Attack(fastHero.Id, fastEnemy.CombatantId),
                    PartyBattleCommand.Attack(slowHero.Id, fastEnemy.CombatantId)
                });

            Assert.That(
                result.Actions.Select(action => action.ActorId),
                Is.EqualTo(new[]
                {
                    "fast-hero",
                    "fast-enemy",
                    "slow-hero",
                    "slow-enemy"
                }));
        }

        [Test]
        public void IncapacitatedPartyMemberNeedsNoCommandAndCannotAct()
        {
            PlayableCharacterData hero = Member("hero", 15);
            PlayableCharacterData fallen = Member("fallen", 20);
            fallen.Stats.CurrentHp = 0;
            MonsterData enemy = Enemy("enemy", 5, hp: 100);

            PartyBattleRoundResult result = Resolver().ResolveRound(
                Battle(new[] { hero, fallen }, new[] { enemy }),
                new[] { PartyBattleCommand.Attack(hero.Id, enemy.CombatantId) });

            Assert.That(
                result.Actions.Select(action => action.ActorId),
                Does.Not.Contain(fallen.Id));
        }

        [Test]
        public void MissingPartyCommandRejectsRoundBeforeDamageOccurs()
        {
            PlayableCharacterData first = Member("first", 15);
            PlayableCharacterData second = Member("second", 10);
            MonsterData enemy = Enemy("enemy", 5);
            PartyBattleState battle = Battle(
                new[] { first, second },
                new[] { enemy });

            Assert.That(
                () => Resolver().ResolveRound(
                    battle,
                    new[]
                    {
                        PartyBattleCommand.Attack(first.Id, enemy.CombatantId)
                    }),
                Throws.TypeOf<ArgumentException>());
            Assert.That(enemy.CurrentHp, Is.EqualTo(enemy.MaxHp));
        }

        [Test]
        public void DuplicatePartyCommandIsRejected()
        {
            PlayableCharacterData hero = Member("hero", 10);
            MonsterData enemy = Enemy("enemy", 5);

            Assert.That(
                () => Resolver().ResolveRound(
                    Battle(new[] { hero }, new[] { enemy }),
                    new[]
                    {
                        PartyBattleCommand.Attack(hero.Id, enemy.CombatantId),
                        PartyBattleCommand.Defend(hero.Id)
                    }),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void AttackCannotTargetAnAlly()
        {
            PlayableCharacterData hero = Member("hero", 10);
            PlayableCharacterData ally = Member("ally", 8);
            MonsterData enemy = Enemy("enemy", 5);

            Assert.That(
                () => Resolver().ResolveRound(
                    Battle(new[] { hero, ally }, new[] { enemy }),
                    new[]
                    {
                        PartyBattleCommand.Attack(hero.Id, ally.Id),
                        PartyBattleCommand.Defend(ally.Id)
                    }),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void LaterPartyAttackRetargetsAfterOriginalEnemyFalls()
        {
            PlayableCharacterData first = Member("first", 20, attack: 100);
            PlayableCharacterData second = Member("second", 15, attack: 100);
            MonsterData firstEnemy = Enemy("first-enemy", 5, hp: 10);
            MonsterData secondEnemy = Enemy("second-enemy", 4, hp: 10);
            PartyBattleState battle = Battle(
                new[] { first, second },
                new[] { firstEnemy, secondEnemy });

            PartyBattleRoundResult result = Resolver().ResolveRound(
                battle,
                new[]
                {
                    PartyBattleCommand.Attack(first.Id, firstEnemy.CombatantId),
                    PartyBattleCommand.Attack(second.Id, firstEnemy.CombatantId)
                });

            Assert.That(
                result.Actions.Select(action => action.TargetId),
                Is.EqualTo(new[] { "first-enemy", "second-enemy" }));
            Assert.That(result.EnemiesWereDefeated, Is.True);
        }

        [Test]
        public void CombatantDefeatedBeforeItsTurnDoesNotAct()
        {
            PlayableCharacterData first = Member("first", 10, hp: 10);
            PlayableCharacterData second = Member("second", 5);
            MonsterData enemy = Enemy("enemy", 20, attack: 100, hp: 100);

            PartyBattleRoundResult result = Resolver().ResolveRound(
                Battle(new[] { first, second }, new[] { enemy }),
                new[]
                {
                    PartyBattleCommand.Attack(first.Id, enemy.CombatantId),
                    PartyBattleCommand.Attack(second.Id, enemy.CombatantId)
                });

            Assert.That(
                result.Actions.Select(action => action.ActorId),
                Is.EqualTo(new[] { "enemy", "second" }));
        }

        [Test]
        public void LaterEnemyRetargetsAfterOriginalPartyTargetFalls()
        {
            PlayableCharacterData first = Member("first", 5, hp: 10);
            PlayableCharacterData second = Member("second", 4, hp: 10);
            MonsterData firstEnemy = Enemy(
                "first-enemy", 20, attack: 100, hp: 100);
            MonsterData secondEnemy = Enemy(
                "second-enemy", 15, attack: 100, hp: 100);

            PartyBattleRoundResult result = Resolver().ResolveRound(
                Battle(
                    new[] { first, second },
                    new[] { firstEnemy, secondEnemy }),
                new[]
                {
                    PartyBattleCommand.Defend(first.Id),
                    PartyBattleCommand.Defend(second.Id)
                });

            Assert.That(
                result.Actions.Select(action => action.TargetId),
                Is.EqualTo(new[] { "first", "second" }));
            Assert.That(result.PartyWasDefeated, Is.True);
        }

        [Test]
        public void FasterDefendReducesDamageLaterInTheRound()
        {
            PlayableCharacterData hero = Member("hero", 20, hp: 100);
            MonsterData enemy = Enemy("enemy", 10, attack: 10, hp: 100);

            PartyBattleRoundResult result = Resolver().ResolveRound(
                Battle(new[] { hero }, new[] { enemy }),
                new[] { PartyBattleCommand.Defend(hero.Id) });

            Assert.That(result.Actions[1].WasGuarded, Is.True);
            Assert.That(result.Actions[1].Damage, Is.EqualTo(5));
            Assert.That(hero.Stats.CurrentHp, Is.EqualTo(95));
        }

        [Test]
        public void SlowerDefendDoesNotReduceEarlierDamage()
        {
            PlayableCharacterData hero = Member("hero", 5, hp: 100);
            MonsterData enemy = Enemy("enemy", 20, attack: 10, hp: 100);

            PartyBattleRoundResult result = Resolver().ResolveRound(
                Battle(new[] { hero }, new[] { enemy }),
                new[] { PartyBattleCommand.Defend(hero.Id) });

            Assert.That(result.Actions[0].WasGuarded, Is.False);
            Assert.That(result.Actions[0].Damage, Is.EqualTo(10));
            Assert.That(hero.Stats.CurrentHp, Is.EqualTo(90));
        }

        [Test]
        public void EnemyCommandSourceCanChooseAnotherPartyTarget()
        {
            PlayableCharacterData first = Member("first", 5);
            PlayableCharacterData second = Member("second", 4);
            MonsterData enemy = Enemy("enemy", 20, hp: 100);
            PartyBattleRoundResolver resolver = Resolver(
                new FixedTargetCommandSource(second.Id));

            PartyBattleRoundResult result = resolver.ResolveRound(
                Battle(new[] { first, second }, new[] { enemy }),
                new[]
                {
                    PartyBattleCommand.Defend(first.Id),
                    PartyBattleCommand.Defend(second.Id)
                });

            Assert.That(result.Actions[0].TargetId, Is.EqualTo(second.Id));
            Assert.That(first.Stats.CurrentHp, Is.EqualTo(first.Stats.MaxHp));
            Assert.That(second.Stats.CurrentHp, Is.LessThan(second.Stats.MaxHp));
        }

        [Test]
        public void InvalidEnemyCommandSourceIsRejectedBeforeRound()
        {
            PlayableCharacterData hero = Member("hero", 10);
            MonsterData enemy = Enemy("enemy", 5);
            PartyBattleRoundResolver resolver = Resolver(
                new InvalidActorCommandSource());

            Assert.That(
                () => resolver.ResolveRound(
                    Battle(new[] { hero }, new[] { enemy }),
                    new[] { PartyBattleCommand.Defend(hero.Id) }),
                Throws.TypeOf<InvalidOperationException>());
            Assert.That(hero.Stats.CurrentHp, Is.EqualTo(hero.Stats.MaxHp));
        }

        [Test]
        public void AlreadyCompletedBattleProducesNoActions()
        {
            PlayableCharacterData hero = Member("hero", 10);
            MonsterData enemy = Enemy("enemy", 5);
            enemy.CurrentHp = 0;

            PartyBattleRoundResult result = Resolver().ResolveRound(
                Battle(new[] { hero }, new[] { enemy }),
                Array.Empty<PartyBattleCommand>());

            Assert.That(result.Actions, Is.Empty);
            Assert.That(result.EnemiesWereDefeated, Is.True);
        }

        private static PartyBattleRoundResolver Resolver(
            IEnemyBattleCommandSource enemySource = null)
        {
            return new PartyBattleRoundResolver(
                new MinimumIndexRandom(),
                new NormalHitCombatRandom(),
                enemySource);
        }

        private static PartyBattleState Battle(
            PlayableCharacterData[] party,
            MonsterData[] enemies)
        {
            return new PartyBattleState(party, enemies);
        }

        private static PlayableCharacterData Member(
            string id,
            int speed,
            int attack = 8,
            int hp = 100)
        {
            return new PlayableCharacterData(
                id,
                id,
                JobId.Mercenary,
                new CombatantStats(hp, attack, 0, speed, 10, 5, 2),
                applyJobModifiers: false);
        }

        private static MonsterData Enemy(
            string id,
            int speed,
            int attack = 8,
            int hp = 30)
        {
            return new MonsterData(
                id,
                id,
                hp,
                attack,
                0,
                5,
                5,
                speed,
                0,
                0,
                0);
        }

        private sealed class MinimumIndexRandom : ITurnOrderRandom
        {
            public int NextIndex(int minimumInclusive, int maximumExclusive)
            {
                return minimumInclusive;
            }
        }

        private sealed class NormalHitCombatRandom : ICombatRandom
        {
            public bool RollPercent(int chancePercent)
            {
                return chancePercent > 50;
            }
        }

        private sealed class FixedTargetCommandSource : IEnemyBattleCommandSource
        {
            private readonly string targetId;

            public FixedTargetCommandSource(string targetId)
            {
                this.targetId = targetId;
            }

            public PartyBattleCommand CreateCommand(
                ICombatant enemy,
                PartyBattleState battle)
            {
                return PartyBattleCommand.Attack(
                    enemy.CombatantId,
                    targetId);
            }
        }

        private sealed class InvalidActorCommandSource : IEnemyBattleCommandSource
        {
            public PartyBattleCommand CreateCommand(
                ICombatant enemy,
                PartyBattleState battle)
            {
                return PartyBattleCommand.Defend("not-the-enemy");
            }
        }
    }
}
