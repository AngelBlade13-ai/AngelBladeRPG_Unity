using System;
using System.Collections.Generic;

public enum BattleTimingMode
{
    Wait,
    Active
}

public sealed class ActionGaugeBattle
{
    public const float MaximumGauge = 100f;
    public const float FillPerSpeedPerSecond = 2f;

    private readonly PartyBattleState battle;
    private readonly Dictionary<string, GaugeEntry> gauges =
        new Dictionary<string, GaugeEntry>(StringComparer.Ordinal);
    private long nextReadyOrder;

    public BattleTimingMode TimingMode { get; set; }

    public ActionGaugeBattle(
        PartyBattleState battleState,
        BattleTimingMode timingMode = BattleTimingMode.Wait)
    {
        battle = battleState ??
            throw new ArgumentNullException(nameof(battleState));
        TimingMode = timingMode;
        SynchronizeCombatants();
    }

    public void Tick(float deltaTime, bool commandMenuIsOpen)
    {
        if (deltaTime < 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime));
        }

        SynchronizeCombatants();
        if (deltaTime == 0f ||
            (TimingMode == BattleTimingMode.Wait && commandMenuIsOpen) ||
            battle.IsPartyDefeated || battle.AreEnemiesDefeated)
        {
            return;
        }

        List<GaugeCrossing> crossings = new List<GaugeCrossing>();
        int stableIndex = 0;
        AddGaugeProgress(battle.PartyMembers, deltaTime, crossings, ref stableIndex);
        AddGaugeProgress(battle.Enemies, deltaTime, crossings, ref stableIndex);
        crossings.Sort(CompareCrossings);

        foreach (GaugeCrossing crossing in crossings)
        {
            GaugeEntry entry = gauges[crossing.CombatantId];
            if (!entry.ReadyOrder.HasValue)
            {
                entry.ReadyOrder = nextReadyOrder++;
            }
        }
    }

    public ICombatant GetNextReadyCombatant()
    {
        return FindNextReady(includeParty: true, includeEnemies: true);
    }

    public ICombatant GetNextReadyEnemy()
    {
        return FindNextReady(includeParty: false, includeEnemies: true);
    }

    public bool IsReady(string combatantId)
    {
        return TryGetEntry(combatantId, out GaugeEntry entry) &&
            entry.ReadyOrder.HasValue;
    }

    public float GetGauge(string combatantId)
    {
        return TryGetEntry(combatantId, out GaugeEntry entry)
            ? entry.Value
            : 0f;
    }

    public int GetGaugePercent(string combatantId)
    {
        return (int)Math.Round(
            (GetGauge(combatantId) / MaximumGauge) * 100f,
            MidpointRounding.AwayFromZero);
    }

    public bool ConsumeTurn(string combatantId)
    {
        if (!TryGetEntry(combatantId, out GaugeEntry entry) ||
            !entry.ReadyOrder.HasValue)
        {
            return false;
        }

        entry.Value = 0f;
        entry.ReadyOrder = null;
        return true;
    }

    public void ResetAll()
    {
        SynchronizeCombatants();
        foreach (GaugeEntry entry in gauges.Values)
        {
            entry.Value = 0f;
            entry.ReadyOrder = null;
        }

        nextReadyOrder = 0;
    }

    public void SynchronizeCombatants()
    {
        HashSet<string> activeIds = new HashSet<string>(StringComparer.Ordinal);
        SynchronizeGroup(battle.PartyMembers, activeIds);
        SynchronizeGroup(battle.Enemies, activeIds);

        List<string> removedIds = new List<string>();
        foreach (string combatantId in gauges.Keys)
        {
            if (!activeIds.Contains(combatantId))
            {
                removedIds.Add(combatantId);
            }
        }

        foreach (string combatantId in removedIds)
        {
            gauges.Remove(combatantId);
        }
    }

    private void AddGaugeProgress(
        IReadOnlyList<ICombatant> combatants,
        float deltaTime,
        ICollection<GaugeCrossing> crossings,
        ref int stableIndex)
    {
        foreach (ICombatant combatant in combatants)
        {
            GaugeEntry entry = gauges[combatant.CombatantId];
            int currentStableIndex = stableIndex++;
            if (combatant.Stats.CurrentHp <= 0)
            {
                entry.Value = 0f;
                entry.ReadyOrder = null;
                continue;
            }

            if (entry.ReadyOrder.HasValue)
            {
                continue;
            }

            float fillRate = Math.Max(1, combatant.Stats.Speed) *
                FillPerSpeedPerSecond;
            float remaining = MaximumGauge - entry.Value;
            float timeToReady = remaining / fillRate;
            entry.Value = Math.Min(
                MaximumGauge,
                entry.Value + (fillRate * deltaTime));
            if (entry.Value >= MaximumGauge)
            {
                crossings.Add(new GaugeCrossing(
                    combatant.CombatantId,
                    timeToReady,
                    currentStableIndex));
            }
        }
    }

    private ICombatant FindNextReady(bool includeParty, bool includeEnemies)
    {
        SynchronizeCombatants();
        ICombatant selected = null;
        long selectedOrder = long.MaxValue;

        if (includeParty)
        {
            FindNextInGroup(battle.PartyMembers, ref selected, ref selectedOrder);
        }

        if (includeEnemies)
        {
            FindNextInGroup(battle.Enemies, ref selected, ref selectedOrder);
        }

        return selected;
    }

    private void FindNextInGroup(
        IReadOnlyList<ICombatant> combatants,
        ref ICombatant selected,
        ref long selectedOrder)
    {
        foreach (ICombatant combatant in combatants)
        {
            GaugeEntry entry = gauges[combatant.CombatantId];
            if (combatant.Stats.CurrentHp > 0 &&
                entry.ReadyOrder.HasValue &&
                entry.ReadyOrder.Value < selectedOrder)
            {
                selected = combatant;
                selectedOrder = entry.ReadyOrder.Value;
            }
        }
    }

    private void SynchronizeGroup(
        IReadOnlyList<ICombatant> combatants,
        ISet<string> activeIds)
    {
        foreach (ICombatant combatant in combatants)
        {
            activeIds.Add(combatant.CombatantId);
            if (!gauges.ContainsKey(combatant.CombatantId))
            {
                gauges.Add(combatant.CombatantId, new GaugeEntry());
            }
        }
    }

    private bool TryGetEntry(string combatantId, out GaugeEntry entry)
    {
        entry = null;
        return !string.IsNullOrWhiteSpace(combatantId) &&
            gauges.TryGetValue(combatantId.Trim(), out entry);
    }

    private static int CompareCrossings(GaugeCrossing left, GaugeCrossing right)
    {
        int timeComparison = left.TimeToReady.CompareTo(right.TimeToReady);
        return timeComparison != 0
            ? timeComparison
            : left.StableIndex.CompareTo(right.StableIndex);
    }

    private sealed class GaugeEntry
    {
        public float Value;
        public long? ReadyOrder;
    }

    private sealed class GaugeCrossing
    {
        public string CombatantId { get; }
        public float TimeToReady { get; }
        public int StableIndex { get; }

        public GaugeCrossing(
            string combatantId,
            float timeToReady,
            int stableIndex)
        {
            CombatantId = combatantId;
            TimeToReady = timeToReady;
            StableIndex = stableIndex;
        }
    }
}
