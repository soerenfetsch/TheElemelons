using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Just a visual effect. Small green cloud. Gameobject is destroyed after spelllifetime runs out ~0.6s.
/// </summary>
public class WitchGreenPoof : MonoBehaviour
{
    Witch witchScript;
    public float spellLifeTime = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        witchScript = GameObject.Find("Witch").GetComponent<Witch>();
        StartCoroutine(Expire());
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        Destroy(gameObject);
    }
}
