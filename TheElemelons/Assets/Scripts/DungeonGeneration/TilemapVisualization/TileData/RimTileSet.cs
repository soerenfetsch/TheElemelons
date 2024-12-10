using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "RimTileSet", menuName = "ScriptableObjects/RimTileSet", order = 1)]
public class RimTileSet : ScriptableObject
{
    public TileBase rimLeft, rimTop, rimRight, rimBottom;
    // Corner tiles which run on the outer side of the tile
    public TileBase rimTopLeft, rimTopRight, rimBottomLeft, rimBottomRight;
    // Corner tiles which run on the inner side of the tile
    public TileBase rimTopLeftOther, rimTopRightOther, rimBottomLeftOther, rimBottomRightOther;
}
