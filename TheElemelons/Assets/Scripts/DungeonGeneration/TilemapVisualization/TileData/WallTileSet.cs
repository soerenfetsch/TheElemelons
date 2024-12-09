using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "WallTileSet", menuName = "ScriptableObjects/WallTileSet", order = 1)]
public class WallTileSet : ScriptableObject
{
    public TileBase leftWall, leftCorner, rightWall, rightCorner;
    public TileBase topWallBottom, topWallUpper, bottomWall;
}
