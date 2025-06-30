using UnityEngine;

/// <summary>
/// Abstract class responsible for visualizing the tilemap in the dungeon generation process.
/// </summary>
/// <remarks>
/// This class provides methods to paint and clear tiles on the dungeon map.
/// </remarks>
///
public abstract class AbstractTilemapVisualizer : MonoBehaviour
{
    /// <summary>
    /// Paints a list of tiles on the dungeon map.
    /// </summary>
    /// <param name="mapData">The data representing the map to be painted.</param>
    public abstract void PaintTiles(MapData mapData);

    /// <summary>
    /// Clears the existing dungeon tiles, effectively resetting the Tilemap.
    /// </summary>
    public abstract void Clear();
}

