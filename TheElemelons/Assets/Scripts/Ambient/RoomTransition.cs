using UnityEngine;
public class RoomTransition : MonoBehaviour
{
    public Vector2 destination;

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this object.
    /// If the collider belongs to the feet of the player, the player gets teleported to the destination.
    /// </summary>
    /// <param name="other">The Collider2D that has entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Feet"))
        {
            other.transform.parent.position = destination;
        }
    }
}
