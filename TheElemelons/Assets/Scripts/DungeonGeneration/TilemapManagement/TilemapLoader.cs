using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
<<<<<<< Updated upstream
/// Loads and clears tilemap data for the dungeon. 
/// Uses a TilemapData ScriptableObject to restore or reset the tilemap's state.
=======
/// Loads and manages tilemap data for saving and loading tilemaps in the dungeon.
/// Provides methods to load tiles from a <see cref="TilemapData"/> asset and clear the tilemap.
>>>>>>> Stashed changes
/// </summary>
public class TilemapLoader : MonoBehaviour
{
    /// <summary>
    /// The <see cref="TilemapData"/> asset containing the saved tile positions and tiles.
    /// </summary>
    public TilemapData tilemapData;

    /// <summary>
    /// The <see cref="Tilemap"/> component to load tiles into or clear.
    /// </summary>
    public Tilemap tilemap;

    /// <summary>
    /// Constructs a new TilemapLoader with the given tilemap data and tilemap.
    /// </summary>
    /// <param name="tilemapData">The tilemap data asset to use for loading.</param>
    /// <param name="tilemap">The tilemap to load into or clear.</param>
    public TilemapLoader(TilemapData tilemapData, Tilemap tilemap)
    {
        this.tilemapData = tilemapData;
        this.tilemap = tilemap;
    }

    /// <summary>
    /// Loads the tilemap data from the <see cref="TilemapData"/> object and sets the tiles on the tilemap.
    /// This method clears all existing tiles on the tilemap and then sets the tiles based on the positions
    /// and tile data stored in the <see cref="TilemapData"/> object.
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

