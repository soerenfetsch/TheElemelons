using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// delegate tip to tooltip of player on collision
/// can be set to not repeat, to only trigger during a certain game progression stage or to be enabled by external condition
/// </summary>
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
