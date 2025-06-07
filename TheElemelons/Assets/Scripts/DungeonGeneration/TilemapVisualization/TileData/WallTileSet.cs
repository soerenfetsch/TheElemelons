using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ScriptableObject that stores tile references for wall tiles used in dungeon tilemaps,
/// including left, right, top, and bottom walls and their corners.
/// </summary>
[CreateAssetMenu(fileName = "WallTileSet", menuName = "ScriptableObjects/WallTileSet", order = 1)]
public class WallTileSet : ScriptableObject
{
    public TileBase leftWall, leftCorner, rightWall, rightCorner;
    public TileBase topWallBottom, topWallUpper, bottomWall;
}
