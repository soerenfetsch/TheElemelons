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

    /// <summary>
    /// Loads the tilemap data from the TilemapData object and sets the tiles on the tilemap.
    /// This method clears all existing tiles on the tilemap and then sets the tiles based on the positions
    /// and tile data stored in the TilemapData object.
    /// </summary>
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

    /// <summary>
    /// Clears all tiles on the tilemap.
    /// This method removes all existing tiles from the tilemap and logs a message indicating that the tilemap has been cleared.
    /// </summary>
    [ContextMenu("Clear Tilemap")]
    public void ClearTilemap()
    {
        tilemap.ClearAllTiles();
        Debug.Log("Tilemap cleared successfully.");
    }
}

