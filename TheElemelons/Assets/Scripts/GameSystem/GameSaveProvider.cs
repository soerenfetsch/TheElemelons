using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Provides functionality to save, load, reset, and check existence of game save data.
/// Handles special logic for mapping certain progression stages to their castle equivalents.
/// </summary>
public class GameSaveProvider : IGameSaveProvider
{
    /// <summary>
    /// The filename for the save file.
    /// </summary>
    private const string SaveKey = "save.json"; // Now used as the filename

    /// <summary>
    /// The full path to the save file in the persistent data directory.
    /// </summary>
    public string SavePath => Path.Combine(Application.persistentDataPath, SaveKey);

    /// <summary>
    /// Saves the provided <see cref="GameSaveData"/> to disk.
    /// Handles mapping EARTH_EARLY/EARTH_EARLY_BOSS to CASTLE_EARLY and
    /// EARTH_LATE/EARTH_LATE_BOSS to CASTLE_LATE for save compatibility.
    /// </summary>
    /// <param name="data">The game save data to persist.</param>
    public void SaveGame(GameSaveData data)
    {
        if (data.progressionStage == GameProgressionStage.INVALID)
        {
            Debug.LogError("[GameSaveProvider] Cannot save game with INVALID progression stage.");
            return;
        }
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

    /// <summary>
    /// Loads the <see cref="GameSaveData"/> from disk, if it exists.
    /// </summary>
    /// <returns>The loaded game save data, or null if not found or on error.</returns>
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

    /// <summary>
    /// Deletes the save file from disk, if it exists.
    /// </summary>
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

    /// <summary>
    /// Checks if a save file currently exists on disk.
    /// </summary>
    /// <returns>True if a save file exists; otherwise, false.</returns>
    public bool SaveExists()
    {
        return File.Exists(SavePath);
    }
}