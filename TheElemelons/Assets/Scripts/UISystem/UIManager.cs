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
    BuffDebuffIconBar buffDebuffIconBar;
    PlayerMovement playerMovement;

    InputManager inputManager;
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
    [Header("BuffDebuff Animation Management")]
    public Animator speedBuffAnimator;

    // Start is called before the first frame update
    void Start()
    {
        health = playerObject.GetComponent<Health>();
        healthBar = playerObject.GetComponent<HealthBar>();
        mana = playerObject.GetComponent<Mana>();
        manaBar = playerObject.GetComponent<ManaBar>();
        buffDebuffIconBar = playerObject.GetComponent<BuffDebuffIconBar>();
        castingScript = playerObject.GetComponent<Casting>();
        inputManager = playerObject.GetComponent<InputManager>();
        playerMovement = playerObject.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
        UpdateManaBar();
        UpdateBuffDebuffIcons();
        AutoManaShowHurtHeal();
        AutoHealthShowHurtHeal();

        UpdateSpeedBuffAnimation();

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
        Debug.Log("HealthShowHurt");
        healthBar.Hurt();
    }
    public void HealthShowHeal()
    {
        Debug.Log("HealthShowHeal");
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
        Debug.Log("ManaShowHurt");
        manaBar.Hurt();
    }
    public void ManaShowHeal()
    {
        Debug.Log("ManaShowHeal");
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
        mousePos = inputManager.GetMousePosition();
        float angle = inputManager.GetAimDirection();
        aim_transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    //BuffDebuff Animation Management
    private void UpdateSpeedBuffAnimation()
    {
        Vector2 movement = playerMovement.GetMovement();
        speedBuffAnimator.SetFloat("Horizontal", movement.x);
        speedBuffAnimator.SetFloat("Vertical", movement.y);
    }

    /// <summary>
    /// Start or stop the speed buff animation.
    /// Should be called at the start and at the end of the speed buff effect.
    /// </summary>
    /// <param name="isBuffed"></param>
    public void SetSpeedBuffAnimation(bool isBuffed)
    {
        speedBuffAnimator.SetBool("run", isBuffed);
    }

    // BuffDebuff Icon Management
    private void UpdateBuffDebuffIcons()
    {
        buffDebuffIconBar.UpdateIconBar();
    }

    public void AddBuffDebuffIcon(BuffDebuffManager.BuffDebuffType type)
    {
        buffDebuffIconBar.AddBuff(type);
    }

    public void RemoveBuffDebuffIcon(BuffDebuffManager.BuffDebuffType type)
    {
        buffDebuffIconBar.RemoveBuff(type);
    }

}
