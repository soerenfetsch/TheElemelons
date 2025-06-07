using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Door in the castle hub that can only be opened or closed depending on the current game progression stage.
/// </summary>
public class Door_CastleHub_2 : Door
{
    public List<GameProgressionStage> canOpenCondition;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (canOpenCondition != null)
        {
            canOpen = !canOpenCondition.Contains(GameManager.Instance.CurrentProgressionStage);
        }
    }
}
