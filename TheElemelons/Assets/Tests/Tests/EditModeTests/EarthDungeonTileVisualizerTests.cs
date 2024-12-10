using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[TestFixture]
public class EarthDungeonTileVisualizerTests
{
    private EarthDungeonTileVizualizer visualizer;
    private MapData mapData;
    private Room room;

    [SetUp]
    public void SetUp()
    {
        var gameObject = new GameObject();
        // Create Grid for Tilemaps
        var gridGameObject = new GameObject("Grid", typeof(Grid));
        visualizer = gameObject.AddComponent<EarthDungeonTileVizualizer>();
        // Create and assign Tilemaps to the visualizer
        visualizer.collisionTilemap = CreateTilemap(gridGameObject, "CollisionTilemap");
        visualizer.floorTilemap = CreateTilemap(gridGameObject, "FloorTilemap");
        visualizer.decorationTilemap = CreateTilemap(gridGameObject, "DecorationTilemap");

        visualizer.basicWalls = ScriptableObject.CreateInstance<WallTileSet>();

        // Initialize wall tiles
        visualizer.basicWalls.bottomWall = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicWalls.leftWall = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicWalls.rightWall = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicWalls.topWallBottom = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicWalls.topWallUpper = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicWalls.leftCorner = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicWalls.rightCorner = ScriptableObject.CreateInstance<Tile>();

        visualizer.basicRims = ScriptableObject.CreateInstance<RimTileSet>();

        // Initialize rim tiles
        visualizer.basicRims.rimTopLeft = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicRims.rimTopRight = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicRims.rimBottomLeft = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicRims.rimBottomRight = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicRims.rimTop = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicRims.rimLeft = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicRims.rimRight = ScriptableObject.CreateInstance<Tile>();
        visualizer.basicRims.rimBottom = ScriptableObject.CreateInstance<Tile>();

        // Initialize atlas tilemap data
        visualizer.floorAtlasTilemapData = ScriptableObject.CreateInstance<TilemapData>();
        visualizer.floorAtlasTilemapData.tilePositions = new List<Vector3Int>();
        visualizer.floorAtlasTilemapData.tiles = new List<TileBase>();

        mapData = new MapData();
        room = new Room();
        for (int x = 1; x <= 3; x++)
        {
            for (int y = 1; y <= 3; y++)
            {
                room.AddFloorPosition(new Vector2Int(x, y));
            }
        }
        mapData.AddRoom(room);

        // Add wall positions
        for (int x = 0; x <= 4; x++)
        {
            mapData.AddWallPosition(new Vector2Int(x, 0));
            mapData.AddWallPosition(new Vector2Int(x, 4));
        }
        for (int y = 1; y <= 3; y++)
        {
            mapData.AddWallPosition(new Vector2Int(0, y));
            mapData.AddWallPosition(new Vector2Int(4, y));
        }
    }

    // Helper method to create Tilemaps
    private Tilemap CreateTilemap(GameObject parent, string name)
    {
        var tilemapGO = new GameObject(name, typeof(Tilemap), typeof(TilemapRenderer));
        tilemapGO.transform.parent = parent.transform;
        return tilemapGO.GetComponent<Tilemap>();
    }

    [Test]
    public void PaintWalls_ShouldPlaceWallTilesCorrectly()
    {
        visualizer.PaintWalls(mapData);

        // Bottom row
        for (int x = 0; x <= 4; x++)
        {
            TileBase tile = visualizer.collisionTilemap.GetTile(new Vector3Int(x, 0, 0));
            Assert.AreEqual(visualizer.basicWalls.bottomWall, tile,
                $"Tile at ({x}, 0): " + tile.GetInstanceID() + " is not a bottom wall "
                + visualizer.basicWalls.bottomWall.GetInstanceID());
        }
        // Left side
        for (int y = 1; y <= 4; y++)
        {
            TileBase tile = visualizer.collisionTilemap.GetTile(new Vector3Int(0, y, 0));
            Assert.AreEqual(visualizer.basicWalls.leftWall, tile,
                $"Tile at (0, {y}): " + tile.GetInstanceID() + " is not a left wall "
                + visualizer.basicWalls.leftWall.GetInstanceID());
        }
        // Right side
        for (int y = 1; y <= 4; y++)
        {
            TileBase tile = visualizer.collisionTilemap.GetTile(new Vector3Int(4, y, 0));
            Assert.AreEqual(visualizer.basicWalls.rightWall, tile,
                $"Tile at (4, {y}): " + tile.GetInstanceID() + " is not a right wall "
                + visualizer.basicWalls.rightWall.GetInstanceID());
        }
        // Top row
        TileBase leftCorner = visualizer.collisionTilemap.GetTile(new Vector3Int(0, 5, 0));
        Assert.AreEqual(visualizer.basicWalls.leftCorner, leftCorner,
            $"Tile at (0, 4): " + leftCorner.GetInstanceID() + " is not a left corner "
            + visualizer.basicWalls.leftCorner.GetInstanceID());
        TileBase rightCorner = visualizer.collisionTilemap.GetTile(new Vector3Int(4, 5, 0));
        Assert.AreEqual(visualizer.basicWalls.rightCorner, rightCorner,
            $"Tile at (4, 4): " + rightCorner.GetInstanceID() + " is not a right corner "
            + visualizer.basicWalls.rightCorner.GetInstanceID());
        for (int x = 1; x <= 3; x++)
        {
            TileBase lowerTile = visualizer.collisionTilemap.GetTile(new Vector3Int(x, 4, 0));
            Assert.AreEqual(visualizer.basicWalls.topWallBottom, lowerTile,
                $"Tile at ({x}, 4): " + lowerTile.GetInstanceID() + " is not a top wall bottom "
                + visualizer.basicWalls.topWallBottom.GetInstanceID());
            TileBase upperTile = visualizer.collisionTilemap.GetTile(new Vector3Int(x, 5, 0));
            Assert.AreEqual(visualizer.basicWalls.topWallUpper, upperTile,
                $"Tile at ({x}, 5): " + upperTile.GetInstanceID() + " is not a top wall upper "
                + visualizer.basicWalls.topWallUpper.GetInstanceID());
        }
    }

