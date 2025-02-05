using System.IO;
using UnityEngine;

/// <summary>
/// Manages saving, loading, and resetting game save data.
/// Utilizes Unity's persistent data path for file storage.
/// </summary>
public class GameSaveProvider : MonoBehaviour
{
    /// <summary>
    /// The name of the save file.
    /// </summary>
    private const string SaveFileName = "gameSave.json";

    /// <summary>
    /// Saves the current game progress to a file in JSON format.
    /// </summary>
    /// <param name="saveData">The game save data to be saved.</param>
    public void SaveGame(GameSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true); // Convert data to JSON format
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName); // Save file location

        try
        {
            File.WriteAllText(filePath, json); // Write JSON to file
            Debug.Log($"Game saved successfully to {filePath}");
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    /// <summary>
    /// Loads the game progress from a JSON save file.
    /// </summary>
    /// <returns>The loaded game save data, or null if no save file exists.</returns>
    public GameSaveData LoadGame()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No save file found. Returning null.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(filePath); // Read JSON from file
            return JsonUtility.FromJson<GameSaveData>(json); // Deserialize JSON to GameSaveData
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Deletes the current save file, resetting the game's progress.
    /// </summary>
    public void ResetSave()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath); // Delete the save file
                Debug.Log("Save file deleted successfully.");
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to reset save: {e.Message}");
        }
    }
}
