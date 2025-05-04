using NUnit.Framework;
using static Room;

public class GameStateTest
{
    [Test]
    public void GameProgressionStage_Enum_ContainsAllExpectedValues()
    {
        Assert.AreEqual(0, (int)GameProgressionStage.CASTLE_INTRODUCTION);
        Assert.AreEqual(1, (int)GameProgressionStage.CASTLE_EARLY);
        Assert.AreEqual(2, (int)GameProgressionStage.EARTH_EARLY);
        Assert.AreEqual(3, (int)GameProgressionStage.CASTLE_END);
        Assert.AreEqual(4, (int)GameProgressionStage.CASTLE_LATE);
        Assert.AreEqual(5, (int)GameProgressionStage.EARTH_LATE);
    }

    [Test]
    public void GameStatus_Enum_ContainsAllExpectedValues()
    {
        Assert.AreEqual(0, (int)GameStatus.INGAME);
        Assert.AreEqual(1, (int)GameStatus.MAIN_MENU);
        Assert.AreEqual(2, (int)GameStatus.PAUSE_MENU);
        Assert.AreEqual(3, (int)GameStatus.DIALOGUE);
        Assert.AreEqual(4, (int)GameStatus.SETTINGS_MENU);
        Assert.AreEqual(5, (int)GameStatus.DEATH_SCREEN);
    }

    [Test]
    public void GameSaveData_CanCreateInstanceAndSetProperties()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.progressionStage = GameProgressionStage.CASTLE_END;
        saveData.maxHealth = 100;
        saveData.maxMana = 50;

        Assert.AreEqual(GameProgressionStage.CASTLE_END, saveData.progressionStage);
        Assert.AreEqual(100, saveData.maxHealth);
        Assert.AreEqual(50, saveData.maxMana);
    }

    [Test]
    public void GameSaveData_DefaultValuesAreCorrect()
    {
        GameSaveData saveData = new GameSaveData();

        // Assert that default values are what you expect.  Adjust as necessary.
        Assert.AreEqual(default(GameProgressionStage), saveData.progressionStage);
        Assert.AreEqual(3, saveData.maxHealth);
        Assert.AreEqual(3, saveData.maxMana);
    }
}