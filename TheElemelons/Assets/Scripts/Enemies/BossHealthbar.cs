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
    float dist;
    float destination;
    void Start()
    {
        dist = bloodMarker.position.x - endOfLife.position.x;
        destination = bloodMarker.position.y;
    }
    // Update is called once per frame
    void Update()
    {
        CalcDestination();
        MoveToDestination();
    }
    void CalcDestination()
    {
        if (bossHealth.health > 0f)
        {
            float percentage = bossHealth.health / bossHealth.maxhealth;
            destination = endOfLife.position.x + dist * percentage;
        }
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
