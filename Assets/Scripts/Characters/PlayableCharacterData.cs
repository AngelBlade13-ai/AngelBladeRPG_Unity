using System;
using System.Collections.Generic;

public enum JobAffinity
{
    Low,
    Neutral,
    High
}

public class PlayableCharacterData : ICombatant
{
    public const string ProtagonistId = "pc_protagonist";

    private readonly Dictionary<JobId, JobAffinity> jobAffinities =
        new Dictionary<JobId, JobAffinity>();
    private readonly JobProgression jobProgression = new JobProgression();
    private readonly CharacterProgression characterProgression;
    private readonly bool applyJobModifiers;
    private readonly StatValues baseStats;
    private StatValues lastEffectiveStats;

    public string Id { get; }
    public string Name { get; }
    public string CombatantId => Id;
    public string DisplayName => Name;
    public CombatantStats Stats { get; }
    public CharacterEquipment Equipment { get; }
    public JobId CurrentJob { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public bool IsIncapacitated => Stats.CurrentHp <= 0;
    public CharacterRosterHistory RosterHistory { get; }
    public int JobPoints => jobProgression.JobPoints;
    public int Level => characterProgression.Level;
    public int XP => characterProgression.XP;
    public int XPToNextLevel => characterProgression.XPToNextLevel;
    public IReadOnlyCollection<string> LearnedJobNodeIds =>
        jobProgression.LearnedNodeIds;

    public PlayableCharacterData(
        string id,
        string name,
        JobId startingJob,
        CombatantStats combatStats = null,
        bool applyJobModifiers = true,
        CharacterProgression characterProgression = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("A character ID is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A character name is required.", nameof(name));
        }

        if (!JobCatalog.Contains(startingJob))
        {
            throw new ArgumentOutOfRangeException(nameof(startingJob));
        }

        Id = id.Trim();
        Name = name.Trim();
        CurrentJob = startingJob;
        Stats = combatStats ?? CreateDefaultCombatStats();
        Equipment = new CharacterEquipment();
        this.characterProgression =
            characterProgression ?? new CharacterProgression();
        this.applyJobModifiers = applyJobModifiers;
        baseStats = StatValues.From(Stats);
        RosterHistory = new CharacterRosterHistory(Id);
        RecalculateEffectiveStats();
    }

    private static CombatantStats CreateDefaultCombatStats()
    {
        // Temporary shared level-one profile until authored growth data lands.
        return new CombatantStats(100, 12, 3, 10, 20, 8, 3);
    }

    public bool TryAssignJob(JobId jobId)
    {
        if (!IsAvailable || !JobCatalog.Contains(jobId) ||
            !Equipment.WeaponIsCompatibleWith(jobId))
        {
            return false;
        }

        CurrentJob = jobId;
        RecalculateEffectiveStats();
        return true;
    }

    public bool TryAssignJob(JobId jobId, Inventory inventory)
    {
        if (!IsAvailable || !JobCatalog.Contains(jobId))
        {
            return false;
        }

        if (!Equipment.WeaponIsCompatibleWith(jobId) &&
            !Equipment.TryUnequip(EquipmentSlot.Weapon, inventory))
        {
            return false;
        }

        CurrentJob = jobId;
        RecalculateEffectiveStats();
        return true;
    }

    public bool TryEquipItem(
        EquipmentSlot slot,
        string itemId,
        Inventory inventory)
    {
        if (!IsAvailable ||
            !Equipment.TryEquip(slot, itemId, CurrentJob, inventory))
        {
            return false;
        }

        RecalculateEffectiveStats();
        return true;
    }

    public bool TryUnequipItem(EquipmentSlot slot, Inventory inventory)
    {
        if (!IsAvailable || !Equipment.TryUnequip(slot, inventory))
        {
            return false;
        }

        RecalculateEffectiveStats();
        return true;
    }

    public bool TryAddJobPoints(int amount)
    {
        return IsAvailable && jobProgression.TryAddJobPoints(amount);
    }

    public int GainXP(int amount)
    {
        if (!IsAvailable)
        {
            return 0;
        }

        int levelsGained = characterProgression.GainXP(amount, Stats);
        if (levelsGained > 0)
        {
            CaptureExternalBaseStatChanges();
            lastEffectiveStats = StatValues.From(Stats);
        }

        return levelsGained;
    }

    public bool TryPurchaseJobNode(string nodeId)
    {
        if (!IsAvailable || string.IsNullOrWhiteSpace(nodeId))
        {
            return false;
        }

        bool purchased = jobProgression.TryPurchase(JobNodeCatalog.Get(nodeId));
        if (purchased)
        {
            RecalculateEffectiveStats();
        }

        return purchased;
    }

    public bool HasLearnedJobNode(string nodeId)
    {
        return jobProgression.HasLearned(nodeId);
    }

    public bool CanUseLearnedJobFeature(string nodeId)
    {
        return IsAvailable &&
            jobProgression.CanUseLearnedJobFeature(nodeId, CurrentJob);
    }

    public PermanentStatBonuses GetPermanentStatBonuses()
    {
        return jobProgression.GetPermanentStatBonuses();
    }

    public void SetJobAffinity(JobId jobId, JobAffinity affinity)
    {
        if (!JobCatalog.Contains(jobId))
        {
            throw new ArgumentOutOfRangeException(nameof(jobId));
        }

        jobAffinities[jobId] = affinity;
        if (jobId == CurrentJob)
        {
            RecalculateEffectiveStats();
        }
    }

