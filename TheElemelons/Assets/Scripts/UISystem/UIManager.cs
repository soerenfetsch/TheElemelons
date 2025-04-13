using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region HUD & In-Game UI Elements

    [Header("Player & HUD")]
    public GameObject playerObject;
    private Health health;
    private Mana mana;
    private HealthBar healthBar;
    private ManaBar manaBar;
    private BuffDebuffIconBar buffDebuffIconBar;
    private PlayerMovement playerMovement;

    [Header("Spell Selection UI")]
    public SpriteRenderer charge1UI;
    public SpriteRenderer charge2UI;
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite earthSprite;
    public Sprite windSprite;
    private Casting castingScript;

    [Header("Aim")]
    public Camera cam;
    public Transform pivot;
    public Transform aim_transform;
    private InputManager inputManager;

    [Header("BuffDebuff Animation")]
    public Animator speedBuffAnimator;

    // Variables to track changes in health and mana for automatic animations.
    private float lastHealth;
    private float lastMana;

    #endregion

    #region Panel Management

    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject startMenuPanel;
    public GameObject settingsMenuPanel;
    public GameObject ingamePanel;

    // We use a dictionary to map state types to panels.
    private Dictionary<Type, GameObject> panelMap;

    #endregion

    private void Awake()
    {
        // Initialize HUD and component references.
        if (playerObject != null)
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
        else
        {
            Logger.LogError("UIManager: PlayerObject is not assigned.");
        }

        // Initialize panel mapping
        panelMap = new Dictionary<Type, GameObject>
        {
            { typeof(MainMenuState), startMenuPanel },
            { typeof(InGameState), ingamePanel },
            { typeof(PauseState), pauseMenuPanel },
            { typeof(SettingsState), settingsMenuPanel }
            // Add additional mappings if needed (e.g., DialogueState -> dialoguePanel)
        };

        // Subscribe to state changes from GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
        }
    }

    #region HUD Update Methods

    private void Update()
    {
        UpdateHealthBar();
        UpdateManaBar();
        UpdateBuffDebuffIcons();
        AutoManaShowHurtHeal();
        AutoHealthShowHurtHeal();
        UpdateSpeedBuffAnimation();

        lastHealth = health != null ? health.GetHealth() : lastHealth;
        lastMana = mana != null ? mana.GetMana() : lastMana;
    }

    private void FixedUpdate()
    {
        UpdateAimingArrow();
    }

    // Health Management
    void UpdateHealthBar()
    {
        if (health != null && healthBar != null)
            healthBar.UpdateHeartFullness(health.GetHealth(), health.GetMaxHealth());
    }

    public void HealthShowHurt()
    {
        if (healthBar != null)
        {
            Logger.Log("HealthShowHurt");
            healthBar.Hurt();
        }
    }

    public void HealthShowHeal()
    {
        if (healthBar != null)
        {
            Logger.Log("HealthShowHeal");
            healthBar.Heal();
        }
    }

    public void AutoHealthShowHurtHeal()
    {
        if (health != null && healthBar != null)
        {
            if (health.GetHealth() < lastHealth)
                healthBar.Hurt();
            else if (health.GetHealth() > lastHealth)
                healthBar.Heal();
        }
    }

    // Mana Management
    void UpdateManaBar()
    {
        if (mana != null && manaBar != null)
            manaBar.UpdateStarFullness(mana.GetMana(), mana.GetMaxMana());
    }

    public void ManaShowHurt()
    {
        if (manaBar != null)
        {
            Logger.Log("ManaShowHurt");
            manaBar.Hurt();
        }
    }

    public void ManaShowHeal()
    {
        if (manaBar != null)
        {
            Logger.Log("ManaShowHeal");
            manaBar.Heal();
        }
    }

    public void AutoManaShowHurtHeal()
    {
        if (mana != null && manaBar != null)
        {
            if (mana.GetMana() < lastMana)
                manaBar.Hurt();
            else if (mana.GetMana() > lastMana)
                manaBar.Heal();
        }
    }

    // Spell Selection
    public void loadChargeSprite(ChargeTypes selectedCharge)
    {
        Sprite spriteToBeLoaded = null;
        switch (selectedCharge)
        {
            case ChargeTypes.fire:
                spriteToBeLoaded = fireSprite;
                break;
            case ChargeTypes.water:
                spriteToBeLoaded = waterSprite;
                break;
            case ChargeTypes.earth:
                spriteToBeLoaded = earthSprite;
                break;
            case ChargeTypes.wind:
                spriteToBeLoaded = windSprite;
                break;
        }

        if (castingScript != null)
        {
            int chargeCount = castingScript.GetCurrentChargesCount();
            if (chargeCount == 0)
            {
                charge1UI.sprite = null;
                charge2UI.sprite = null;
            }
            else if (chargeCount == 1)
            {
                charge1UI.sprite = spriteToBeLoaded;
            }
            else if (chargeCount == 2)
            {
                charge2UI.sprite = spriteToBeLoaded;
            }
        }
    }

    public void ClearChargeSprites()
    {
        if (castingScript != null && castingScript.GetCurrentChargesCount() == 0)
        {
            charge1UI.sprite = null;
            charge2UI.sprite = null;
        }
    }

    // Aiming
    void UpdateAimingArrow()
    {
        if (inputManager != null)
        {
            Vector2 mousePos = inputManager.GetMousePosition();
            float angle = inputManager.GetAimDirection();
            if (aim_transform != null)
                aim_transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // BuffDebuff Icon Management
    private void UpdateBuffDebuffIcons()
    {
        if (buffDebuffIconBar != null)
            buffDebuffIconBar.UpdateIconBar();
    }

    public void AddBuffDebuffIcon(BuffDebuffManager.BuffDebuffType type)
    {
        if (buffDebuffIconBar != null)
            buffDebuffIconBar.AddBuff(type);
    }

    public void RemoveBuffDebuffIcon(BuffDebuffManager.BuffDebuffType type)
    {
        if (buffDebuffIconBar != null)
            buffDebuffIconBar.RemoveBuff(type);
    }

    // Speed Buff Animation
    private void UpdateSpeedBuffAnimation()
    {
        if (playerMovement != null && speedBuffAnimator != null)
        {
            Vector2 movement = playerMovement.GetMovement();
            speedBuffAnimator.SetFloat("Horizontal", movement.x);
            speedBuffAnimator.SetFloat("Vertical", movement.y);
        }
    }

    public void SetSpeedBuffAnimation(bool isBuffed)
    {
        if (speedBuffAnimator != null)
            speedBuffAnimator.SetBool("run", isBuffed);
    }

    #endregion

    #region Panel Management

    /// <summary>
    /// Handles UI panel transitions based on game state.
    /// This method is called whenever the game state changes.
    /// </summary>
    /// <param name="newState">The new game state.</param>
    private void OnGameStateChanged(IGameState newState)
    {
        // Disable all panels first.
        DisableAllPanels();

        // Activate specific panel based on the new state's type.
        Type stateType = newState.GetType();
        if (panelMap.ContainsKey(stateType))
        {
            panelMap[stateType].SetActive(true);
        }
    }

    /// <summary>
    /// Disables all UI panels.
    /// </summary>
    private void DisableAllPanels()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        if (ingamePanel != null) ingamePanel.SetActive(false);
    }

    #endregion
}
