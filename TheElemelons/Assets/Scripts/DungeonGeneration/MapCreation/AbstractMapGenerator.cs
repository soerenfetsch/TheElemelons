using UnityEngine;

/// <summary>
/// Abstract base class for all map generators.
/// Provides an interface for generating a complete map, including floors, walls, decorations, and spawn points.
/// </summary>
public abstract class AbstractMapGenerator : MonoBehaviour
{
    /// <summary>
    /// Creates and returns the entire map, including floors, walls, decorations, and spawn points.
    /// </summary>
    /// <returns>A <see cref="MapData"/> object representing the generated map.</returns>
    public abstract MapData GenerateMap();
}