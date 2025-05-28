using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogOnWalkInTrigger : DialogueTrigger
{
    public bool canInteract = true;
    public override void Interact()
    {
    }
    public void DoDialog()
    {
        base.Interact();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (canInteract && other.gameObject.CompareTag("Player") &&
            other.isTrigger)
        {
            DoDialog();
        }
    }
}
