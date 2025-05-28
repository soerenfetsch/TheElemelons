using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogOnWalkInTrigger : DialogueTrigger
{
    public bool canInteract = true;
    public bool canRepeat = true;
    bool wasTriggered = false;
    public override void Interact()
    { }
    public virtual void DoDialog()
    {
        base.Interact();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (canInteract && other.gameObject.CompareTag("Player") &&
            other.isTrigger && (canRepeat || !wasTriggered))
        {
            DoDialog();
            wasTriggered = true;
        }
    }
}
