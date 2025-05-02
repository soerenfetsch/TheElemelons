using NUnit.Framework;
using System.IO;
using UnityEngine;

public class GameSaveProviderTests
{
    private GameSaveProvider saveProvider;

    [SetUp]
    public void Setup()
    {
        // Initialize the save provider
        saveProvider = new GameSaveProvider();

        // Ensure no previous save exists
        if (File.Exists(saveProvider.SavePath)) // Use saveProvider.SavePath
        {
            File.Delete(saveProvider.SavePath); // Use saveProvider.SavePath
        }
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the test save file after each test
        if (File.Exists(saveProvider.SavePath)) // Use saveProvider.SavePath
        {
            File.Delete(saveProvider.SavePath); // Use saveProvider.SavePath
        }
    }

    [Test]
    public void SaveGame_SavesDataToFile()
    {
        // Arrange
        GameSaveData data = new GameSaveData
        {
            progressionStage = GameProgressionStage.CASTLE_EARLY,
            maxHealth = 100,
            maxMana = 50,
            saveFileName = "test_save.json"
        };

        // Act
        saveProvider.SaveGame(data);

        // Assert
        Assert.IsTrue(File.Exists(saveProvider.SavePath)); // Use saveProvider.SavePath

        // Optionally, verify the contents of the file
        string jsonData = File.ReadAllText(saveProvider.SavePath); // Use saveProvider.SavePath
        GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(jsonData);
        Assert.AreEqual(GameProgressionStage.CASTLE_EARLY, loadedData.progressionStage);
        Assert.AreEqual(100, loadedData.maxHealth);
        Assert.AreEqual(50, loadedData.maxMana);
        Assert.AreEqual("save.json", loadedData.saveFileName); // Use "save.json"
    }

    [Test]
    public void LoadGame_LoadsDataFromFile()
    {
        // Arrange
        // First, save a game
        GameSaveData data = new GameSaveData
        {
            progressionStage = GameProgressionStage.CASTLE_LATE,
            maxHealth = 120,
            maxMana = 60,
            saveFileName = "test_save.json"
        };
        saveProvider.SaveGame(data);

        // Act
        GameSaveData loadedData = saveProvider.LoadGame();

        // Assert
        Assert.IsNotNull(loadedData);
        Assert.AreEqual(GameProgressionStage.CASTLE_LATE, loadedData.progressionStage);
        Assert.AreEqual(120, loadedData.maxHealth);
        Assert.AreEqual(60, loadedData.maxMana);
        Assert.AreEqual("save.json", loadedData.saveFileName); // Use "save.json"
    }

    [Test]
    public void ResetSave_DeletesSaveFile()
    {
        // Arrange
        // First, save a game
        GameSaveData data = new GameSaveData
        {
            progressionStage = GameProgressionStage.CASTLE_EARLY,
            maxHealth = 100,
            maxMana = 50,
            saveFileName = "test_save.json"
        };
        saveProvider.SaveGame(data);
        Assert.IsTrue(File.Exists(saveProvider.SavePath)); // Use saveProvider.SavePath

        // Act
        saveProvider.ResetSave();

        // Assert
        Assert.IsFalse(File.Exists(saveProvider.SavePath)); // Use saveProvider.SavePath
    }

    [Test]
    public void SaveGame_HandlesEarthEarlyCorrectly()
    {
        // Arrange
        GameSaveData data = new GameSaveData
        {
            progressionStage = GameProgressionStage.EARTH_EARLY,
            maxHealth = 100,
            maxMana = 50,
            saveFileName = "test_save.json"
        };

        // Act
        saveProvider.SaveGame(data);

        // Assert
        // Since EARTH_EARLY should be saved as CASTLE_EARLY, verify that the file is created and contains CASTLE_EARLY.
        Assert.IsTrue(File.Exists(saveProvider.SavePath));

        string jsonData = File.ReadAllText(saveProvider.SavePath);
        GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(jsonData);
        Assert.AreEqual(GameProgressionStage.CASTLE_EARLY, loadedData.progressionStage);
    }

    [Test]
    public void SaveGame_HandlesEarthLateCorrectly()
    {
        // Arrange
        GameSaveData data = new GameSaveData
        {
            progressionStage = GameProgressionStage.EARTH_LATE,
            maxHealth = 100,
            maxMana = 50,
            saveFileName = "test_save.json"
        };

        // Act
        saveProvider.SaveGame(data);

        // Assert
        // Since EARTH_LATE should be saved as CASTLE_LATE, verify that the file is created and contains CASTLE_LATE.
        Assert.IsTrue(File.Exists(saveProvider.SavePath));

        string jsonData = File.ReadAllText(saveProvider.SavePath);
        GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(jsonData);
        Assert.AreEqual(GameProgressionStage.CASTLE_LATE, loadedData.progressionStage);
    }

    [Test]
    public void LoadGame_ReturnsNullIfNoSaveFileExists()
    {
        // Arrange
        // Ensure no save file exists
        if (File.Exists(saveProvider.SavePath)) // Use saveProvider.SavePath
        {
            File.Delete(saveProvider.SavePath); // Use saveProvider.SavePath
        }

        // Act
        GameSaveData loadedData = saveProvider.LoadGame();

        // Assert
        Assert.IsNull(loadedData);
    }

    [Test]
    public void SaveAndLoadGame_HandlesAllPropertiesCorrectly()
    {
        // Arrange
        GameSaveData data = new GameSaveData
        {
            progressionStage = GameProgressionStage.CASTLE_END,
            maxHealth = 150,
            maxMana = 75,
            saveFileName = "test_save.json"
        };

        // Act
        saveProvider.SaveGame(data);
        GameSaveData loadedData = saveProvider.LoadGame();

        // Assert
        Assert.IsNotNull(loadedData);
        Assert.AreEqual(data.progressionStage, loadedData.progressionStage);
        Assert.AreEqual(data.maxHealth, loadedData.maxHealth);
        Assert.AreEqual(data.maxMana, loadedData.maxMana);
        Assert.AreEqual("save.json", loadedData.saveFileName); // Use "save.json"
    }
}