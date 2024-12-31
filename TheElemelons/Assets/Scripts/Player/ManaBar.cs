using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{

    [Header("UI")]
    public Image[] manaStars;



    public void UpdateStarFullness(float mana, float maxMana)
    {
        for (int i = 0; i < manaStars.Length; i++)
        {
            //number of heart containers
            if (i < maxMana)
            {
                manaStars[i].enabled = true;
            }
            else
            {
                manaStars[i].enabled = false;
            }
            //fullness of heart containers
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
                    manaStars[i].GetComponent<Animator>().SetFloat("fullness", mana % 1);
                }
            }
        }
    }

    public void Hurt()
    {
        for (int i = 0; i < manaStars.Length; i++)
        {
            manaStars[i].GetComponent<Animator>().SetTrigger("hurt");
        }
    }

    public void Heal()
    {
        for (int i = 0; i < manaStars.Length; i++)
        {
            manaStars[i].GetComponent<Animator>().SetTrigger("heal");
        }
    }
}
