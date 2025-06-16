using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
<<<<<<< Updated upstream
/// ScriptableObject that stores tile references for rim (border) tiles used in dungeon tilemaps,
/// including sides and both outer and inner corners.
=======
/// ScriptableObject that defines tile sets for dungeon rim (border) tiles, including sides and corners.
/// Supports both outer and inner corner tiles for flexible dungeon visualization.
>>>>>>> Stashed changes
/// </summary>
[CreateAssetMenu(fileName = "RimTileSet", menuName = "ScriptableObjects/RimTileSet", order = 1)]
public class RimTileSet : ScriptableObject
{
    /// <summary>
    /// Tile for the left rim (west border).
    /// </summary>
    public TileBase rimLeft;
    /// <summary>
    /// Tile for the top rim (north border).
    /// </summary>
    public TileBase rimTop;
    /// <summary>
    /// Tile for the right rim (east border).
    /// </summary>
    public TileBase rimRight;
    /// <summary>
    /// Tile for the bottom rim (south border).
    /// </summary>
    public TileBase rimBottom;

    /// <summary>
    /// Tile for the outer top-left corner.
    /// </summary>
    public TileBase rimTopLeft;
    /// <summary>
    /// Tile for the outer top-right corner.
    /// </summary>
    public TileBase rimTopRight;
    /// <summary>
    /// Tile for the outer bottom-left corner.
    /// </summary>
    public TileBase rimBottomLeft;
    /// <summary>
    /// Tile for the outer bottom-right corner.
    /// </summary>
    public TileBase rimBottomRight;

    /// <summary>
    /// Tile for the inner top-left corner.
    /// </summary>
    public TileBase rimTopLeftOther;
    /// <summary>
    /// Tile for the inner top-right corner.
    /// </summary>
    public TileBase rimTopRightOther;
    /// <summary>
    /// Tile for the inner bottom-left corner.
    /// </summary>
    public TileBase rimBottomLeftOther;
    /// <summary>
    /// Tile for the inner bottom-right corner.
    /// </summary>
    public TileBase rimBottomRightOther;
}
