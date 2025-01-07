using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//UI Element of mana bar, managing fullness of mana star containers
public class ManaBar : MonoBehaviour
{
    [Header("Mana Management")]
    public Mana manaScript;
    float maxMana = 5f;
    float mana;
    [Header("UI")]
    public Image[] manaStars;
    public Sprite manaStar0;
    public Sprite manaStar1;
    public Sprite manaStar2;
    public Sprite manaStar3;
    public Sprite manaStar4;
    // Start is called before the first frame update
    void Start()
    {
        //Assign Variables
        manaScript = GetComponent<Mana>();
        maxMana = manaScript.GetMaxMana();
        mana = manaScript.GetMana();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if hurt or heal animation needs to be performed and act on it
        if (manaScript.GetMana() < mana)
        {
            Hurt();
        }
        else if (manaScript.GetMana() > mana)
        {
            Heal();
        }
        mana = manaScript.GetMana();
        //Iterate through all mana star container images
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

    //Trigger Hurt animation
    public void Hurt()
    {
        for (int i = 0; i < manaStars.Length; i++)
        {
            manaStars[i].GetComponent<Animator>().SetTrigger("hurt");
        }
    }

    //Trigger Heal animation

    public void Heal()
    {
        for (int i = 0; i < manaStars.Length; i++)
        {
            manaStars[i].GetComponent<Animator>().SetTrigger("heal");
        }
    }
}
