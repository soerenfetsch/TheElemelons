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

    [Header("UI")]
    /// <summary>
    /// Array of mana star images representing mana.
    /// </summary>
    public Image[] manaStars;
    public void UpdateStarFullness(float mana, float maxMana)
    {
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
            }
            else if (i > mana)
            {
                manaStars[i].GetComponent<Animator>().SetFloat("fullness", 0);
            }
            else
            {
                if (mana % 1 == 0)
                {
                    if (mana < maxMana)
                    {
                        manaStars[i].GetComponent<Animator>().SetFloat("fullness", 0);
                    }
                    else
                    {
                        manaStars[i].GetComponent<Animator>().SetFloat("fullness", 1);
                    }
                }
                else
                {

                    float stars = (mana <= 0.125f && mana != 0) ? 0.25f : mana;
                    manaStars[i].GetComponent<Animator>().SetFloat("fullness", stars % 1);
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
