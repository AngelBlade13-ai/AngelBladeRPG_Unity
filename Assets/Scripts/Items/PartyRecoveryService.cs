public static class PartyRecoveryService
{
    public static bool NeedsFullRecovery(PartyRoster roster)
    {
        if (roster == null)
        {
            return false;
        }

        foreach (PlayableCharacterData character in roster.Characters)
        {
            if (character.IsAvailable &&
                (character.Stats.CurrentHp < character.Stats.MaxHp ||
                    character.Stats.CurrentMp < character.Stats.MaxMp))
            {
                return true;
            }
        }

        return false;
    }

    public static int FullRestore(PartyRoster roster)
    {
        if (roster == null)
        {
            return 0;
        }

        int restoredCharacters = 0;
        foreach (PlayableCharacterData character in roster.Characters)
        {
            if (!character.IsAvailable)
            {
                continue;
            }

            character.Stats.CurrentHp = character.Stats.MaxHp;
            character.Stats.CurrentMp = character.Stats.MaxMp;
            restoredCharacters += 1;
        }

        return restoredCharacters;
    }
}
