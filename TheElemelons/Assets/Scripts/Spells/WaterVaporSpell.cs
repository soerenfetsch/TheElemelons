using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterVaporSpell : SpellEffect
{

    float spellLifeTime = 10f;
    float fadeDuration = 2f;
    SpriteRenderer spriteRenderer;
    public GameObject questionMark;
    Dictionary<GameObject, GameObject> victims;
    CircleCollider2D circleCollider2D;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        transform.position = caster.transform.position;
        player = GameObject.Find("Player").GetComponent<Player>();
        victims = new Dictionary<GameObject, GameObject>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider2D.enabled = false;
        StartCoroutine(Expire());
        StartCoroutine(ColliderOnOff());
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(2.6f);
        circleCollider2D.enabled = true;
        yield return new WaitForSeconds(spellLifeTime);
        float fadeTimer = fadeDuration;
        Color c = spriteRenderer.color;
        float chunkiness = 30f;
        while (fadeTimer > 0f)
        {
            fadeTimer -= Time.deltaTime * chunkiness;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            yield return new WaitForSeconds(Time.deltaTime * chunkiness);
        }
        Destroy(gameObject);
    }
    IEnumerator ColliderOnOff()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            circleCollider2D.enabled = !circleCollider2D.enabled;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy targetScript = other.GetComponent<Enemy>();
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger &&
        targetScript != null && other.GetComponent<Health>() != null &&
        targetScript.GetCurrentState() != EnemyStatus.Staggered)
        {
            //ConfuseVictim(other.gameObject); iif !victims.ContainsKey(other.gameObject)
            Debug.Log("Confusing : " + other.gameObject.name);
            BuffDebuffManager.Instance.ApplyBuffDebuff(other.gameObject, "WaterVaporConfuse", BuffDebuffManager.BuffDebuffType.ConfusedDebuff, 0.8f, 1, false);

        }
        if (other.gameObject.CompareTag("Player") && other.GetComponent<Player>() != null)
        {
            BuffDebuffManager.Instance.ApplyBuffDebuff(other.gameObject, "WaterVaporHide", BuffDebuffManager.BuffDebuffType.HiddenBuff, 0.8f, 1, false);
        }
    }
    /*
        void OnTriggerExit2D(Collider2D other)
        {
            if (victims.ContainsKey(other.gameObject))
            {
                UnConfuseVictim(other.gameObject);
            }
        }
        void ConfuseVictim(GameObject victim)
        {
            Vector3 pos = victim.transform.position;
            GameObject questionMarkEffect = Instantiate(questionMark, pos, Quaternion.Euler(0, 0, 0));
            questionMarkEffect.transform.parent = victim.transform;
            victims.Add(victim, questionMarkEffect);
            victim.GetComponent<Enemy>().DoObstructSight(true);
        }
        void UnConfuseVictim(GameObject victim, bool removeFromList = true)
        {
            if (victim.GetComponent<Enemy>() == null)
            {
                Debug.Log(victim.gameObject.name + " has no enemy script");
            }
            victim.GetComponent<Enemy>().DoObstructSight(false);
            Destroy(victims[victim]);
            if (removeFromList)
            {
                victims.Remove(victim);
            }
        }

        void OnDestroy()
        {
            foreach ((GameObject key, GameObject value) in victims)
            {
                if (victims[key] != null)
                {
                    UnConfuseVictim(key, removeFromList: false);
                }
            }
        }
    */
}
