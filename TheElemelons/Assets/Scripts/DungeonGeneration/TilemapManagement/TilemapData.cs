using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilemapData", menuName = "ScriptableObjects/TilemapData", order = 1)]
public class TilemapData : ScriptableObject
{
    public List<Vector3Int> tilePositions;
    public List<TileBase> tiles;
}

