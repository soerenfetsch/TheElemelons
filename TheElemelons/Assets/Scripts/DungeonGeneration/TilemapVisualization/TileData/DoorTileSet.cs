using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DoorTileSet", menuName = "ScriptableObjects/DoorTileSet", order = 1)]
public class DoorTileSet : ScriptableObject
{
    [System.Serializable]
    public class HorizontalDoor
    {
        public TileBase doorBottomLeft, doorBottomMiddle, doorBottomRight;
        public TileBase doorUpperLeft, doorUpperMiddle, doorUpperRight;
    }

    [System.Serializable]
    public class VerticalDoor
    {
        // Tiles go from top to bottom
        public TileBase doorOne, doorTwo, doorThree;
        public TileBase doorFour, doorFive;
    }

    public HorizontalDoor topDoor, bottomDoor;
    public VerticalDoor leftDoor, rightDoor;
}
