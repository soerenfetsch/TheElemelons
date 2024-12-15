using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Health healthScript;
    float maxHealth;
    float health;
    public Image[] hearts;
    public Sprite heart0;
    public Sprite heart1;
    public Sprite heart2;
    public Sprite heart3;
    public Sprite heart4;
    // Start is called before the first frame update
    void Start()
    {
        healthScript = GetComponent<Health>();
        maxHealth = healthScript.GetMaxHealth();
        health = healthScript.GetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (healthScript.GetHealth() < health)
        {
            Hurt();
        }
        else if (healthScript.GetHealth() > health)
        {
            Heal();
        }
        health = healthScript.GetHealth();
        for (int i = 0; i < hearts.Length; i++)
        {
            //number of heart containers
            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
            //fullness of heart containers
            if (i < Mathf.Floor(health))
            {
                hearts[i].GetComponent<Animator>().SetFloat("fullness", 1);
                hearts[i].sprite = heart4;
            }
            else if (i > health)
            {
                hearts[i].GetComponent<Animator>().SetFloat("fullness", 0);
                hearts[i].sprite = heart0;
            }
            else
            {
                if (health % 1 == 0)
                {
                    if (health < maxHealth)
                    {
                        hearts[i].GetComponent<Animator>().SetFloat("fullness", 0);
                        hearts[i].sprite = heart0;
                    }
                    else
                    {
                        hearts[i].GetComponent<Animator>().SetFloat("fullness", 1);
                        hearts[i].sprite = heart4;
                    }
                }
                else if (health % 1 <= 0.25)
                {
                    hearts[i].GetComponent<Animator>().SetFloat("fullness", health % 1);
                    hearts[i].sprite = heart1;
                }
                else if (health % 1 <= 0.5)
                {
                    hearts[i].GetComponent<Animator>().SetFloat("fullness", health % 1);
                    hearts[i].sprite = heart2;
                }
                else if (health % 1 <= 0.75)
                {
                    hearts[i].GetComponent<Animator>().SetFloat("fullness", health % 1);
                    hearts[i].sprite = heart3;
                }
            }
        }
    }
    public void Hurt()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<Animator>().SetTrigger("hurt");
        }
    }

    public void Heal()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<Animator>().SetTrigger("heal");
        }
    }
    public void Dead()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<Animator>().SetFloat("fullness", 0);
        }
    }
}
