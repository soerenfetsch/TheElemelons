using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Provides functionality for saving and loading game progress using JSON.
/// This class follows a Singleton pattern and should be attached to a dedicated GameObject.
/// </summary>
public class GameSaveProvider : MonoBehaviour
{
    #region Singleton

    /// <summary>
    /// Singleton instance of the GameSaveProvider.
    /// </summary>
    public static GameSaveProvider Instance { get; private set; }

    #endregion

    #region Public Fields

    /// <summary>
    /// Reference to the GameProgression asset. This must be assigned in the Inspector.
    /// </summary>
    public GameProgression gameProgress;

    #endregion

    #region Private Fields

    /// <summary>
    /// The file path for saving the game progress.
    /// </summary>
    private string saveFilePath;

    #endregion

    #region Unity Callbacks

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Initializes the Singleton instance and sets up the save file path.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            saveFilePath = Path.Combine(Application.persistentDataPath, "gameSave.json");
            // Optionally, uncomment the next line if you want this object to persist across scenes:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Private Nested Types

    /// <summary>
    /// Data structure used for serializing and deserializing game progress to/from JSON.
    /// </summary>
    [System.Serializable]
    private class GameProgressData
    {
        public int currentDungeon;
        public string[] collectedElemelons;
        public string[] completedDialogues;
        public int playerHealth;
        public int playerMana;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Saves the current game progress to a JSON file.
    /// </summary>
    public void SaveGame()
    {
        GameProgressData data = new GameProgressData
        {
            currentDungeon = gameProgress.currentDungeon,
            collectedElemelons = gameProgress.collectedElemelons.ToArray(),
            completedDialogues = gameProgress.completedDialogues.ToArray(),
            playerHealth = gameProgress.playerHealth,
            playerMana = gameProgress.playerMana
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved to: " + saveFilePath);
    }

    /// <summary>
    /// Loads game progress from a JSON file, if it exists.
    /// Updates the GameProgression asset with the loaded data.
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameProgressData data = JsonUtility.FromJson<GameProgressData>(json);
            gameProgress.currentDungeon = data.currentDungeon;
            gameProgress.collectedElemelons = new List<string>(data.collectedElemelons);
            gameProgress.completedDialogues = new List<string>(data.completedDialogues);
            gameProgress.playerHealth = data.playerHealth;
            gameProgress.playerMana = data.playerMana;

            Debug.Log("Game Loaded from: " + saveFilePath);
        }
        else
        {
            Debug.LogWarning("No save file found at: " + saveFilePath);
        }
    }

    /// <summary>
    /// Resets the saved game by deleting the save file and clearing the GameProgression data.
    /// </summary>
    public void ResetSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
        gameProgress.ResetProgress();
        Debug.Log("Save Reset and GameProgression cleared.");
    }

    #endregion
}