using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

/// <summary>
/// Saves the current state of a tilemap to a <see cref="TilemapData"/> asset.
/// Provides functionality to serialize tile positions and their corresponding tiles for later loading.
/// </summary>
public class TilemapSaver : MonoBehaviour
{
    /// <summary>
    /// The <see cref="Tilemap"/> component whose data will be saved.
    /// </summary>
    public Tilemap tilemap;

    /// <summary>
    /// The <see cref="TilemapData"/> asset to store the saved tile positions and tiles.
    /// </summary>
    public TilemapData tilemapData;

    /// <summary>
    /// Saves the current tilemap data by storing the positions and tiles into the <see cref="tilemapData"/> object.
    /// This method iterates through all positions within the tilemap's cell bounds, checks for non-null tiles,
    /// and adds their positions and tile references to the <see cref="tilemapData"/> lists.
    /// The changes are then made persistent by marking the <see cref="tilemapData"/> asset as dirty and saving the assets.
    /// </summary>
    [ContextMenu("Save Tilemap Data")]
    public void SaveTilemapData()
    {
        // Initialize or clear the lists before saving
        tilemapData.tilePositions = new List<Vector3Int>();
        tilemapData.tiles = new List<TileBase>();

        // Iterate through all positions in the tilemap's bounds
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(position);
            if (tile != null)
            {
                tilemapData.tilePositions.Add(position);
                tilemapData.tiles.Add(tile);
            }
        }

#if UNITY_EDITOR
        // Save the asset to make changes persistent
        EditorUtility.SetDirty(tilemapData);
        AssetDatabase.SaveAssets();
        Debug.Log("Tilemap data saved successfully.");
#else
        Debug.LogWarning("Tilemap data save is only available in the Unity Editor.");
#endif
    }
}

