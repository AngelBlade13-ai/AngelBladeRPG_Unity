using System;
using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class PartyBattleStateTests
    {
        [Test]
        public void BattleUsesActiveRosterOrderAndExcludesBenchedMembers()
        {
            PartyRoster roster = CreateRoster(3);
            roster.TrySetActiveParty(new[] { "member-2", "member-1" });

            PartyBattleState battle = PartyBattleState.FromRoster(
                roster,
                new[] { CreateEnemy("enemy-1") });

            Assert.That(
                battle.PartyMembers.Select(member => member.CombatantId),
                Is.EqualTo(new[] { "member-2", "member-1" }));
            Assert.That(battle.GetCombatant("member-3"), Is.Null);
        }

        [Test]
        public void PartyActorCanTargetLivingEnemies()
        {
            PlayableCharacterData hero = CreateMember("hero");
            MonsterData goblin = CreateEnemy("goblin");
            MonsterData wisp = CreateEnemy("wisp");
            PartyBattleState battle = new PartyBattleState(
                new[] { hero },
                new[] { goblin, wisp });

            ICombatant[] targets = battle.GetValidTargets(
                hero.Id,
                BattleTargetType.SingleEnemy).ToArray();

            Assert.That(targets, Is.EqualTo(new ICombatant[] { goblin, wisp }));
        }

        [Test]
        public void EnemyTargetingTreatsOtherEnemiesAsAllies()
        {
            PlayableCharacterData hero = CreateMember("hero");
            MonsterData goblin = CreateEnemy("goblin");
            MonsterData wisp = CreateEnemy("wisp");
            PartyBattleState battle = new PartyBattleState(
                new[] { hero },
                new[] { goblin, wisp });

            ICombatant[] allies = battle.GetValidTargets(
                goblin.CombatantId,
                BattleTargetType.SingleAlly).ToArray();
            ICombatant[] opponents = battle.GetValidTargets(
                goblin.CombatantId,
                BattleTargetType.SingleEnemy).ToArray();

            Assert.That(allies, Is.EqualTo(new ICombatant[] { goblin, wisp }));
            Assert.That(opponents, Is.EqualTo(new ICombatant[] { hero }));
        }

        [Test]
        public void IncapacitatedTargetsAreSeparatedFromLivingAllies()
        {
            PlayableCharacterData hero = CreateMember("hero");
            PlayableCharacterData ally = CreateMember("ally");
            ally.Stats.CurrentHp = 0;
            PartyBattleState battle = new PartyBattleState(
                new[] { hero, ally },
                new[] { CreateEnemy("enemy") });

            Assert.That(
                battle.GetValidTargets(hero.Id, BattleTargetType.SingleAlly),
                Is.EqualTo(new ICombatant[] { hero }));
            Assert.That(
                battle.GetValidTargets(
                    hero.Id,
                    BattleTargetType.IncapacitatedAlly),
                Is.EqualTo(new ICombatant[] { ally }));
        }

        [Test]
        public void IncapacitatedActorCannotSelectTargets()
        {
            PlayableCharacterData hero = CreateMember("hero");
            hero.Stats.CurrentHp = 0;
            PartyBattleState battle = new PartyBattleState(
                new[] { hero },
                new[] { CreateEnemy("enemy") });

            Assert.That(
                battle.GetValidTargets(hero.Id, BattleTargetType.SingleEnemy),
                Is.Empty);
        }

        [Test]
        public void TrySelectTargetRejectsWrongSideOrUnknownTarget()
        {
            PlayableCharacterData hero = CreateMember("hero");
            PlayableCharacterData ally = CreateMember("ally");
            MonsterData enemy = CreateEnemy("enemy");
            PartyBattleState battle = new PartyBattleState(
                new[] { hero, ally },
                new[] { enemy });

            bool wrongSide = battle.TrySelectTarget(
                hero.Id,
                BattleTargetType.SingleEnemy,
                ally.Id,
                out ICombatant rejected);
            bool selected = battle.TrySelectTarget(
                hero.Id,
                BattleTargetType.SingleEnemy,
                enemy.CombatantId,
                out ICombatant target);

            Assert.That(wrongSide, Is.False);
            Assert.That(rejected, Is.Null);
            Assert.That(selected, Is.True);
            Assert.That(target, Is.SameAs(enemy));
        }

        [Test]
        public void AllTargetCommandsUseTargetListInsteadOfSingleSelection()
        {
            PlayableCharacterData hero = CreateMember("hero");
            MonsterData first = CreateEnemy("first");
            MonsterData second = CreateEnemy("second");
            PartyBattleState battle = new PartyBattleState(
                new[] { hero },
                new[] { first, second });

            bool selected = battle.TrySelectTarget(
                hero.Id,
                BattleTargetType.AllEnemies,
                first.CombatantId,
                out ICombatant target);

            Assert.That(selected, Is.False);
            Assert.That(target, Is.Null);
            Assert.That(
                battle.GetValidTargets(hero.Id, BattleTargetType.AllEnemies),
                Is.EqualTo(new ICombatant[] { first, second }));
        }

        [Test]
        public void DefeatFlagsRequireEveryMemberOfThatSideToBeDown()
        {
            PlayableCharacterData hero = CreateMember("hero");
            PlayableCharacterData ally = CreateMember("ally");
            MonsterData enemy = CreateEnemy("enemy");
            PartyBattleState battle = new PartyBattleState(
                new[] { hero, ally },
                new[] { enemy });

            hero.Stats.CurrentHp = 0;
            enemy.Stats.CurrentHp = 0;

            Assert.That(battle.IsPartyDefeated, Is.False);
            Assert.That(battle.AreEnemiesDefeated, Is.True);

            ally.Stats.CurrentHp = 0;
            Assert.That(battle.IsPartyDefeated, Is.True);
        }

        [Test]
        public void DuplicateCombatantIdsAcrossSidesAreRejected()
        {
            PlayableCharacterData hero = CreateMember("shared-id");
            MonsterData enemy = CreateEnemy("shared-id");

            Assert.That(
                () => new PartyBattleState(new[] { hero }, new[] { enemy }),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ReinforcementsCanJoinAndReplaceDefeatedWave()
        {
            PlayableCharacterData hero = CreateMember("hero");
            PlayableCharacterData ally = CreateMember("ally");
            MonsterData firstWave = CreateEnemy("wave-one");
            PartyBattleState battle = new PartyBattleState(
                new[] { hero },
                new[] { firstWave });
            firstWave.CurrentHp = 0;
            MonsterData secondWave = CreateEnemy("wave-two");

            bool added = battle.TryAddPartyMember(ally);
            bool replaced = battle.TryReplaceEnemies(new[] { secondWave });

            Assert.That(added, Is.True);
            Assert.That(replaced, Is.True);
            Assert.That(battle.PartyMembers, Is.EqualTo(new[] { hero, ally }));
            Assert.That(battle.Enemies, Is.EqualTo(new[] { secondWave }));
            Assert.That(battle.GetCombatant(firstWave.Id), Is.Null);
            Assert.That(battle.AreEnemiesDefeated, Is.False);
        }

        [Test]
        public void MoreThanFourPartyMembersAreRejected()
        {
            PlayableCharacterData[] party = Enumerable.Range(1, 5)
                .Select(index => CreateMember($"member-{index}"))
                .ToArray();

            Assert.That(
                () => new PartyBattleState(
                    party,
                    new[] { CreateEnemy("enemy") }),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void BattleRequiresAtLeastOneMemberOnEachSide()
        {
            Assert.That(
                () => new PartyBattleState(
                    Array.Empty<ICombatant>(),
                    new[] { CreateEnemy("enemy") }),
                Throws.TypeOf<ArgumentException>());
            Assert.That(
                () => new PartyBattleState(
                    new[] { CreateMember("hero") },
                    Array.Empty<ICombatant>()),
                Throws.TypeOf<ArgumentException>());
        }

        private static PartyRoster CreateRoster(int count)
        {
            PartyRoster roster = new PartyRoster();
            for (int index = 1; index <= count; index++)
            {
                roster.TryAddCharacter(CreateMember($"member-{index}"));
            }

            return roster;
        }

        private static PlayableCharacterData CreateMember(string id)
        {
            return new PlayableCharacterData(id, id, JobId.Mercenary);
        }

        private static MonsterData CreateEnemy(string id)
        {
            return new MonsterData(
                id,
                id,
                30,
                8,
                2,
                5,
                5,
                8,
                0,
                0,
                0);
        }
    }
}
