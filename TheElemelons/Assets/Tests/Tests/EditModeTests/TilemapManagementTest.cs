using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TilemapTests
{
    private GameObject tilemapGameObject;
    private Tilemap tilemap;
    private TilemapData tilemapData;
    private TilemapLoader tilemapLoader;
    private TilemapSaver tilemapSaver;

    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject with a Tilemap component
        tilemapGameObject = new GameObject("Tilemap");
        tilemap = tilemapGameObject.AddComponent<Tilemap>();
        var tilemapRenderer = tilemapGameObject.AddComponent<TilemapRenderer>();

        // Create a new TilemapData ScriptableObject
        tilemapData = ScriptableObject.CreateInstance<TilemapData>();
        tilemapData.tilePositions = new List<Vector3Int>();
        tilemapData.tiles = new List<TileBase>();

        // Create TilemapLoader and TilemapSaver components
        tilemapLoader = tilemapGameObject.AddComponent<TilemapLoader>();
        tilemapLoader.tilemapData = tilemapData;
        tilemapLoader.tilemap = tilemap;

        tilemapSaver = tilemapGameObject.AddComponent<TilemapSaver>();
        tilemapSaver.tilemapData = tilemapData;
        tilemapSaver.tilemap = tilemap;
    }

    [TearDown]
    public void TearDown()
    {
        // Destroy the created GameObject and ScriptableObject
        Object.DestroyImmediate(tilemapGameObject);
        Object.DestroyImmediate(tilemapData);
    }

    [UnityTest]
    public IEnumerator TestSaveAndLoadTilemapData()
    {
        // Create a new tile and set it at position (0, 0, 0)
        var tile = ScriptableObject.CreateInstance<Tile>();
        var position = new Vector3Int(0, 0, 0);
        tilemap.SetTile(position, tile);

        // Save the tilemap data
        tilemapSaver.SaveTilemapData();

        // Clear the tilemap
        tilemapLoader.ClearTilemap();
        Assert.AreEqual(0, tilemap.GetUsedTilesCount(), "Tilemap should be cleared.");

        // Load the tilemap data
        tilemapLoader.LoadTilemapData();
        Assert.AreEqual(1, tilemap.GetUsedTilesCount(), "Tilemap should have one tile.");
        Assert.AreEqual(tile, tilemap.GetTile(position), "The loaded tile should match the saved tile.");

        yield return null;
    }
}
