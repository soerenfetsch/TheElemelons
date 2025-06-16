using System.Collections.Generic;
using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// ScriptableObject that stores a list of tile positions as Vector2Int values.
/// Used for saving and loading specific tile positions in the dungeon.
=======
/// ScriptableObject that stores a list of tile positions (as Vector2) for use in dungeon generation, pathfinding, or other tile-based systems.
/// Typically used to save and load floor or feature positions independently of tile assets.
>>>>>>> Stashed changes
/// </summary>
[CreateAssetMenu(fileName = "TilePositionData", menuName = "ScriptableObjects/TilePositionData", order = 1)]
public class TilePositionData : ScriptableObject
{
    /// <summary>
    /// List of tile positions (center points in world or cell space) used for floor, pathfinding, or feature placement.
    /// </summary>
    public List<Vector2> tilePositions;
}