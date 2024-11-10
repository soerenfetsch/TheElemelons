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

    // Private method to paint the floor tiles
    protected abstract void PaintFloor(IEnumerable<Vector2Int> positions);

    // Private method to paint the wall tiles
    protected abstract void PaintWalls(IEnumerable<Vector2Int> positions);

    // Private method to add decoration for the floors in the dungeon
    protected abstract void DecorateDungeon(IEnumerable<Vector2Int> positions);
}

