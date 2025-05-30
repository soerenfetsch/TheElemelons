using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearOnProgressionStage : MonoBehaviour
{
    public List<GameProgressionStage> allowedGameProgressionStages;
    // Start is called before the first frame update
    void Start()
    {
        if (!(allowedGameProgressionStages == null) &&
        !allowedGameProgressionStages.Contains(GameManager.Instance.CurrentProgressionStage))
        {
            gameObject.SetActive(false);
        }
    }
}
