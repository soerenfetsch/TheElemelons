using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilePositionData", menuName = "ScriptableObjects/TilePositionData", order = 1)]
public class TilePositionData : ScriptableObject
{
    public List<Vector2> tilePositions;
}