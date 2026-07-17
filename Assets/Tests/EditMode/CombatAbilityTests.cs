using System;
using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class CombatAbilityTests
    {
        [Test]
        public void CatalogContainsTheFirstFiveCoreJobActions()
        {
            Assert.That(CombatAbilityCatalog.All.Count, Is.EqualTo(5));
            Assert.That(
                CombatAbilityCatalog.GetCoreForJob(JobId.Mercenary).StableId,
                Is.EqualTo(CombatAbilityCatalog.PowerStrikeId));
            Assert.That(
                CombatAbilityCatalog.GetCoreForJob(JobId.Mage).StableId,
                Is.EqualTo(CombatAbilityCatalog.EmberId));
            Assert.That(
                CombatAbilityCatalog.GetCoreForJob(JobId.BloodMage).StableId,
                Is.EqualTo(CombatAbilityCatalog.BloodBoltId));
            Assert.That(
                CombatAbilityCatalog.GetCoreForJob(JobId.WhiteMage).StableId,
                Is.EqualTo(CombatAbilityCatalog.MendId));
            Assert.That(
                CombatAbilityCatalog.GetCoreForJob(JobId.Paladin).StableId,
                Is.EqualTo(CombatAbilityCatalog.LayOnHandsId));
        }

        [Test]
        public void EmberSpendsMpAndUsesMagicStats()
        {
            PlayableCharacterData mage = Member(
                "mage",
                JobId.Mage,
                magicPower: 10);
            MonsterData enemy = Enemy("enemy", magicDefense: 3);
            CombatAbilityDefinition ember =
                CombatAbilityCatalog.Get(CombatAbilityCatalog.EmberId);

            CombatActionResult result = new CombatAbilityAction(ember).Execute(
                Context(mage, enemy));

            Assert.That(result.Type, Is.EqualTo(CombatActionType.Magic));
            Assert.That(result.Damage, Is.EqualTo(13));
            Assert.That(mage.Stats.CurrentMp, Is.EqualTo(16));
        }

        [Test]
        public void MendSpendsMpAndClampsHealingAtMaximumHp()
        {
            PlayableCharacterData healer = Member(
                "healer",
                JobId.WhiteMage,
                magicPower: 8);
            PlayableCharacterData ally = Member("ally", JobId.Knight);
            ally.Stats.CurrentHp = 90;
            CombatAbilityDefinition mend =
                CombatAbilityCatalog.Get(CombatAbilityCatalog.MendId);

            CombatActionResult result = new CombatAbilityAction(mend).Execute(
                Context(healer, ally));

            Assert.That(result.Type, Is.EqualTo(CombatActionType.Healing));
            Assert.That(result.Healing, Is.EqualTo(10));
            Assert.That(ally.Stats.CurrentHp, Is.EqualTo(100));
            Assert.That(healer.Stats.CurrentMp, Is.EqualTo(16));
        }

        [Test]
        public void BloodBoltSpendsHpInsteadOfMp()
        {
            PlayableCharacterData bloodMage = Member(
                "blood-mage",
                JobId.BloodMage,
                magicPower: 10);
            MonsterData enemy = Enemy("enemy", magicDefense: 2);
            CombatAbilityDefinition bloodBolt =
                CombatAbilityCatalog.Get(CombatAbilityCatalog.BloodBoltId);

            CombatActionResult result =
                new CombatAbilityAction(bloodBolt).Execute(
                    Context(bloodMage, enemy));

            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Damage, Is.EqualTo(18));
            Assert.That(bloodMage.Stats.CurrentHp, Is.EqualTo(92));
            Assert.That(bloodMage.Stats.CurrentMp, Is.EqualTo(20));
        }

        [Test]
        public void HpCostCannotIncapacitateTheCaster()
        {
            PlayableCharacterData bloodMage = Member(
                "blood-mage",
                JobId.BloodMage);
            bloodMage.Stats.CurrentHp = 8;
            MonsterData enemy = Enemy("enemy");
            CombatAbilityDefinition bloodBolt =
                CombatAbilityCatalog.Get(CombatAbilityCatalog.BloodBoltId);

            CombatActionResult result =
                new CombatAbilityAction(bloodBolt).Execute(
                    Context(bloodMage, enemy));

            Assert.That(result.Succeeded, Is.False);
            Assert.That(bloodMage.Stats.CurrentHp, Is.EqualTo(8));
            Assert.That(enemy.CurrentHp, Is.EqualTo(enemy.MaxHp));
        }

        [Test]
        public void PhysicalAbilityIsReducedByAnActiveGuard()
        {
            PlayableCharacterData mercenary = Member(
                "mercenary",
                JobId.Mercenary,
                attack: 10);
            MonsterData enemy = Enemy("enemy", defense: 1);
            CombatAbilityDefinition powerStrike =
                CombatAbilityCatalog.Get(CombatAbilityCatalog.PowerStrikeId);

            CombatActionResult result =
                new CombatAbilityAction(powerStrike).Execute(
                    Context(mercenary, enemy, targetIsGuarding: true));

            Assert.That(result.Damage, Is.EqualTo(7));
            Assert.That(result.WasGuarded, Is.True);
        }

        [Test]
        public void RoundRejectsAbilityFromTheWrongJobBeforeMutation()
        {
            PlayableCharacterData mercenary = Member(
                "hero",
                JobId.Mercenary);
            MonsterData enemy = Enemy("enemy");
            PartyBattleState battle = Battle(mercenary, enemy);

            Assert.That(
                () => Resolver().ResolveRound(
                    battle,
                    new[]
                    {
                        PartyBattleCommand.Ability(
                            mercenary.Id,
                            CombatAbilityCatalog.EmberId,
                            enemy.CombatantId)
                    }),
                Throws.TypeOf<ArgumentException>());
            Assert.That(enemy.CurrentHp, Is.EqualTo(enemy.MaxHp));
            Assert.That(mercenary.Stats.CurrentMp, Is.EqualTo(20));
        }

        [Test]
        public void RoundRejectsInsufficientMpBeforeMutation()
        {
            PlayableCharacterData mage = Member("mage", JobId.Mage);
            mage.Stats.CurrentMp = 3;
            MonsterData enemy = Enemy("enemy");

            Assert.That(
                () => Resolver().ResolveRound(
                    Battle(mage, enemy),
                    new[]
                    {
                        PartyBattleCommand.Ability(
                            mage.Id,
                            CombatAbilityCatalog.EmberId,
                            enemy.CombatantId)
                    }),
                Throws.TypeOf<ArgumentException>());
            Assert.That(enemy.CurrentHp, Is.EqualTo(enemy.MaxHp));
        }

        [Test]
        public void HealingAbilityRequiresALivingAllyTarget()
        {
            PlayableCharacterData healer = Member(
                "healer",
                JobId.WhiteMage);
            MonsterData enemy = Enemy("enemy");

            Assert.That(
                () => Resolver().ResolveRound(
                    Battle(healer, enemy),
                    new[]
                    {
                        PartyBattleCommand.Ability(
                            healer.Id,
                            CombatAbilityCatalog.MendId,
                            enemy.CombatantId)
                    }),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void HealingAbilityResolvesInSharedSpeedOrder()
        {
            PlayableCharacterData healer = Member(
                "healer",
                JobId.WhiteMage,
                speed: 20);
            PlayableCharacterData ally = Member(
                "ally",
                JobId.Knight,
                speed: 10);
            ally.Stats.CurrentHp = 50;
            MonsterData enemy = Enemy("enemy", speed: 5);
            PartyBattleState battle = new PartyBattleState(
                new[] { healer, ally },
                new[] { enemy });

            PartyBattleRoundResult result = Resolver().ResolveRound(
                battle,
                new[]
                {
                    PartyBattleCommand.Ability(
                        healer.Id,
                        CombatAbilityCatalog.MendId,
                        ally.Id),
                    PartyBattleCommand.Defend(ally.Id)
                });

            Assert.That(result.Actions[0].ActorId, Is.EqualTo(healer.Id));
            Assert.That(result.Actions[0].TargetId, Is.EqualTo(ally.Id));
            Assert.That(result.Actions[0].Healing, Is.GreaterThan(0));
        }

        [Test]
        public void BloodSpellFailsSafelyIfEarlierDamageMakesCostUnsafe()
        {
            PlayableCharacterData bloodMage = Member(
                "blood-mage",
                JobId.BloodMage,
                speed: 5);
            bloodMage.Stats.CurrentHp = 15;
            MonsterData enemy = Enemy(
                "enemy",
                attack: 8,
                speed: 20);

            PartyBattleRoundResult result = Resolver().ResolveRound(
                Battle(bloodMage, enemy),
                new[]
                {
                    PartyBattleCommand.Ability(
                        bloodMage.Id,
                        CombatAbilityCatalog.BloodBoltId,
                        enemy.CombatantId)
                });

            CombatActionResult bloodBolt = result.Actions.Single(
                action => action.ActorId == bloodMage.Id);
            Assert.That(bloodBolt.Succeeded, Is.False);
            Assert.That(bloodMage.Stats.CurrentHp, Is.EqualTo(7));
            Assert.That(enemy.CurrentHp, Is.EqualTo(enemy.MaxHp));
        }

        private static CombatActionContext Context(
            ICombatant actor,
            ICombatant target,
            bool targetIsGuarding = false)
        {
            return new CombatActionContext(
                actor,
                target,
                new NormalHitRandom(),
                targetIsGuarding);
        }

        private static PartyBattleRoundResolver Resolver()
        {
            return new PartyBattleRoundResolver(
                new MinimumIndexRandom(),
                new NormalHitRandom());
        }

        private static PartyBattleState Battle(
            PlayableCharacterData character,
            MonsterData enemy)
        {
            return new PartyBattleState(
                new[] { character },
                new[] { enemy });
        }

        private static PlayableCharacterData Member(
            string id,
            JobId job,
            int attack = 8,
            int speed = 10,
            int magicPower = 5)
        {
            return new PlayableCharacterData(
                id,
                id,
                job,
                new CombatantStats(
                    100,
                    attack,
                    0,
                    speed,
                    20,
                    magicPower,
                    2),
                applyJobModifiers: false);
        }

        private static MonsterData Enemy(
            string id,
            int attack = 5,
            int defense = 0,
            int speed = 5,
            int magicDefense = 0)
        {
            return new MonsterData(
                id,
                id,
                100,
                attack,
                defense,
                5,
                5,
                speed,
                0,
                0,
                magicDefense);
        }

        private sealed class NormalHitRandom : ICombatRandom
        {
            public bool RollPercent(int chancePercent)
            {
                return chancePercent > 50;
            }
        }

        private sealed class MinimumIndexRandom : ITurnOrderRandom
        {
            public int NextIndex(int minimumInclusive, int maximumExclusive)
            {
                return minimumInclusive;
            }
        }
    }
}
