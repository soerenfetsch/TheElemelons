using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestInteractableA : InteractableObject
{
    public bool wasInteracted = false;

    public override void Interact()
    {
        wasInteracted = true;
        Debug.Log("TestInteractableA was interacted with.");
    }
}


public class TestInteractableB : InteractableObject
{
    public bool wasInteracted = false;

    public override void Interact()
    {
        wasInteracted = true;
        transform.position += Vector3.right * 2; // Moves object for testing
        Debug.Log("TestInteractableB moved on interaction.");
    }
}


public class TestInteractableC : InteractableObject
{
    public int interactionCount = 0;

    public override void Interact()
    {
        interactionCount++;
        Debug.Log($"TestInteractableC has been interacted with {interactionCount} times.");
    }
}