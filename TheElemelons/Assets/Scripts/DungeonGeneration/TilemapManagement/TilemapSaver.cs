using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TilemapSaver : MonoBehaviour
{
    public Tilemap tilemap;
    public TilemapData tilemapData;

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

