using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class AbstractTilemapVisualizer : MonoBehaviour
{
    // Public method to paint a list of tiles on the dungeon map.
    public abstract void PaintTiles(MapData mapData);

    // Clear the existing dungeon tiles (reset Tilemap).
    public abstract void Clear();
}

