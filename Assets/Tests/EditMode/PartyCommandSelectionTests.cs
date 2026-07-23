using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class PartyCommandSelectionTests
    {
        [Test]
        public void SelectionBeginsWithFirstLivingPartyMember()
        {
            PlayableCharacterData fallen = Member("fallen");
            fallen.Stats.CurrentHp = 0;
            PlayableCharacterData active = Member("active");

            PartyCommandSelection selection = Selection(
                new[] { fallen, active },
                new[] { Enemy("enemy") });

            Assert.That(selection.CurrentActor, Is.SameAs(active));
            Assert.That(selection.SelectedTarget.CombatantId, Is.EqualTo("enemy"));
            Assert.That(selection.IsComplete, Is.False);
        }

        [Test]
        public void QueuedCommandsAdvanceThroughLivingFormationOrder()
        {
            PlayableCharacterData first = Member("first");
            PlayableCharacterData second = Member("second");
            PartyCommandSelection selection = Selection(
                new[] { first, second },
                new[] { Enemy("enemy") });

            bool firstQueued = selection.TryQueueAttack();
            bool secondQueued = selection.TryQueueDefend();

            Assert.That(firstQueued, Is.True);
            Assert.That(secondQueued, Is.True);
            Assert.That(selection.IsComplete, Is.True);
            Assert.That(
                selection.Commands.Select(command => command.ActorId),
                Is.EqualTo(new[] { "first", "second" }));
        }

        [Test]
        public void TargetCyclingWrapsInBothDirections()
        {
            PartyCommandSelection selection = Selection(
                new[] { Member("hero") },
                new[]
                {
                    Enemy("first"),
                    Enemy("second"),
                    Enemy("third")
                });

            selection.CycleTarget(-1);
            Assert.That(selection.SelectedTarget.CombatantId, Is.EqualTo("third"));

            selection.CycleTarget(1);
            Assert.That(selection.SelectedTarget.CombatantId, Is.EqualTo("first"));
        }

        [Test]
        public void AttackStoresCurrentlySelectedTarget()
        {
            PartyCommandSelection selection = Selection(
                new[] { Member("hero") },
                new[] { Enemy("first"), Enemy("second") });
            selection.CycleTarget(1);

            selection.TryQueueAttack();

            Assert.That(selection.Commands[0].TargetId, Is.EqualTo("second"));
            Assert.That(
                selection.Commands[0].Type,
                Is.EqualTo(PartyBattleCommandType.PhysicalAttack));
        }

        [Test]
        public void DefendDoesNotRequireAnEnemyTarget()
        {
            MonsterData enemy = Enemy("enemy");
            enemy.CurrentHp = 0;
            PartyCommandSelection selection = Selection(
                new[] { Member("hero") },
                new[] { enemy });

            bool queued = selection.TryQueueDefend();

            Assert.That(queued, Is.True);
            Assert.That(selection.Commands[0].TargetId, Is.Empty);
        }

        [Test]
        public void CompletedSelectionRejectsAdditionalCommands()
        {
            PartyCommandSelection selection = Selection(
                new[] { Member("hero") },
                new[] { Enemy("enemy") });
            selection.TryQueueDefend();

            Assert.That(selection.TryQueueAttack(), Is.False);
            Assert.That(selection.TryQueueDefend(), Is.False);
            Assert.That(selection.Commands, Has.Count.EqualTo(1));
        }

        [Test]
        public void SelectionReportsWhetherAnyCommandsHaveBeenQueued()
        {
            PartyCommandSelection selection = Selection(
                new[] { Member("hero") },
                new[] { Enemy("enemy") });

            Assert.That(selection.HasQueuedCommands, Is.False);

            selection.TryQueueDefend();
            Assert.That(selection.HasQueuedCommands, Is.True);
        }

        [Test]
        public void CoreAbilityBeginsWithItsAuthoredTargetType()
        {
            PartyCommandSelection selection = Selection(
                new[] { Member("hero", JobId.Mercenary) },
                new[] { Enemy("enemy") });

            bool began = selection.TryBeginCoreAbility();

            Assert.That(began, Is.True);
            Assert.That(selection.IsChoosingAbility, Is.True);
            Assert.That(
                selection.PendingAbility.StableId,
                Is.EqualTo(CombatAbilityCatalog.PowerStrikeId));
            Assert.That(
                selection.CurrentTargetType,
                Is.EqualTo(BattleTargetType.SingleEnemy));
            Assert.That(selection.SelectedTarget.CombatantId, Is.EqualTo("enemy"));
        }

        [Test]
        public void HealingAbilityCyclesThroughLivingAllies()
        {
            PlayableCharacterData healer = Member(
                "healer",
                JobId.WhiteMage);
            PlayableCharacterData ally = Member("ally", JobId.Knight);
            PartyCommandSelection selection = Selection(
                new[] { healer, ally },
                new[] { Enemy("enemy") });

            selection.TryBeginCoreAbility();
            selection.CycleTarget(1);

            Assert.That(
                selection.CurrentTargetType,
                Is.EqualTo(BattleTargetType.SingleAlly));
            Assert.That(selection.SelectedTarget, Is.SameAs(ally));
        }

        [Test]
        public void ConfirmedAbilityStoresStableAbilityAndTargetIds()
        {
            PlayableCharacterData healer = Member(
                "healer",
                JobId.WhiteMage);
            PlayableCharacterData ally = Member("ally", JobId.Knight);
            PartyCommandSelection selection = Selection(
                new[] { healer, ally },
                new[] { Enemy("enemy") });
            selection.TryBeginCoreAbility();
            selection.CycleTarget(1);

            bool queued = selection.TryQueuePendingAbility();

            Assert.That(queued, Is.True);
            Assert.That(
                selection.Commands[0].Type,
                Is.EqualTo(PartyBattleCommandType.Ability));
            Assert.That(
                selection.Commands[0].AbilityId,
                Is.EqualTo(CombatAbilityCatalog.MendId));
            Assert.That(selection.Commands[0].TargetId, Is.EqualTo(ally.Id));
            Assert.That(selection.IsChoosingAbility, Is.False);
        }

        [Test]
        public void UnaffordableCoreAbilityCannotBeginTargeting()
        {
            PlayableCharacterData mage = Member("mage", JobId.Mage);
            mage.Stats.CurrentMp = 3;
            PartyCommandSelection selection = Selection(
                new[] { mage },
                new[] { Enemy("enemy") });

            Assert.That(selection.TryBeginCoreAbility(), Is.False);
            Assert.That(selection.IsChoosingAbility, Is.False);
        }

        [Test]
        public void AttackCancelsAbilityTargetingAndUsesAnEnemyTarget()
        {
            PlayableCharacterData healer = Member(
                "healer",
                JobId.WhiteMage);
            PartyCommandSelection selection = Selection(
                new[] { healer },
                new[] { Enemy("enemy") });
            selection.TryBeginCoreAbility();

            bool queued = selection.TryQueueAttack();

            Assert.That(queued, Is.True);
            Assert.That(
                selection.Commands[0].Type,
                Is.EqualTo(PartyBattleCommandType.PhysicalAttack));
            Assert.That(selection.Commands[0].TargetId, Is.EqualTo("enemy"));
            Assert.That(selection.IsChoosingAbility, Is.False);
        }

        [Test]
        public void UnsafeBloodCostCannotBeginTargeting()
        {
            PlayableCharacterData bloodMage = Member(
                "blood-mage",
                JobId.BloodMage);
            bloodMage.Stats.CurrentHp = 8;
            PartyCommandSelection selection = Selection(
                new[] { bloodMage },
                new[] { Enemy("enemy") });

            Assert.That(selection.TryBeginCoreAbility(), Is.False);
        }

        [Test]
        public void BattleItemRequiresOwnedPotionAndAnInjuredAlly()
        {
            PlayableCharacterData hero = Member("hero");
            var inventory = new Inventory();
            PartyBattleState battle = new PartyBattleState(
                new[] { hero },
                new[] { Enemy("enemy") });
            var selection = new PartyCommandSelection(
                battle,
                hero,
                inventory);

            Assert.That(selection.TryBeginBattleItem(), Is.False);

            inventory.TryAdd(ItemCatalog.MinorPotionId);
            Assert.That(selection.TryBeginBattleItem(), Is.False);

            hero.Stats.ApplyDamage(10);
            Assert.That(selection.TryBeginBattleItem(), Is.True);
        }

        [Test]
        public void BattleItemTargetsOnlyLivingInjuredAllies()
        {
            PlayableCharacterData actor = Member("actor");
            PlayableCharacterData injured = Member("injured");
            PlayableCharacterData fallen = Member("fallen");
            injured.Stats.ApplyDamage(10);
            fallen.Stats.CurrentHp = 0;
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId);
            var selection = new PartyCommandSelection(
                new PartyBattleState(
                    new[] { actor, injured, fallen },
                    new[] { Enemy("enemy") }),
                actor,
                inventory);

            selection.TryBeginBattleItem();

            Assert.That(selection.GetCurrentTargetCount(), Is.EqualTo(1));
            Assert.That(selection.SelectedTarget, Is.SameAs(injured));
        }

        [Test]
        public void ConfirmedBattleItemStoresStableItemAndTargetIds()
        {
            PlayableCharacterData actor = Member("actor");
            PlayableCharacterData injured = Member("injured");
            injured.Stats.ApplyDamage(10);
            var inventory = new Inventory();
            inventory.TryAdd(ItemCatalog.MinorPotionId, 2);
            var selection = new PartyCommandSelection(
                new PartyBattleState(
                    new[] { actor, injured },
                    new[] { Enemy("enemy") }),
                actor,
                inventory);
            selection.TryBeginBattleItem();

            bool queued = selection.TryQueuePendingItem();

            Assert.That(queued, Is.True);
            Assert.That(
                selection.Commands[0].Type,
                Is.EqualTo(PartyBattleCommandType.Item));
            Assert.That(
                selection.Commands[0].ItemId,
                Is.EqualTo(ItemCatalog.MinorPotionId));
            Assert.That(selection.Commands[0].TargetId, Is.EqualTo(injured.Id));
            Assert.That(selection.IsChoosingItem, Is.False);
        }

        private static PartyCommandSelection Selection(
            PlayableCharacterData[] party,
            MonsterData[] enemies)
        {
            return new PartyCommandSelection(
                new PartyBattleState(party, enemies));
        }

        private static PlayableCharacterData Member(
            string id,
            JobId job = JobId.Mercenary)
        {
            return new PlayableCharacterData(id, id, job);
        }

        private static MonsterData Enemy(string id)
        {
            return new MonsterData(
                id,
                id,
                30,
                8,
                1,
                5,
                5,
                8,
                0,
                0,
                0);
        }
    }
}
