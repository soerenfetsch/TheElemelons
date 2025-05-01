using NUnit.Framework;
using System.IO;
using UnityEngine;

public class GameSaveProviderTests
{
    private GameSaveProvider saveProvider;
    private string savePath;

    [SetUp]
    public void Setup()
    {
        // Initialize the save provider and set a test save path
        saveProvider = new GameSaveProvider();
        savePath = Path.Combine(Application.persistentDataPath, "test_save.json"); // Use a specific test file

        // Ensure no previous save exists
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the test save file after each test
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
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
        Assert.IsTrue(File.Exists(savePath));

        // Optionally, verify the contents of the file
        string jsonData = File.ReadAllText(savePath);
        GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(jsonData);
        Assert.AreEqual(GameProgressionStage.CASTLE_EARLY, loadedData.progressionStage);
        Assert.AreEqual(100, loadedData.maxHealth);
        Assert.AreEqual(50, loadedData.maxMana);
        Assert.AreEqual("test_save.json", loadedData.saveFileName);
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
        Assert.AreEqual("test_save.json", loadedData.saveFileName);
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
        Assert.IsTrue(File.Exists(savePath));

        // Act
        saveProvider.ResetSave();

        // Assert
        Assert.IsFalse(File.Exists(savePath));
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
        // Since EARTH_EARLY should not be saved, verify that the file is not created or is empty.
        Assert.IsFalse(File.Exists(savePath) || new FileInfo(savePath).Length == 0);
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
        // Since EARTH_LATE should not be saved, verify that the file is not created or is empty.
        Assert.IsFalse(File.Exists(savePath) || new FileInfo(savePath).Length == 0);
    }

    [Test]
    public void LoadGame_ReturnsNullIfNoSaveFileExists()
    {
        // Arrange
        // Ensure no save file exists
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
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
            saveFileName = "full_test_save.json"
        };

        // Act
        saveProvider.SaveGame(data);
        GameSaveData loadedData = saveProvider.LoadGame();

        // Assert
        Assert.IsNotNull(loadedData);
        Assert.AreEqual(data.progressionStage, loadedData.progressionStage);
        Assert.AreEqual(data.maxHealth, loadedData.maxHealth);
        Assert.AreEqual(data.maxMana, loadedData.maxMana);
        Assert.AreEqual(data.saveFileName, loadedData.saveFileName);
    }
}