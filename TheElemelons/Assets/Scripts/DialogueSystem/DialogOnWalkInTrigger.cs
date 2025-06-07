using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Start a dialogue on collision
/// can be set to not repeat, to only trigger during a certain game progression stage or to be enabled by external condition
/// </summary>
public class DialogOnWalkInTrigger : DialogueTrigger
{
    public bool canInteract = true;
    public bool canRepeat = true;
    bool wasTriggered = false;
    public List<GameProgressionStage> canInteractCondition;
    public float dialogueCooldown = 60f;
    public bool otherCondition = true; // set externally
    public override void Interact()
    { }
    public virtual void DoDialog()
    {
        base.Interact();
        StartCoroutine(DialogueCooldown());
    }
    IEnumerator DialogueCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(dialogueCooldown);
        canInteract = true;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (canInteract && other.gameObject.CompareTag("Player") &&
            other.isTrigger && (canRepeat || !wasTriggered) && otherCondition &&
            canInteractCondition.Contains(GameManager.Instance.CurrentProgressionStage))
        {
            DoDialog();
            wasTriggered = true;
        }
    }
}
