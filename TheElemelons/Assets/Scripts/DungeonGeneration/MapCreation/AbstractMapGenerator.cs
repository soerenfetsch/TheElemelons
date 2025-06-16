using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Abstract base class for map generators. Provides an interface for generating the entire dungeon map,
/// including floors, walls, decorations, and spawn points.
=======
/// Abstract base class for all map generators.
/// Provides an interface for generating a complete map, including floors, walls, decorations, and spawn points.
>>>>>>> Stashed changes
/// </summary>
public abstract class AbstractMapGenerator : MonoBehaviour
{
    /// <summary>
<<<<<<< Updated upstream
    /// Public method to create the entire map (floors, walls, decorations, spawn points).
    /// </summary>
    /// <returns>The generated MapData object containing all map information.</returns>
=======
    /// Creates and returns the entire map, including floors, walls, decorations, and spawn points.
    /// </summary>
    /// <returns>A <see cref="MapData"/> object representing the generated map.</returns>
>>>>>>> Stashed changes
    public abstract MapData GenerateMap();
}