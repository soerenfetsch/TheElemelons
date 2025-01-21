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
    /// <summary>
    /// Reference to the Health script for managing player health.
    /// </summary>
    public Health healthScript;

    private float maxHealth;
    private float health;

    /// <summary>
    /// Array of heart container images representing health.
    /// </summary>
    public Image[] hearts;

    /// <summary>
    /// Sprites representing different levels of heart fullness.
    /// </summary>
    public Sprite heart0; // Empty heart
    public Sprite heart1; // 25% full heart
    public Sprite heart2; // 50% full heart
    public Sprite heart3; // 75% full heart
    public Sprite heart4; // Fully full heart

    /// <summary>
    /// Initializes the health bar and assigns variables.
    /// </summary>
    void Start()
    {
        healthScript = GetComponent<Health>();
        maxHealth = healthScript.GetMaxHealth();
        health = healthScript.GetHealth();
    }

    /// <summary>
    /// Updates the health bar UI to reflect changes in player health.
    /// </summary>
    void Update()
    {
        // Check if hurt or heal animation needs to be performed and act on it
        if (healthScript.GetHealth() < health)
        {
            Hurt();
        }
        else if (healthScript.GetHealth() > health)
        {
            Heal();
        }
        health = healthScript.GetHealth();

        // Iterate through all heart container images
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

    /// <summary>
    /// Sets all hearts to empty, used when health is 0.
    /// </summary>
    public void Dead()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<Animator>().SetFloat("fullness", 0);
        }
    }
}
