using System.IO;
using UnityEngine;

/// <summary>
/// Manages saving, loading, and resetting game save data.
/// </summary>
public class SaveManager : MonoBehaviour
{
    private const string SaveFileName = "gameSave.json";
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    /// <summary>
    /// Saves the current game progress.
    /// </summary>
    public void SaveGame(SaveGame saveData)
    {
        if (saveData == null)
        {
            Debug.LogError("Save failed: SaveGame is null.");
            return;
        }

        saveData.version = Application.version; // Store game version

        string json = JsonUtility.ToJson(saveData, true);
        try
        {
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"Game saved successfully to {SaveFilePath}");
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    /// <summary>
    /// Loads game progress.
    /// </summary>
    public SaveGame LoadGame()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning("No save file found.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(SaveFilePath);
            SaveGame saveData = JsonUtility.FromJson<SaveGame>(json);

            // Handle version mismatches
            if (saveData.version != Application.version)
            {
                Debug.LogWarning($"Save file version mismatch! Expected {Application.version}, but found {saveData.version}.");
                return null; // Discard old save
            }

            return saveData;
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Deletes the save file.
    /// </summary>
    public void ResetSave()
    {
        try
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
                Debug.Log("Save file deleted.");
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to reset save: {e.Message}");
        }
    }
}
