using System;

public class BattleTurnParticipant
{
    public string CombatantId { get; }
    public int Speed { get; }

    public BattleTurnParticipant(string combatantId, int speed)
    {
        if (string.IsNullOrWhiteSpace(combatantId))
        {
            throw new ArgumentException(
                "A combatant ID is required.",
                nameof(combatantId));
        }

        if (speed < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(speed));
        }

        CombatantId = combatantId.Trim();
        Speed = speed;
    }
}
