using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
<<<<<<< Updated upstream
/// Manages saving and printing tile positions from a Tilemap to a TilePositionData ScriptableObject.
/// Used for storing and debugging specific tile positions in the dungeon.
=======
/// Manages saving and printing tile positions from a <see cref="Tilemap"/> to a <see cref="TilePositionData"/> asset.
/// Useful for dungeon generation, pathfinding, or feature placement systems that require lists of tile center positions.
>>>>>>> Stashed changes
/// </summary>
public class TilePositionManager : MonoBehaviour
{
    /// <summary>
    /// The tilemap from which to read tile positions.
    /// </summary>
    public Tilemap tilemap;

    /// <summary>
    /// The <see cref="TilePositionData"/> asset to store the saved tile positions.
    /// </summary>
    public TilePositionData tilePositionData;

    /// <summary>
<<<<<<< Updated upstream
    /// Saves the positions of all non-null tiles in the tilemap to the tilePositionData object.
=======
    /// Saves the center positions of all non-null tiles in the <see cref="tilemap"/> to <see cref="tilePositionData"/>.
    /// Each position is stored as a Vector2 (center of the tile cell).
>>>>>>> Stashed changes
    /// </summary>
    [ContextMenu("Save Tile Positions")]
    public void SaveTilePositions()
    {
        tilePositionData.tilePositions = new List<Vector2>();

        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(position);
            if (tile != null)
            {
                // Store the center of the tile cell as a Vector2
                tilePositionData.tilePositions.Add(new Vector2(position.x + 0.5f, position.y + 0.5f));
            }
        }

#if UNITY_EDITOR
        // Save the asset to make changes persistent in the Unity Editor
        EditorUtility.SetDirty(tilePositionData);
        AssetDatabase.SaveAssets();
        Debug.Log("Tilemap data saved successfully.");
#else
        Debug.LogWarning("Tile position data save is only available in the Unity Editor.");
#endif
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Prints all saved tile positions to the console for debugging.
=======
    /// Prints all saved tile positions from <see cref="tilePositionData"/> to the console.
>>>>>>> Stashed changes
    /// </summary>
    [ContextMenu("Print tile positions")]
    public void PrintTilePositions()
    {
        foreach (var position in tilePositionData.tilePositions)
        {
            Debug.Log(position);
        }
    }
}