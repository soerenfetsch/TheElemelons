using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI element representing the health bar. 
/// Manages the fullness of heart containers based on player health.
/// </summary>
public class HealthBar : MonoBehaviour
{
    public Image[] hearts;

    // Update is called once per frame
    public void UpdateHeartFullness(float health, float maxHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // Manage visibility of heart containers based on max health
            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }

            // Manage fullness of heart containers
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
    /// Triggers the hurt animation for all hearts.
    /// </summary>
    public void Hurt()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<Animator>().SetTrigger("hurt");
        }
    }

    /// <summary>
    /// Triggers the heal animation for all hearts.
    /// </summary>
    public void Heal()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<Animator>().SetTrigger("heal");
        }
    }
}
