using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana_UI : MonoBehaviour
{
    float mana;
    float maxMana;
    public Image[] manaStars;
    public Sprite manaStar0;
    public Sprite manaStar1;
    public Sprite manaStar2;
    public Sprite manaStar3;
    public Sprite manaStar4;
        // Start is called before the first frame update
    void Start()
    {
        maxMana = this.GetComponent<Casting>().maxMana;
        mana = this.GetComponent<Casting>().mana;
    }

    // Update is called once per frame
    void Update()
    {
        mana = this.GetComponent<Casting>().mana;
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
    public void hurt(){
        for (int i = 0; i < manaStars.Length; i++)
        {
            manaStars[i].GetComponent<Animator>().SetTrigger("hurt");
        }
    }

}
