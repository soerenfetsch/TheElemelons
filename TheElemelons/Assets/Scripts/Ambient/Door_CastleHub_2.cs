using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_CastleHub_2 : Door
{

    public List<GameProgressionStage> canOpenCondition;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (canOpenCondition != null)
        {
            canOpen = !canOpenCondition.Contains(GameManager.Instance.CurrentProgressionStage);
        }
    }
}
