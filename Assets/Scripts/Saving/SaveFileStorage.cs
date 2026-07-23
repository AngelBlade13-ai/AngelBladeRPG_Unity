using System;
using System.IO;
using System.Text;
using UnityEngine;

public enum SaveFileWriteStatus
{
    Success,
    InvalidSlot,
    InvalidData,
    UnsupportedVersion,
    IoError
}

public enum SaveFileReadStatus
{
    Success,
    RecoveredBackup,
    Missing,
    InvalidSlot,
    Corrupt,
    UnsupportedVersion,
    IoError
}

public sealed class SaveFileStorage
{
    private const string SaveExtension = ".json";
    private const string TemporaryExtension = ".tmp";
    private const string BackupExtension = ".bak";
    private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

    public string RootDirectory { get; }

    public SaveFileStorage(string rootDirectory)
    {
        if (string.IsNullOrWhiteSpace(rootDirectory))
        {
            throw new ArgumentException(
                "A save directory is required.",
                nameof(rootDirectory));
        }

        RootDirectory = Path.GetFullPath(rootDirectory);
    }

    public static SaveFileStorage CreateDefault()
    {
        return new SaveFileStorage(Path.Combine(
            Application.persistentDataPath,
            "Saves"));
    }

    public SaveFileWriteStatus TryWrite(
        string slotId,
        GameSaveData data)
    {
        if (!TryGetPaths(
                slotId,
                out string savePath,
                out string temporaryPath,
                out string backupPath))
        {
            return SaveFileWriteStatus.InvalidSlot;
        }

        string json;
        try
        {
            json = SaveDataJson.Serialize(data, prettyPrint: true);
        }
        catch (NotSupportedException)
        {
            return SaveFileWriteStatus.UnsupportedVersion;
        }
        catch (ArgumentException)
        {
            return SaveFileWriteStatus.InvalidData;
        }

        try
        {
            Directory.CreateDirectory(RootDirectory);
            WriteDurably(temporaryPath, json);

            if (File.Exists(savePath))
            {
                File.Replace(
                    temporaryPath,
                    savePath,
                    backupPath,
                    ignoreMetadataErrors: true);
            }
            else
            {
                File.Move(temporaryPath, savePath);
            }

            return SaveFileWriteStatus.Success;
        }
        catch (IOException)
        {
            return SaveFileWriteStatus.IoError;
        }
        catch (UnauthorizedAccessException)
        {
            return SaveFileWriteStatus.IoError;
        }
        finally
        {
            TryDeleteTemporaryFile(temporaryPath);
        }
    }

    public SaveFileReadStatus TryRead(
        string slotId,
        out GameSaveData data)
    {
        data = null;
        if (!TryGetPaths(
                slotId,
                out string savePath,
                out _,
                out string backupPath))
        {
            return SaveFileReadStatus.InvalidSlot;
        }

        FileReadAttempt primary = ReadFile(savePath);
        if (primary.Status == FileReadAttemptStatus.Success)
        {
            data = primary.Data;
            return SaveFileReadStatus.Success;
        }

        if (primary.Status == FileReadAttemptStatus.UnsupportedVersion)
        {
            return SaveFileReadStatus.UnsupportedVersion;
        }

        if (primary.Status == FileReadAttemptStatus.IoError)
        {
            return SaveFileReadStatus.IoError;
        }

        FileReadAttempt backup = ReadFile(backupPath);
        if (backup.Status == FileReadAttemptStatus.Success)
        {
            data = backup.Data;
            return SaveFileReadStatus.RecoveredBackup;
        }

        if (backup.Status == FileReadAttemptStatus.UnsupportedVersion)
        {
            return SaveFileReadStatus.UnsupportedVersion;
        }

        if (backup.Status == FileReadAttemptStatus.IoError)
        {
            return SaveFileReadStatus.IoError;
        }

        return primary.Status == FileReadAttemptStatus.Missing &&
            backup.Status == FileReadAttemptStatus.Missing
            ? SaveFileReadStatus.Missing
            : SaveFileReadStatus.Corrupt;
    }

    public bool TryGetSavePath(string slotId, out string savePath)
    {
        return TryGetPaths(slotId, out savePath, out _, out _);
    }

    private bool TryGetPaths(
        string slotId,
        out string savePath,
        out string temporaryPath,
        out string backupPath)
    {
        savePath = null;
        temporaryPath = null;
        backupPath = null;
        if (!IsValidSlotId(slotId))
        {
            return false;
        }

        string normalizedId = slotId.Trim();
        savePath = Path.Combine(
            RootDirectory,
            normalizedId + SaveExtension);
        temporaryPath = savePath + TemporaryExtension;
        backupPath = savePath + BackupExtension;
        return true;
    }

    private static bool IsValidSlotId(string slotId)
    {
        if (string.IsNullOrWhiteSpace(slotId))
        {
            return false;
        }

        string value = slotId.Trim();
        if (value.Length > 64)
        {
            return false;
        }

        foreach (char character in value)
        {
            if (!char.IsLetterOrDigit(character) &&
                character != '_' &&
                character != '-')
            {
                return false;
            }
        }

        return true;
    }

    private static void WriteDurably(string path, string contents)
    {
        using (var stream = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None))
        using (var writer = new StreamWriter(stream, Utf8WithoutBom))
        {
            writer.Write(contents);
            writer.Flush();
            stream.Flush(flushToDisk: true);
        }
    }

    private static FileReadAttempt ReadFile(string path)
    {
        if (!File.Exists(path))
        {
            return FileReadAttempt.Missing();
        }

        try
        {
            string json = File.ReadAllText(path, Utf8WithoutBom);
            SaveDataReadStatus status =
                SaveDataJson.TryDeserialize(json, out GameSaveData data);
            switch (status)
            {
                case SaveDataReadStatus.Success:
                    return FileReadAttempt.Success(data);
                case SaveDataReadStatus.UnsupportedVersion:
                    return FileReadAttempt.UnsupportedVersion();
                default:
                    return FileReadAttempt.Corrupt();
            }
        }
        catch (IOException)
        {
            return FileReadAttempt.IoError();
        }
        catch (UnauthorizedAccessException)
        {
            return FileReadAttempt.IoError();
        }
    }

    private static void TryDeleteTemporaryFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private enum FileReadAttemptStatus
    {
        Success,
        Missing,
        Corrupt,
        UnsupportedVersion,
        IoError
    }

    private sealed class FileReadAttempt
    {
        public FileReadAttemptStatus Status { get; }
        public GameSaveData Data { get; }

        private FileReadAttempt(
            FileReadAttemptStatus status,
            GameSaveData data = null)
        {
            Status = status;
            Data = data;
        }

        public static FileReadAttempt Success(GameSaveData data)
        {
            return new FileReadAttempt(
                FileReadAttemptStatus.Success,
                data);
        }

        public static FileReadAttempt Missing()
        {
            return new FileReadAttempt(FileReadAttemptStatus.Missing);
        }

        public static FileReadAttempt Corrupt()
        {
            return new FileReadAttempt(FileReadAttemptStatus.Corrupt);
        }

        public static FileReadAttempt UnsupportedVersion()
        {
            return new FileReadAttempt(
                FileReadAttemptStatus.UnsupportedVersion);
        }

        public static FileReadAttempt IoError()
        {
            return new FileReadAttempt(FileReadAttemptStatus.IoError);
        }
    }
}
