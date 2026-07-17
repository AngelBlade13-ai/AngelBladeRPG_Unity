using System;
using System.Collections.Generic;

public enum PartyManagementResult
{
    Success,
    CharacterNotFound,
    CharacterUnavailable,
    AlreadyActive,
    AlreadyInReserve,
    ActivePartyFull,
    LastActiveMember,
    InvalidMove,
    InvalidJob
}

public sealed class PartyManagementService
{
    private readonly PartyRoster roster;

    public PartyManagementService(PartyRoster roster)
    {
        this.roster = roster ??
            throw new ArgumentNullException(nameof(roster));
    }

    public IReadOnlyList<PlayableCharacterData> GetOrderedCharacters()
    {
        List<PlayableCharacterData> ordered =
            new List<PlayableCharacterData>();
        HashSet<string> activeIds = new HashSet<string>();

        foreach (string characterId in roster.ActiveCharacterIds)
        {
            PlayableCharacterData character = roster.GetCharacter(characterId);
            if (character != null && character.IsAvailable)
            {
                ordered.Add(character);
                activeIds.Add(character.Id);
            }
        }

        List<PlayableCharacterData> reserve =
            new List<PlayableCharacterData>();
        foreach (PlayableCharacterData character in roster.Characters)
        {
            if (character.IsAvailable && !activeIds.Contains(character.Id))
            {
                reserve.Add(character);
            }
        }

        reserve.Sort((left, right) => string.Compare(
            left.Id,
            right.Id,
            StringComparison.Ordinal));
        ordered.AddRange(reserve);
        return ordered.AsReadOnly();
    }

    public bool IsActive(string characterId)
    {
        return IndexOfActiveCharacter(characterId) >= 0;
    }

    public PartyManagementResult AddToActiveParty(string characterId)
    {
        PartyManagementResult validation = ValidateCharacter(
            characterId,
            out PlayableCharacterData character);
        if (validation != PartyManagementResult.Success)
        {
            return validation;
        }

        if (IsActive(character.Id))
        {
            return PartyManagementResult.AlreadyActive;
        }

        if (roster.ActiveCharacterIds.Count >= PartyRoster.MaximumActiveMembers)
        {
            return PartyManagementResult.ActivePartyFull;
        }

        List<string> formation = new List<string>(roster.ActiveCharacterIds)
        {
            character.Id
        };
        return roster.TrySetActiveParty(formation)
            ? PartyManagementResult.Success
            : PartyManagementResult.CharacterUnavailable;
    }

    public PartyManagementResult MoveToReserve(string characterId)
    {
        PartyManagementResult validation = ValidateCharacter(
            characterId,
            out PlayableCharacterData character);
        if (validation != PartyManagementResult.Success)
        {
            return validation;
        }

        int activeIndex = IndexOfActiveCharacter(character.Id);
        if (activeIndex < 0)
        {
            return PartyManagementResult.AlreadyInReserve;
        }

        if (roster.ActiveCharacterIds.Count <= 1)
        {
            return PartyManagementResult.LastActiveMember;
        }

        List<string> formation = new List<string>(roster.ActiveCharacterIds);
        formation.RemoveAt(activeIndex);
        return roster.TrySetActiveParty(formation)
            ? PartyManagementResult.Success
            : PartyManagementResult.CharacterUnavailable;
    }

    public PartyManagementResult MoveActiveCharacter(
        string characterId,
        int direction)
    {
        PartyManagementResult validation = ValidateCharacter(
            characterId,
            out PlayableCharacterData character);
        if (validation != PartyManagementResult.Success)
        {
            return validation;
        }

        if (direction != -1 && direction != 1)
        {
            return PartyManagementResult.InvalidMove;
        }

        int activeIndex = IndexOfActiveCharacter(character.Id);
        int destination = activeIndex + direction;
        if (activeIndex < 0 || destination < 0 ||
            destination >= roster.ActiveCharacterIds.Count)
        {
            return PartyManagementResult.InvalidMove;
        }

        List<string> formation = new List<string>(roster.ActiveCharacterIds);
        string movedId = formation[activeIndex];
        formation[activeIndex] = formation[destination];
        formation[destination] = movedId;
        return roster.TrySetActiveParty(formation)
            ? PartyManagementResult.Success
            : PartyManagementResult.InvalidMove;
    }

    public PartyManagementResult AssignJob(
        string characterId,
        JobId jobId)
    {
        PartyManagementResult validation = ValidateCharacter(
            characterId,
            out PlayableCharacterData character);
        if (validation != PartyManagementResult.Success)
        {
            return validation;
        }

        if (!JobCatalog.Contains(jobId))
        {
            return PartyManagementResult.InvalidJob;
        }

        return character.TryAssignJob(jobId)
            ? PartyManagementResult.Success
            : PartyManagementResult.CharacterUnavailable;
    }

    private PartyManagementResult ValidateCharacter(
        string characterId,
        out PlayableCharacterData character)
    {
        character = roster.GetCharacter(characterId);
        if (character == null)
        {
            return PartyManagementResult.CharacterNotFound;
        }

        return character.IsAvailable
            ? PartyManagementResult.Success
            : PartyManagementResult.CharacterUnavailable;
    }

    private int IndexOfActiveCharacter(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
        {
            return -1;
        }

        for (int index = 0; index < roster.ActiveCharacterIds.Count; index++)
        {
            if (string.Equals(
                roster.ActiveCharacterIds[index],
                characterId.Trim(),
                StringComparison.Ordinal))
            {
                return index;
            }
        }

        return -1;
    }
}
