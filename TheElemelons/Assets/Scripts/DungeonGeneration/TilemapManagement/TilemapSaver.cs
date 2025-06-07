using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

/// <summary>
/// Saves the current tilemap data by storing the positions and tiles into the TilemapData object.
/// Used for persisting and restoring dungeon tilemaps in the editor.
/// </summary>
public class TilemapSaver : MonoBehaviour
{
    public Tilemap tilemap;
    public TilemapData tilemapData;

    /// <summary>
    /// Saves the current tilemap data by storing the positions and tiles into the tilemapData object.
    /// This method iterates through all positions within the tilemap's cell bounds, checks for non-null tiles,
    /// and adds their positions and tile references to the tilemapData lists.
    /// The changes are then made persistent by marking the tilemapData asset as dirty and saving the assets.
    /// </summary>
    [ContextMenu("Save Tilemap Data")]
    public void SaveTilemapData()
    {
        tilemapData.tilePositions = new List<Vector3Int>();
        tilemapData.tiles = new List<TileBase>();

        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(position);
            if (tile != null)
            {
                tilemapData.tilePositions.Add(position);
                tilemapData.tiles.Add(tile);
            }
        }

        // Save the asset to make changes persistent
        EditorUtility.SetDirty(tilemapData);
        AssetDatabase.SaveAssets();
        Debug.Log("Tilemap data saved successfully.");
    }
}

