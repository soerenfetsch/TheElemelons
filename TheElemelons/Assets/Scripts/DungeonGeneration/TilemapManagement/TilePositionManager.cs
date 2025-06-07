using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages saving and printing tile positions from a Tilemap to a TilePositionData ScriptableObject.
/// Used for storing and debugging specific tile positions in the dungeon.
/// </summary>
public class TilePositionManager : MonoBehaviour
{
    public Tilemap tilemap;
    public TilePositionData tilePositionData;

    /// <summary>
    /// Saves the positions of all non-null tiles in the tilemap to the tilePositionData object.
    /// </summary>
    [ContextMenu("Save Tile Positions")]
    public void SaveTilePositions()
    {
        tilePositionData.tilePositions = new List<Vector2Int>();

        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(position);
            if (tile != null)
            {
                tilePositionData.tilePositions.Add(new Vector2Int(position.x, position.y));
            }
        }

        // Save the asset to make changes persistent
        EditorUtility.SetDirty(tilePositionData);
        AssetDatabase.SaveAssets();
        Debug.Log("Tilemap data saved successfully.");
    }

    /// <summary>
    /// Prints all saved tile positions to the console for debugging.
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