using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ScriptableObject that stores tile references for horizontal and vertical doors used in dungeon tilemaps.
/// Provides nested classes for organizing door tile variations.
/// </summary>
[CreateAssetMenu(fileName = "DoorTileSet", menuName = "ScriptableObjects/DoorTileSet", order = 1)]
public class DoorTileSet : ScriptableObject
{
    /// <summary>
    /// Stores tile references for a horizontal door (top or bottom).
    /// </summary>
    [System.Serializable]
    public class HorizontalDoor
    {
        public TileBase doorBottomLeft, doorBottomMiddle, doorBottomRight;
        public TileBase doorUpperLeft, doorUpperMiddle, doorUpperRight;
    }

    /// <summary>
    /// Stores tile references for a vertical door (left or right).
    /// Tiles are ordered from top to bottom.
    /// </summary>
    [System.Serializable]
    public class VerticalDoor
    {
        public TileBase doorOne, doorTwo, doorThree;
        public TileBase doorFour, doorFive;
    }

    public HorizontalDoor topDoor, bottomDoor;
    public VerticalDoor leftDoor, rightDoor;
}
