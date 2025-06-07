using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ScriptableObject that stores tile references for rim (border) tiles used in dungeon tilemaps,
/// including sides and both outer and inner corners.
/// </summary>
[CreateAssetMenu(fileName = "RimTileSet", menuName = "ScriptableObjects/RimTileSet", order = 1)]
public class RimTileSet : ScriptableObject
{
    public TileBase rimLeft, rimTop, rimRight, rimBottom;
    // Corner tiles which run on the outer side of the tile
    public TileBase rimTopLeft, rimTopRight, rimBottomLeft, rimBottomRight;
    // Corner tiles which run on the inner side of the tile
    public TileBase rimTopLeftOther, rimTopRightOther, rimBottomLeftOther, rimBottomRightOther;
}
