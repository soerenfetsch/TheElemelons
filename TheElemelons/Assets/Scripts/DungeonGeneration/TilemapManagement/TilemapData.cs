using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ScriptableObject that stores tilemap data, including tile positions and their corresponding tiles.
/// Used for saving and loading tilemaps in the dungeon.
/// </summary>
[CreateAssetMenu(fileName = "TilemapData", menuName = "ScriptableObjects/TilemapData", order = 1)]
public class TilemapData : ScriptableObject
{
    public List<Vector3Int> tilePositions;
    public List<TileBase> tiles;
}

