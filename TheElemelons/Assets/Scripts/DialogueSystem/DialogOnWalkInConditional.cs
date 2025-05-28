using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogOnWalkInConditional : DialogOnWalkInTrigger
{
    public List<GameProgressionStage> canInteractCondition;
    public float dialogueCooldown = 60f;
    /*public override void DoDialog()
    {
        base.Interact();
        StartCoroutine(DialogueCooldown());
    }*/
    IEnumerator DialogueCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(dialogueCooldown);
        canInteract = true;
    }
    void Update()
    {
        if (canInteractCondition != null)
        {
            canInteract = canInteractCondition.Contains(GameManager.Instance.CurrentProgressionStage);
        }
    }
}
