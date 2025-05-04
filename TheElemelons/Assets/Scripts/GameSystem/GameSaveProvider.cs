using UnityEngine;
using System.IO;
using System;

public class GameSaveProvider : IGameSaveProvider
{
    private const string SaveKey = "save.json"; // Now used as the filename
    public string SavePath => Path.Combine(Application.persistentDataPath, SaveKey);

    public void SaveGame(GameSaveData data)
    {
        // Handle EARTH_EARLY and EARTH_LATE saving logic
        GameProgressionStage stageToSave = data.progressionStage;
        if (data.progressionStage == GameProgressionStage.EARTH_EARLY)
        {
            stageToSave = GameProgressionStage.CASTLE_EARLY;
        }
        else if (data.progressionStage == GameProgressionStage.EARTH_LATE)
        {
            stageToSave = GameProgressionStage.CASTLE_LATE;
        }

        // Create a new GameSaveData object with the adjusted progression stage
        GameSaveData saveData = new GameSaveData
        {
            progressionStage = stageToSave,
            maxHealth = data.maxHealth,
            maxMana = data.maxMana,
            saveDate = DateTime.Now
        };

        string jsonData = JsonUtility.ToJson(saveData);

        try
        {
            File.WriteAllText(SavePath, jsonData);
            Debug.Log("Saved to " + SavePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save game: " + e.Message);
        }
    }

    public GameSaveData LoadGame()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                string jsonData = File.ReadAllText(SavePath);
                GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(jsonData);
                Debug.Log("Loaded from " + SavePath);
                return loadedData;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load game: " + e.Message);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("No save file found at " + SavePath);
            return null;
        }
    }

    public void ResetSave()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                File.Delete(SavePath);
                Debug.Log("Save deleted from " + SavePath);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to delete save: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("No save file found at " + SavePath + " to delete.");
        }
    }
}