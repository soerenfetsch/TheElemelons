

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePositionManager : MonoBehaviour
{
    public Tilemap tilemap;
    public TilePositionData tilePositionData;

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

#if UNITY_EDITOR
        // Save the asset to make changes persistent
        EditorUtility.SetDirty(tilePositionData);
        AssetDatabase.SaveAssets();
        Debug.Log("Tilemap data saved successfully.");
#else
        Debug.LogWarning("Tile position data save is only available in the Unity Editor.");
#endif
    }

    [ContextMenu("Print tile positions")]
    public void PrintTilePositions()
    {
        foreach (var position in tilePositionData.tilePositions)
        {
            Debug.Log(position);
        }
    }
}