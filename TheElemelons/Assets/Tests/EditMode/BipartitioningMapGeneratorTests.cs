using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[TestFixture]
public class BipartitioningMapGeneratorTests
{
    private BipartitioningMapGenerator generator;

    [SetUp]
    public void SetUp()
    {
        generator = new BipartitioningMapGenerator();
    }

    [Test]
    public void CreateFloor_ShouldCreateTilesWithinBoundary()
    {
        MapData mapData = new MapData();
        generator.startPosition = new Vector2Int(0, 0);
        generator.dungeonWidth = 100;
        generator.dungeonHeight = 100;

        generator.CreateFloor(mapData);

        // Assert that map data is not empty
        Assert.IsNotEmpty(mapData.floorPositions,
            $"Expected: Not empty\n" +
            $"Actual: Empty");

        foreach (var position in mapData.floorPositions)
        {
            Assert.IsTrue(position.x >= 0 && position.x < generator.dungeonWidth,
                $"Expected: x >= 0 && x < {generator.dungeonWidth}\n" +
                $"Actual: x = {position.x}");
            Assert.IsTrue(position.y >= 0 && position.y < generator.dungeonHeight,
                $"Expected: y >= 0 && y < {generator.dungeonHeight}\n" +
                $"Actual: y = {position.y}");
        }
    }

    [Test]
    public void CreateWalls_ShouldCreateWallsAroundRoom()
    {
        // Add a 3x3 room to the map data
        MapData mapData = new MapData();
        Room room = new Room();
        for (int x = 1; x <= 3; x++)
        {
            for (int y = 1; y <= 3; y++)
            {
                room.AddFloorPosition(new Vector2Int(x, y));
            }
        }
        mapData.AddRoom(room);

        generator.CreateWalls(mapData);

        // Assert that wall is placed outside the room
        HashSet<Vector2Int> expectedWallPositions = new HashSet<Vector2Int>
        {
            new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0),
            new Vector2Int(0, 1), new Vector2Int(4, 1),
            new Vector2Int(0, 2), new Vector2Int(4, 2),
            new Vector2Int(0, 3), new Vector2Int(4, 3),
            new Vector2Int(0, 4), new Vector2Int(1, 4), new Vector2Int(2, 4), new Vector2Int(3, 4), new Vector2Int(4, 4)
        };

        Assert.IsTrue(mapData.wallPositions.SetEquals(expectedWallPositions),
            $"Expected: {string.Join(", ", expectedWallPositions)}\n" +
            $"Actual: {string.Join(", ", mapData.wallPositions)}");
    }

    [Test]
    public void CreateDoorPositions_ShouldPlaceDoorInMiddleOfCorridor()
    {
        // Arrange
        MapData mapData = new MapData();
        Room room = new Room();
        for (int x = 1; x <= 3; x++)
        {
            for (int y = 1; y <= 3; y++)
            {
                room.AddFloorPosition(new Vector2Int(x, y));
            }
        }
        mapData.AddRoom(room);

        // Add corridor positions
        for (int y = 4; y <= 6; y++)
        {
            mapData.AddFloorPosition(new Vector2Int(2, y));
        }

        // Act
        generator.CreateDoorPositions(mapData);

        // Assert
        Vector2Int expectedDoorPosition = new Vector2Int(2, 4);
        Assert.IsTrue(mapData.doorPositions.Contains(expectedDoorPosition),
            $"Expected map door position: {expectedDoorPosition}\n" +
            $"Actual: {string.Join(", ", mapData.doorPositions)}");
        Assert.IsTrue(room.doorPositions.Contains(expectedDoorPosition),
            $"Expected room door position: {expectedDoorPosition}\n" +
            $"Actual: {string.Join(", ", room.doorPositions)}");
    }
}
