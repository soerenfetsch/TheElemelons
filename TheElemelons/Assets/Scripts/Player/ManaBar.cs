using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI element representing the mana bar. 
/// Manages the fullness of mana star containers based on player mana.
/// </summary>
public class ManaBar : MonoBehaviour
{
    [Header("Mana Management")]
    /// <summary>
    /// Reference to the Mana script for managing player mana.
    /// </summary>
    public Mana manaScript;

    private float maxMana = 5f;
    private float mana;

    [Header("UI")]
    /// <summary>
    /// Array of mana star images representing mana.
    /// </summary>
    public Image[] manaStars;

    /// <summary>
    /// Sprites representing different levels of mana star fullness.
    /// </summary>
    public Sprite manaStar0; // Empty star
    public Sprite manaStar1; // 25% full star
    public Sprite manaStar2; // 50% full star
    public Sprite manaStar3; // 75% full star
    public Sprite manaStar4; // Fully full star

    /// <summary>
    /// Initializes the mana bar and assigns variables.
    /// </summary>
    void Start()
    {
        manaScript = GetComponent<Mana>();
        maxMana = manaScript.GetMaxMana();
        mana = manaScript.GetMana();
    }

    /// <summary>
    /// Updates the mana bar UI to reflect changes in player mana.
    /// </summary>
    void Update()
    {
        // Check if hurt or heal animation needs to be performed and act on it
        if (manaScript.GetMana() < mana)
        {
            Hurt();
        }
        else if (manaScript.GetMana() > mana)
        {
            Heal();
        }
        mana = manaScript.GetMana();

        // Iterate through all mana star images
        for (int i = 0; i < manaStars.Length; i++)
        {
            // Manage visibility of mana stars based on max mana
            if (i < maxMana)
            {
                manaStars[i].enabled = true;
            }
            else
            {
                manaStars[i].enabled = false;
            }

            // Manage fullness of mana stars
            if (i < Mathf.Floor(mana))
            {
                manaStars[i].GetComponent<Animator>().SetFloat("fullness", 1);
                manaStars[i].sprite = manaStar4;
            }
            else if (i > mana)
            {
                manaStars[i].GetComponent<Animator>().SetFloat("fullness", 0);
                manaStars[i].sprite = manaStar0;
            }
            else
            {
                if (mana % 1 == 0)
                {
                    if (mana < maxMana)
                    {
                        manaStars[i].GetComponent<Animator>().SetFloat("fullness", 0);
                        manaStars[i].sprite = manaStar0;
                    }
                    else
                    {
                        manaStars[i].GetComponent<Animator>().SetFloat("fullness", 1);
                        manaStars[i].sprite = manaStar4;
                    }
                }
                else if (mana % 1 <= 0.25)
                {
                    manaStars[i].GetComponent<Animator>().SetFloat("fullness", mana % 1);
                    manaStars[i].sprite = manaStar1;
                }
                else if (mana % 1 <= 0.5)
                {
                    manaStars[i].GetComponent<Animator>().SetFloat("fullness", mana % 1);
                    manaStars[i].sprite = manaStar2;
                }
                else if (mana % 1 <= 0.75)
                {
                    manaStars[i].GetComponent<Animator>().SetFloat("fullness", mana % 1);
                    manaStars[i].sprite = manaStar3;
                }
            }
        }
    }

    /// <summary>
    /// Triggers the hurt animation for all mana stars.
    /// </summary>
    public void Hurt()
    {
        for (int i = 0; i < manaStars.Length; i++)
        {
            manaStars[i].GetComponent<Animator>().SetTrigger("hurt");
        }
    }

    /// <summary>
    /// Triggers the heal animation for all mana stars.
    /// </summary>
    public void Heal()
    {
        for (int i = 0; i < manaStars.Length; i++)
        {
            manaStars[i].GetComponent<Animator>().SetTrigger("heal");
        }
    }
}
