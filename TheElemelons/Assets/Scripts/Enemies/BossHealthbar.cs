using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthbar : MonoBehaviour
{
    public float moveSpeed = 2f;
    public RectTransform bloodMarker;
    public RectTransform endOfLife;
    public Health bossHealth;
    Vector2 startPos;
    float dist;
    float destination;
    void Start()
    {
        Initialize();
    }
    // Update is called once per frame
    void Update()
    {
        CalcDestination();
        MoveToDestination();
        if (bossHealth == null || bossHealth.gameObject == null)
        {
            ResetBloodMarker();
        }
    }
    void OnEnable()
    {
        ResetBloodMarker();
    }
    public void ResetBloodMarker()
    {
        bloodMarker.localPosition = new Vector3(435, 19, 0);
    }
    void Initialize()
    {
        startPos = bloodMarker.position;
        dist = bloodMarker.position.x - endOfLife.position.x;
        destination = bloodMarker.position.y;
    }
    void CalcDestination()
    {

        float percentage = bossHealth.health / bossHealth.maxhealth;
        destination = endOfLife.position.x + dist * percentage;
        if (bossHealth == null)
        {
            destination = endOfLife.position.x;
        }

    }
    void MoveToDestination()
    {
        if (bloodMarker.position.x - destination > 0.1f)
        {
            Vector2 movement = new Vector2(-1, 0) * moveSpeed * Time.deltaTime;
            bloodMarker.position = bloodMarker.position + (Vector3)movement;
        }
    }
}