    [Test]
    public void PaintFloor_ShouldPlaceRimTilesCorrectly()
    {
        visualizer.PaintFloor(mapData);

        TileBase rimTopLeft = visualizer.floorTilemap.GetTile(new Vector3Int(1, 3, 0));
        Assert.AreEqual(visualizer.basicRims.rimTopLeft, rimTopLeft,
            $"Tile at (1, 3): " + rimTopLeft.GetInstanceID() + " is not a top left rim "
            + visualizer.basicRims.rimTopLeft.GetInstanceID());
        TileBase rimTopRight = visualizer.floorTilemap.GetTile(new Vector3Int(3, 3, 0));
        Assert.AreEqual(visualizer.basicRims.rimTopRight, rimTopRight,
            $"Tile at (3, 3): " + rimTopRight.GetInstanceID() + " is not a top right rim "
            + visualizer.basicRims.rimTopRight.GetInstanceID());
        TileBase rimBottomLeft = visualizer.floorTilemap.GetTile(new Vector3Int(1, 1, 0));
        Assert.AreEqual(visualizer.basicRims.rimBottomLeft, rimBottomLeft,
            $"Tile at (1, 1): " + rimBottomLeft.GetInstanceID() + " is not a bottom left rim "
            + visualizer.basicRims.rimBottomLeft.GetInstanceID());
        TileBase rimBottomRight = visualizer.floorTilemap.GetTile(new Vector3Int(3, 1, 0));
        Assert.AreEqual(visualizer.basicRims.rimBottomRight, rimBottomRight,
            $"Tile at (3, 1): " + rimBottomRight.GetInstanceID() + " is not a bottom right rim "
            + visualizer.basicRims.rimBottomRight.GetInstanceID());
        TileBase rimTop = visualizer.floorTilemap.GetTile(new Vector3Int(2, 3, 0));
        Assert.AreEqual(visualizer.basicRims.rimTop, rimTop,
            $"Tile at (2, 3): " + rimTop.GetInstanceID() + " is not a top rim "
            + visualizer.basicRims.rimTop.GetInstanceID());
        TileBase rimLeft = visualizer.floorTilemap.GetTile(new Vector3Int(1, 2, 0));
        Assert.AreEqual(visualizer.basicRims.rimLeft, rimLeft,
            $"Tile at (1, 2): " + rimLeft.GetInstanceID() + " is not a left rim "
            + visualizer.basicRims.rimLeft.GetInstanceID());
        TileBase rimRight = visualizer.floorTilemap.GetTile(new Vector3Int(3, 2, 0));
        Assert.AreEqual(visualizer.basicRims.rimRight, rimRight,
            $"Tile at (3, 2): " + rimRight.GetInstanceID() + " is not a right rim "
            + visualizer.basicRims.rimRight.GetInstanceID());
        TileBase rimBottom = visualizer.floorTilemap.GetTile(new Vector3Int(2, 1, 0));
        Assert.AreEqual(visualizer.basicRims.rimBottom, rimBottom,
            $"Tile at (2, 1): " + rimBottom.GetInstanceID() + " is not a bottom rim "
            + visualizer.basicRims.rimBottom.GetInstanceID());
    }

    [Test]
    public void Clear_ShouldEmptyTilemaps()
    {
        visualizer.floorTilemap.SetTile(new Vector3Int(0, 0, 0), ScriptableObject.CreateInstance<Tile>());
        visualizer.collisionTilemap.SetTile(new Vector3Int(0, 0, 0), ScriptableObject.CreateInstance<Tile>());
        visualizer.decorationTilemap.SetTile(new Vector3Int(0, 0, 0), ScriptableObject.CreateInstance<Tile>());

        visualizer.Clear();

        Assert.AreEqual(0, visualizer.floorTilemap.GetUsedTilesCount(),
            "Floor tilemap should be empty after Clear()");
        Assert.AreEqual(0, visualizer.collisionTilemap.GetUsedTilesCount(),
            "Collision tilemap should be empty after Clear()");
        Assert.AreEqual(0, visualizer.decorationTilemap.GetUsedTilesCount(),
            "Decoration tilemap should be empty after Clear()");
    }
}