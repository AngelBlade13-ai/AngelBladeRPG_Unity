using System.Collections.Generic;

public class PartyRoster
{
    public const int MaximumActiveMembers = 4;

    private readonly Dictionary<string, PlayableCharacterData> characters =
        new Dictionary<string, PlayableCharacterData>();
    private readonly List<string> activeCharacterIds = new List<string>();

    public IReadOnlyCollection<PlayableCharacterData> Characters =>
        characters.Values;
    public IReadOnlyList<string> ActiveCharacterIds => activeCharacterIds;

    public bool TryAddCharacter(PlayableCharacterData character)
    {
        if (character == null || characters.ContainsKey(character.Id))
        {
            return false;
        }

        characters.Add(character.Id, character);
        return true;
    }

    public PlayableCharacterData GetCharacter(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
        {
            return null;
        }

        return characters.TryGetValue(
            characterId.Trim(),
            out PlayableCharacterData character)
            ? character
            : null;
    }

    public bool TrySetActiveParty(IEnumerable<string> characterIds)
    {
        if (characterIds == null)
        {
            return false;
        }

        List<string> requestedIds = new List<string>();
        HashSet<string> uniqueIds = new HashSet<string>();

        foreach (string characterId in characterIds)
        {
            PlayableCharacterData character = GetCharacter(characterId);
            if (character == null || !character.IsAvailable ||
                !uniqueIds.Add(character.Id))
            {
                return false;
            }

            requestedIds.Add(character.Id);
            if (requestedIds.Count > MaximumActiveMembers)
            {
                return false;
            }
        }

        activeCharacterIds.Clear();
        activeCharacterIds.AddRange(requestedIds);
        return true;
    }

    public bool TryRemovePermanently(string characterId)
    {
        PlayableCharacterData character = GetCharacter(characterId);
        if (character == null || !character.IsAvailable)
        {
            return false;
        }

        character.RemovePermanently();
        activeCharacterIds.Remove(character.Id);
        return true;
    }

    public void RecordBattleParticipation()
    {
        HashSet<string> activeIds =
            new HashSet<string>(activeCharacterIds);

        foreach (PlayableCharacterData character in characters.Values)
        {
            if (character.IsAvailable)
            {
                character.RosterHistory.RecordBattleParticipation(
                    activeIds.Contains(character.Id));
            }
        }
    }

    public int GrantJobPointsToAvailableCharacters(int amount)
    {
        if (amount <= 0)
        {
            return 0;
        }

        int recipients = 0;
        foreach (PlayableCharacterData character in characters.Values)
        {
            if (character.TryAddJobPoints(amount))
            {
                recipients += 1;
            }
        }

        return recipients;
    }

    public bool TryAddBondPoints(
        string firstCharacterId,
        string secondCharacterId,
        int points)
    {
        PlayableCharacterData first = GetCharacter(firstCharacterId);
        PlayableCharacterData second = GetCharacter(secondCharacterId);

        if (first == null || second == null ||
            !first.IsAvailable || !second.IsAvailable ||
            first == second || points <= 0)
        {
            return false;
        }

        first.RosterHistory.TryAddBondPoints(second.Id, points);
        second.RosterHistory.TryAddBondPoints(first.Id, points);
        return true;
    }
}
