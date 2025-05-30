using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class GameSaveProvider : IGameSaveProvider
{
    private const string SaveKey = "save.json"; // Now used as the filename
    public string SavePath => Path.Combine(Application.persistentDataPath, SaveKey);

    public void SaveGame(GameSaveData data)
    {
        // Handle EARTH_EARLY and EARTH_LATE saving logic
        GameProgressionStage stageToSave = data.progressionStage;
        if (data.progressionStage == GameProgressionStage.EARTH_EARLY ||
            data.progressionStage == GameProgressionStage.EARTH_EARLY_BOSS)
        {
            stageToSave = GameProgressionStage.CASTLE_EARLY;
        }
        else if (data.progressionStage == GameProgressionStage.EARTH_LATE ||
                 data.progressionStage == GameProgressionStage.EARTH_LATE_BOSS)
        {
            stageToSave = GameProgressionStage.CASTLE_LATE;
        }

        // Create a new GameSaveData object with the adjusted progression stage
        GameSaveData saveData = new GameSaveData
        {
            progressionStage = stageToSave,
            maxHealth = data.maxHealth,
            maxMana = data.maxMana,
            saveDate = DateTime.Now,
            keyBindings = data.keyBindings
        };
        Debug.Log($"[GameSaveProvider] Preparing to save {saveData.keyBindings?.Count ?? 0} keybindings.");

        foreach(var entry in saveData.keyBindings ?? new List<GameSaveData.KeyBindingEntry>())
        {
            Debug.Log($"  - SAVING TO JSON: Action: {entry.action}, keyCode: {entry.keyCode}");
        }

        string jsonData = JsonUtility.ToJson(saveData);
        Debug.Log("[GameSaveProvider] JSON to be saved: " + jsonData);

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
                Debug.Log("[GameSaveProvider] RAW JSON LOADED: " + jsonData);
                GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(jsonData);
                if (loadedData == null) Debug.LogError("[GameSaveProvider] loadedData is NULL after FromJson.");
                else if (loadedData.keyBindings == null) Debug.LogError("[GameSaveProvider] loadedData.keyBindings is NULL after FromJson.");
                else {
                    Debug.Log($"[GameSaveProvider] Parsed {loadedData.keyBindings.Count} keybindings from JSON.");
                    foreach(var entry in loadedData.keyBindings)
                    {
                        Debug.Log($"  - PARSED FROM JSON: Action: {entry.action}, keyCode: {entry.keyCode}");
                    }
                }
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

    public bool SaveExists()
    {
        return File.Exists(SavePath);
    }
}