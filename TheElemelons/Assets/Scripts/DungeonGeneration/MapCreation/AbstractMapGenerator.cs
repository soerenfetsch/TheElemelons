using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for map generators. Provides an interface for generating the entire dungeon map,
/// including floors, walls, decorations, and spawn points.
/// </summary>
public abstract class AbstractMapGenerator : MonoBehaviour
{
    /// <summary>
    /// Public method to create the entire map (floors, walls, decorations, spawn points).
    /// </summary>
    /// <returns>The generated MapData object containing all map information.</returns>
    public abstract MapData GenerateMap();
}