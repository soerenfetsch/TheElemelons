using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//UI Element of health bar, managing fullness of heart containers
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
        //Assign Variables
        healthScript = GetComponent<Health>();
        maxHealth = healthScript.GetMaxHealth();
        health = healthScript.GetHealth();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if hurt or heal animation needs to be performed and act on it
        if (healthScript.GetHealth() < health)
        {
            Hurt();
        }
        else if (healthScript.GetHealth() > health)
        {
            Heal();
        }
        health = healthScript.GetHealth();
        //Iterate through all heart container images
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
    /// <summary>
    /// Trigger Hurt animation
    /// </summary>
    public void Hurt()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<Animator>().SetTrigger("hurt");
        }
    }
    /// <summary>
    /// Trigger Heal animation
    /// </summary>
    public void Heal()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<Animator>().SetTrigger("heal");
        }
    }
    //Extra function for when health is 0
    public void Dead()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<Animator>().SetFloat("fullness", 0);
        }
    }
}
