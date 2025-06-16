using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
<<<<<<< Updated upstream
/// ScriptableObject that stores tilemap data, including tile positions and their corresponding tiles.
/// Used for saving and loading tilemaps in the dungeon.
=======
/// ScriptableObject that stores tilemap data for saving and loading tilemaps.
/// Contains lists of tile positions and their corresponding tile assets.
>>>>>>> Stashed changes
/// </summary>
[CreateAssetMenu(fileName = "TilemapData", menuName = "ScriptableObjects/TilemapData", order = 1)]
public class TilemapData : ScriptableObject
{
    /// <summary>
    /// List of positions (in cell coordinates) where tiles are placed on the tilemap.
    /// </summary>
    public List<Vector3Int> tilePositions;

    /// <summary>
    /// List of tiles corresponding to each position in <see cref="tilePositions"/>.
    /// </summary>
    public List<TileBase> tiles;
}

