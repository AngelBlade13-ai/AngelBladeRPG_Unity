using System;
using UnityEngine;

public enum SaveDataReadStatus
{
    Success,
    Empty,
    Malformed,
    UnsupportedVersion,
    MissingRequiredData
}

public static class SaveDataJson
{
    public static string Serialize(GameSaveData data, bool prettyPrint = false)
    {
        if (!HasRequiredData(data))
        {
            throw new ArgumentException(
                "Save data is missing required sections.",
                nameof(data));
        }

        if (data.schemaVersion != SaveSchema.CurrentVersion)
        {
            throw new NotSupportedException(
                $"Save schema {data.schemaVersion} is not supported.");
        }

        return JsonUtility.ToJson(data, prettyPrint);
    }

    public static SaveDataReadStatus TryDeserialize(
        string json,
        out GameSaveData data)
    {
        data = null;
        if (string.IsNullOrWhiteSpace(json))
        {
            return SaveDataReadStatus.Empty;
        }

        try
        {
            data = JsonUtility.FromJson<GameSaveData>(json);
        }
        catch (ArgumentException)
        {
            return SaveDataReadStatus.Malformed;
        }

        if (data == null)
        {
            return SaveDataReadStatus.Malformed;
        }

        if (data.schemaVersion != SaveSchema.CurrentVersion)
        {
            data = null;
            return SaveDataReadStatus.UnsupportedVersion;
        }

        if (!HasRequiredData(data))
        {
            data = null;
            return SaveDataReadStatus.MissingRequiredData;
        }

        return SaveDataReadStatus.Success;
    }

    private static bool HasRequiredData(GameSaveData data)
    {
        return data != null &&
            data.player != null &&
            data.party != null &&
            data.inventory != null &&
            data.quests != null &&
            data.world != null &&
            data.camp != null &&
            data.location != null;
    }
}