    public JobAffinity GetJobAffinity(JobId jobId)
    {
        return jobAffinities.TryGetValue(jobId, out JobAffinity affinity)
            ? affinity
            : JobAffinity.Neutral;
    }

    public float GetJobAffinityMultiplier(JobId jobId)
    {
        switch (GetJobAffinity(jobId))
        {
            case JobAffinity.Low:
                return 0.9f;
            case JobAffinity.High:
                return 1.1f;
            default:
                return 1f;
        }
    }

    public void RemovePermanently()
    {
        Equipment.DestroyAll();
        RecalculateEffectiveStats();
        IsAvailable = false;
    }

    private void RecalculateEffectiveStats()
    {
        CaptureExternalBaseStatChanges();

        int missingHp = Stats.MaxHp - Stats.CurrentHp;
        int missingMp = Stats.MaxMp - Stats.CurrentMp;
        bool wasIncapacitated = Stats.CurrentHp <= 0;
        PermanentStatBonuses permanent = GetPermanentStatBonuses();
        EquipmentStatBonuses equipment = Equipment.GetTotalStatBonuses();
        JobStatModifiers job = applyJobModifiers
            ? JobCatalog.Get(CurrentJob).StatModifiers
            : JobStatModifiers.None;
        JobAffinity affinity = GetJobAffinity(CurrentJob);

        Stats.MaxHp = baseStats.MaxHp + permanent.MaxHp + equipment.MaxHp +
            ScaleJobBonus(job.MaxHp, affinity);
        Stats.MaxMp = baseStats.MaxMp + permanent.MaxMp + equipment.MaxMp +
            ScaleJobBonus(job.MaxMp, affinity);
        Stats.Attack = baseStats.Attack + permanent.Attack + equipment.Attack +
            ScaleJobBonus(job.Attack, affinity);
        Stats.Defense = baseStats.Defense + permanent.Defense +
            equipment.Defense +
            ScaleJobBonus(job.Defense, affinity);
        Stats.Speed = baseStats.Speed + permanent.Speed + equipment.Speed +
            ScaleJobBonus(job.Speed, affinity);
        Stats.MagicPower = baseStats.MagicPower + permanent.MagicPower +
            equipment.MagicPower +
            ScaleJobBonus(job.MagicPower, affinity);
        Stats.MagicDefense = baseStats.MagicDefense + permanent.MagicDefense +
            equipment.MagicDefense +
            ScaleJobBonus(job.MagicDefense, affinity);
        Stats.Accuracy = baseStats.Accuracy + permanent.Accuracy +
            equipment.Accuracy +
            ScaleJobBonus(job.Accuracy, affinity);
        Stats.Evasion = baseStats.Evasion + permanent.Evasion +
            equipment.Evasion +
            ScaleJobBonus(job.Evasion, affinity);
        Stats.CriticalChance = baseStats.CriticalChance +
            permanent.CriticalChance + equipment.CriticalChance +
            ScaleJobBonus(job.CriticalChance, affinity);

        Stats.CurrentHp = wasIncapacitated
            ? 0
            : Math.Max(0, Stats.MaxHp - missingHp);
        Stats.CurrentMp = Math.Max(0, Stats.MaxMp - missingMp);
        lastEffectiveStats = StatValues.From(Stats);
    }

    private void CaptureExternalBaseStatChanges()
    {
        if (lastEffectiveStats == null)
        {
            return;
        }

        baseStats.MaxHp += Stats.MaxHp - lastEffectiveStats.MaxHp;
        baseStats.Attack += Stats.Attack - lastEffectiveStats.Attack;
        baseStats.Defense += Stats.Defense - lastEffectiveStats.Defense;
        baseStats.Speed += Stats.Speed - lastEffectiveStats.Speed;
        baseStats.MaxMp += Stats.MaxMp - lastEffectiveStats.MaxMp;
        baseStats.MagicPower +=
            Stats.MagicPower - lastEffectiveStats.MagicPower;
        baseStats.MagicDefense +=
            Stats.MagicDefense - lastEffectiveStats.MagicDefense;
        baseStats.Accuracy += Stats.Accuracy - lastEffectiveStats.Accuracy;
        baseStats.Evasion += Stats.Evasion - lastEffectiveStats.Evasion;
        baseStats.CriticalChance +=
            Stats.CriticalChance - lastEffectiveStats.CriticalChance;
    }

    private static int ScaleJobBonus(int bonus, JobAffinity affinity)
    {
        if (bonus <= 0 || affinity == JobAffinity.Neutral)
        {
            return bonus;
        }

        long tenths = affinity == JobAffinity.High
            ? (long)bonus * 11
            : (long)bonus * 9;
        return affinity == JobAffinity.High
            ? (int)((tenths + 9) / 10)
            : (int)(tenths / 10);
    }

    private sealed class StatValues
    {
        public int MaxHp;
        public int Attack;
        public int Defense;
        public int Speed;
        public int MaxMp;
        public int MagicPower;
        public int MagicDefense;
        public int Accuracy;
        public int Evasion;
        public int CriticalChance;

        public static StatValues From(CombatantStats stats)
        {
            return new StatValues
            {
                MaxHp = stats.MaxHp,
                Attack = stats.Attack,
                Defense = stats.Defense,
                Speed = stats.Speed,
                MaxMp = stats.MaxMp,
                MagicPower = stats.MagicPower,
                MagicDefense = stats.MagicDefense,
                Accuracy = stats.Accuracy,
                Evasion = stats.Evasion,
                CriticalChance = stats.CriticalChance
            };
        }
    }
}
