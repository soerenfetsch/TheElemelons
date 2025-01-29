using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    public Vector2 destination;

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.transform.position = destination;
    }
}
