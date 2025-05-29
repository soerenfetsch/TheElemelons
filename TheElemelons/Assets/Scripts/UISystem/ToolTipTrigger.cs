using System.Collections;
using System.Collections.Generic;
using Ionic.BZip2;
using UnityEngine;

public class ToolTipTrigger : MonoBehaviour
{
    public bool canRepeat = true;
    bool wasTriggered = false;
    public TutorialTip tutorialTip;
    ToolTip toolTip;
    public bool otherCondition = true;
    public List<GameProgressionStage> gameProgressionConditions;
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        toolTip = player.GetComponent<ToolTip>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if ((canRepeat || !wasTriggered) && otherCondition && other.gameObject.CompareTag("Player") &&
            other.isTrigger && gameProgressionConditions.Contains(GameManager.Instance.CurrentProgressionStage))
        {
            wasTriggered = true;
            toolTip.ChangeCurrentTip(tutorialTip);
        }
    }
}
