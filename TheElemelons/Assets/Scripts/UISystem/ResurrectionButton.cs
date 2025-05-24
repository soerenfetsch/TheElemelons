using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectionButton : MonoBehaviour
{
    [Header("Hovering Motion")]
    public float amplitude = 20f;
    public float frequency = 0.25f;
    Vector2 fixpoint;
    // Start is called before the first frame update
    void Start()
    {
        fixpoint = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 moving_by = Vector2.up * Mathf.Sin(frequency * Time.frameCount * Time.fixedDeltaTime) * amplitude;
        transform.position = fixpoint + moving_by;
    }
}
