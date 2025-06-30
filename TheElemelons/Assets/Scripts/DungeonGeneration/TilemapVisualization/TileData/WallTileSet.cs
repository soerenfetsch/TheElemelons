using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ScriptableObject that defines tile sets for dungeon walls, including sides and corners.
/// Used for flexible wall visualization in dungeon generation.
/// </summary>
[CreateAssetMenu(fileName = "WallTileSet", menuName = "ScriptableObjects/WallTileSet", order = 1)]
public class WallTileSet : ScriptableObject
{
    /// <summary>
    /// Tile for the left wall (west border).
    /// </summary>
    public TileBase leftWall;
    /// <summary>
    /// Tile for the left wall corner (northwest or southwest).
    /// </summary>
    public TileBase leftCorner;
    /// <summary>
    /// Tile for the right wall (east border).
    /// </summary>
    public TileBase rightWall;
    /// <summary>
    /// Tile for the right wall corner (northeast or southeast).
    /// </summary>
    public TileBase rightCorner;
    /// <summary>
    /// Tile for the bottom segment of the top wall (north border, lower row).
    /// </summary>
    public TileBase topWallBottom;
    /// <summary>
    /// Tile for the upper segment of the top wall (north border, upper row).
    /// </summary>
    public TileBase topWallUpper;
    /// <summary>
    /// Tile for the bottom wall (south border).
    /// </summary>
    public TileBase bottomWall;
}
