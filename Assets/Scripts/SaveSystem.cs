using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string saveFileName = "ChessTwo.sav";
    private static string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    public static void Save(SaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
            Debug.Log("Game saved successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public static SaveData Load()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                string json = File.ReadAllText(SavePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("Game loaded successfully.");
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load save data: {e.Message}");
            }
        }
        else
        {
            Debug.Log("No save file found.");
        }
        return null;
    }

    public static bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    public static string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "ChessTwo.sav");
    }
}
