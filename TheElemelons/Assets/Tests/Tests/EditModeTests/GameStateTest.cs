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
        saveData.saveFileName = "test_save";

        Assert.AreEqual(GameProgressionStage.CASTLE_END, saveData.progressionStage);
        Assert.AreEqual(100, saveData.maxHealth);
        Assert.AreEqual(50, saveData.maxMana);
        Assert.AreEqual("test_save", saveData.saveFileName);
    }

    [Test]
    public void SerializableRoom_CanCreateInstanceAndSetProperties()
    {
        Room room = new Room();
        room.AddFloorPosition(new UnityEngine.Vector2Int(1, 1));
        room.AddWallPosition(new UnityEngine.Vector2Int(2, 2));
        room.SetRoomType(RoomType.BOSS);

        SerializableRoom serializableRoom = new SerializableRoom(room);

        Assert.AreEqual(1, serializableRoom.floorPositions.Count);
        Assert.AreEqual(1, serializableRoom.wallPositions.Count);
        Assert.AreEqual(RoomType.BOSS, serializableRoom.roomType);
    }

    [Test]
    public void SerializableMapData_CanCreateInstanceAndSetProperties()
    {
        MapData mapData = new MapData();
        Room room = new Room();
        room.AddFloorPosition(new UnityEngine.Vector2Int(1, 1));
        mapData.AddRoom(room);
        mapData.AddWallPosition(new UnityEngine.Vector2Int(2, 2));

        SerializableMapData serializableMapData = new SerializableMapData(mapData);

        Assert.AreEqual(1, serializableMapData.rooms.Count);
        Assert.AreEqual(1, serializableMapData.wallPositions.Count);
    }
}