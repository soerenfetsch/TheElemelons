using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
<<<<<<< Updated upstream
/// ScriptableObject that stores tile references for horizontal and vertical doors used in dungeon tilemaps.
/// Provides nested classes for organizing door tile variations.
=======
/// ScriptableObject that defines tile sets for dungeon doors in all four cardinal directions.
/// Provides nested classes for horizontal and vertical door tile arrangements.
>>>>>>> Stashed changes
/// </summary>
[CreateAssetMenu(fileName = "DoorTileSet", menuName = "ScriptableObjects/DoorTileSet", order = 1)]
public class DoorTileSet : ScriptableObject
{
    /// <summary>
<<<<<<< Updated upstream
    /// Stores tile references for a horizontal door (top or bottom).
=======
    /// Represents a horizontal door (top or bottom) composed of multiple tile segments.
>>>>>>> Stashed changes
    /// </summary>
    [System.Serializable]
    public class HorizontalDoor
    {
        /// <summary>
        /// The bottom-left tile of the horizontal door.
        /// </summary>
        public TileBase doorBottomLeft;
        /// <summary>
        /// The bottom-middle tile of the horizontal door.
        /// </summary>
        public TileBase doorBottomMiddle;
        /// <summary>
        /// The bottom-right tile of the horizontal door.
        /// </summary>
        public TileBase doorBottomRight;
        /// <summary>
        /// The upper-left tile of the horizontal door.
        /// </summary>
        public TileBase doorUpperLeft;
        /// <summary>
        /// The upper-middle tile of the horizontal door.
        /// </summary>
        public TileBase doorUpperMiddle;
        /// <summary>
        /// The upper-right tile of the horizontal door.
        /// </summary>
        public TileBase doorUpperRight;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Stores tile references for a vertical door (left or right).
    /// Tiles are ordered from top to bottom.
=======
    /// Represents a vertical door (left or right) composed of multiple tile segments from top to bottom.
>>>>>>> Stashed changes
    /// </summary>
    [System.Serializable]
    public class VerticalDoor
    {
<<<<<<< Updated upstream
        public TileBase doorOne, doorTwo, doorThree;
        public TileBase doorFour, doorFive;
=======
        /// <summary>
        /// The topmost tile of the vertical door.
        /// </summary>
        public TileBase doorOne;
        /// <summary>
        /// The second tile from the top of the vertical door.
        /// </summary>
        public TileBase doorTwo;
        /// <summary>
        /// The middle tile of the vertical door.
        /// </summary>
        public TileBase doorThree;
        /// <summary>
        /// The fourth tile from the top of the vertical door.
        /// </summary>
        public TileBase doorFour;
        /// <summary>
        /// The bottommost tile of the vertical door.
        /// </summary>
        public TileBase doorFive;
>>>>>>> Stashed changes
    }

    /// <summary>
    /// Tile set for the top (north) horizontal door.
    /// </summary>
    public HorizontalDoor topDoor;
    /// <summary>
    /// Tile set for the bottom (south) horizontal door.
    /// </summary>
    public HorizontalDoor bottomDoor;
    /// <summary>
    /// Tile set for the left (west) vertical door.
    /// </summary>
    public VerticalDoor leftDoor;
    /// <summary>
    /// Tile set for the right (east) vertical door.
    /// </summary>
    public VerticalDoor rightDoor;
}
