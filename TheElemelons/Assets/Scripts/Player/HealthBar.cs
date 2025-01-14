using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//UI Element of health bar, managing fullness of heart containers
public class HealthBar : MonoBehaviour
{
    public Image[] hearts;

    // Update is called once per frame
    public void UpdateHeartFullness(float health, float maxHealth)
    {
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
            }
            else if (i > health)
            {
                hearts[i].GetComponent<Animator>().SetFloat("fullness", 0);
            }
            else
            {
                if (health % 1 == 0)
                {
                    if (health < maxHealth)
                    {
                        hearts[i].GetComponent<Animator>().SetFloat("fullness", 0);
                    }
                    else
                    {
                        hearts[i].GetComponent<Animator>().SetFloat("fullness", 1);
                    }
                }
                else
                {
                    hearts[i].GetComponent<Animator>().SetFloat("fullness", health % 1);
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
}
