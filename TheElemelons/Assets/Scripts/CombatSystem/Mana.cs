using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{

    [Header("Mana Management")]
    bool canCast;
    public float maxMana = 5f;
    public float mana;
    public float manaPerRegen = 0.25f;
    public float manaRegenTime = 0.2f;
    public bool gameRunning = true;

    public float current_regenTime;
    public bool justCasted = false;
    // Start is called before the first frame update
    void Start()
    {
        current_regenTime = manaRegenTime;
        mana = maxMana;
        StartCoroutine(ManaRegen());
    }
    void Update()
    {
        
    }
   public IEnumerator ManaRegen()
    {
        while (gameRunning)
        {
            if (mana <= 0.25)
            {
                yield return new WaitForSeconds(current_regenTime * 2);
            }
            else if (justCasted)
            {
                justCasted = false;
                yield return new WaitForSeconds(current_regenTime);
            }
            yield return new WaitForSeconds(current_regenTime);
            mana = Mathf.Clamp(mana + manaPerRegen, 0f, maxMana);
        }
    }
    public void SubtractMana(float manaCost)
    {
        mana = Mathf.Clamp(mana - manaCost, 0f, maxMana);
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
