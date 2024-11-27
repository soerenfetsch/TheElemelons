using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana_UI : MonoBehaviour
{
    
    [Header("Mana Management")]
    bool canCast;
    public float maxMana = 5f;
    public float mana;
    public float manaPerRegen = 0.25f;
    public float manaRegenSpeed = 0.2f;
    [Header("UI")]
    public Image[] manaStars;
    public Sprite manaStar0;
    public Sprite manaStar1;
    public Sprite manaStar2;
    public Sprite manaStar3;
    public Sprite manaStar4;
    [Header("Internal Stuff")]
    bool gameRunning = true;
    
    public float current_regenspeed;
    public bool justCasted = false;
        // Start is called before the first frame update
    void Start()
    {
        current_regenspeed = manaRegenSpeed;
        mana = maxMana;
        StartCoroutine(ManaRegen());
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < manaStars.Length; i++)
        {
            //number of heart containers
            if (i<maxMana)
            {
                manaStars[i].enabled = true;
            }else
            {
                manaStars[i].enabled = false;
            }
            //fullness of heart containers
            if (i<Mathf.Floor(mana))
            {
                manaStars[i].GetComponent<Animator>().SetFloat("fullness",1);
                manaStars[i].sprite = manaStar4;
            }else if(i>mana)
            {
                manaStars[i].GetComponent<Animator>().SetFloat("fullness",0);
                manaStars[i].sprite = manaStar0;
            }else
            {
                if(mana%1 == 0){
                    if (mana < maxMana)
                    {
                        manaStars[i].GetComponent<Animator>().SetFloat("fullness",0);
                        manaStars[i].sprite = manaStar0;
                    }else
                    {
                        manaStars[i].GetComponent<Animator>().SetFloat("fullness",1);
                        manaStars[i].sprite = manaStar4;
                    }
                }else if(mana%1 <=0.25)
                {
                    manaStars[i].GetComponent<Animator>().SetFloat("fullness",mana%1);
                    manaStars[i].sprite = manaStar0;
                }else if(mana%1 <=0.5)
                {
                    manaStars[i].GetComponent<Animator>().SetFloat("fullness",mana%1);
                    manaStars[i].sprite = manaStar1;
                }else if(mana%1 <=0.75)
                {
                    manaStars[i].GetComponent<Animator>().SetFloat("fullness",mana%1);
                    manaStars[i].sprite = manaStar2;
                }else if(mana%1 <=1)
                {
                    manaStars[i].GetComponent<Animator>().SetFloat("fullness",mana%1);
                    manaStars[i].sprite = manaStar3;
                }
            }
        }
    }
    
    private IEnumerator ManaRegen(){
        while(gameRunning){
            if (mana<=0.25)
            {
                yield return new WaitForSeconds(current_regenspeed*2);
            }else if (justCasted)
            {
                justCasted = false;
                yield return new WaitForSeconds(current_regenspeed);
            }
            yield return new WaitForSeconds(current_regenspeed);
            mana = Mathf.Clamp(mana+manaPerRegen,0f,maxMana);
        }
    }
    public void SubtractMana(float manaCost)
    {
        mana = Mathf.Clamp(mana-manaCost,0f,maxMana);
        hurt();
    }
    public void hurt()
    {
        for (int i = 0; i < manaStars.Length; i++)
        {
            manaStars[i].GetComponent<Animator>().SetTrigger("hurt");
        }
    }

    public void AddMana(float plus)
    {
        mana *= plus;
    }

    public float GetMana()
    {
        return mana;
    }
    public bool IsZero()
    {
        if (mana <= 0)
        {
            return true;
        }
        return false;
    }
}
