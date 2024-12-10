using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapLoader : MonoBehaviour
{
    public TilemapData tilemapData;
    public Tilemap tilemap;

    public TilemapLoader(TilemapData tilemapData, Tilemap tilemap)
    {
        this.tilemapData = tilemapData;
        this.tilemap = tilemap;
    }

    [ContextMenu("Load Tilemap Data")]
    public void LoadTilemapData()
    {
        tilemap.ClearAllTiles();

        for (int i = 0; i < tilemapData.tilePositions.Count; i++)
        {
            tilemap.SetTile(tilemapData.tilePositions[i], tilemapData.tiles[i]);
        }
        Debug.Log("Tilemap data loaded successfully.");
    }

    [ContextMenu("Clear Tilemap")]
    public void ClearTilemap()
    {
        tilemap.ClearAllTiles();
        Debug.Log("Tilemap cleared successfully.");
    }
}

