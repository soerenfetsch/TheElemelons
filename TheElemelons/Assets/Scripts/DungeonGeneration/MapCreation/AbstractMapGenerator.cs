using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMapGenerator : MonoBehaviour
{
    // Public method to create the entire map (floors, walls, decorations, spawn points)
    public abstract MapData GenerateMap();
}