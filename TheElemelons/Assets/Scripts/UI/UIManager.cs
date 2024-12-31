using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject playerObject;
    Health health;
    Mana mana;
    HealthBar healthBar;
    ManaBar manaBar;
    [Header("Aim")]
    public Vector2 mousePos;
    public Camera cam;
    public Transform pivot;
    public Transform aim_transform;
    [Header("Spell Selection UI")]
    public SpriteRenderer charge1UI;
    public SpriteRenderer charge2UI;
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite earthSprite;
    public Sprite windSprite;
    Casting castingScript;
    [Header("ManaHealthBar Management")]
    float lastMana;
    float lastHealth;

    // Start is called before the first frame update
    void Start()
    {
        health = playerObject.GetComponent<Health>();
        healthBar = playerObject.GetComponent<HealthBar>();
        mana = playerObject.GetComponent<Mana>();
        manaBar = playerObject.GetComponent<ManaBar>();
        castingScript = playerObject.GetComponent<Casting>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
        UpdateManaBar();
        AutoManaShowHurtHeal();
        AutoHealthShowHurtHeal();

        lastHealth = health.GetHealth();
        lastMana = mana.GetMana();
    }
    void FixedUpdate()
    {
        UpdateAimingArrow();
    }

    //Health STuff
    void UpdateHealthBar()
    {
        healthBar.UpdateHeartFullness(health.GetHealth(), health.GetMaxHealth());
    }
    public void HealthShowHurt()
    {
        healthBar.Hurt();
    }
    public void HealthShowHeal()
    {
        healthBar.Heal();
    }
    public void AutoHealthShowHurtHeal()
    {
        if (health.GetHealth() < lastHealth)
        {
            healthBar.Hurt();
        }
        else if (health.GetHealth() > lastHealth)
        {
            healthBar.Heal();
        }
    }

    //Mana STuff
    void UpdateManaBar()
    {
        manaBar.UpdateStarFullness(mana.GetMana(), mana.GetMaxMana());
    }
    public void ManaShowHurt()
    {
        manaBar.Hurt();
    }
    public void ManaShowHeal()
    {
        manaBar.Heal();
    }
    public void AutoManaShowHurtHeal()
    {
        if (mana.GetMana() < lastMana)
        {
            manaBar.Hurt();
        }
        else if (mana.GetMana() > lastMana)
        {
            manaBar.Heal();
        }
    }

    //Spell Selection
    public void loadChargeSprite(ChargeTypes selectedCharge)
    {
        Sprite spriteToBeLoaded = null;
        if (selectedCharge == ChargeTypes.fire)
        {
            spriteToBeLoaded = fireSprite;
        }
        if (selectedCharge == ChargeTypes.water)
        {
            spriteToBeLoaded = waterSprite;
        }
        if (selectedCharge == ChargeTypes.earth)
        {
            spriteToBeLoaded = earthSprite;
        }
        if (selectedCharge == ChargeTypes.wind)
        {
            spriteToBeLoaded = windSprite;
        }

        if (castingScript.GetCurrentChargesCount() == 0)
        {
            charge1UI.sprite = null;
            charge2UI.sprite = null;
        }
        else if (castingScript.GetCurrentChargesCount() == 1)
        {
            charge1UI.sprite = spriteToBeLoaded;
        }
        else if (castingScript.GetCurrentChargesCount() == 2)
        {
            charge2UI.sprite = spriteToBeLoaded;
        }

    }

    public void ClearChargeSprites()
    {
        if (castingScript.GetCurrentChargesCount() == 0)
        {
            charge1UI.sprite = null;
            charge2UI.sprite = null;
        }
    }

    //AIM
    void UpdateAimingArrow()
    {
        Vector2 aim_dir = mousePos - new Vector2(pivot.position.x, pivot.position.y);
        float angle = Mathf.Atan2(aim_dir.y, aim_dir.x) * Mathf.Rad2Deg + 90f;
        aim_transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetMousePos(Vector2 newMousepos)
    {
        mousePos = newMousepos;
    }
}
