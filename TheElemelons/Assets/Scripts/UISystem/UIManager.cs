using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
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

    [Header("UI Canvas Panels")]
    public GameObject pauseMenuPanel;
    public GameObject startMenuPanel;
    public GameObject settingsMenuPanel;
    public GameObject ingamePanel;
    public GameObject loadPanel;


    /// <summary>
    /// Singleton instance of UIManager.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// Initializes the UIManager by getting references to various components on the player object.
    /// </summary>
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

    /// <summary>
    /// Update is called once per frame.
    /// Updates the health bar, mana bar, buff/debuff icons, and speed buff animation.
    /// </summary>
    void Update()
    {
        if (GameManager.Instance.CurrentGameStatus != GameStatus.INGAME)
        {
            return;
        }
        UpdateHealthBar();
        UpdateManaBar();
        UpdateBuffDebuffIcons();
        AutoManaShowHurtHeal();
        AutoHealthShowHurtHeal();

        UpdateSpeedBuffAnimation();

        lastHealth = health.GetHealth();
        lastMana = mana.GetMana();
    }

    /// <summary>
    /// FixedUpdate is called at a fixed interval.
    /// Updates the aiming arrow based on the player's input.
    /// </summary>
    void FixedUpdate()
    {
        UpdateAimingArrow();
    }

    // Health Management

    /// <summary>
    /// Updates the health bar to reflect the player's current health.
    /// </summary>
    void UpdateHealthBar()
    {
        healthBar.UpdateHeartFullness(health.GetHealth(), health.GetMaxHealth());
    }

    /// <summary>
    /// Shows the hurt animation on the health bar.
    /// </summary>
    public void HealthShowHurt()
    {
        Debug.Log("HealthShowHurt");
        healthBar.Hurt();
    }

    /// <summary>
    /// Shows the heal animation on the health bar.
    /// </summary>
    public void HealthShowHeal()
    {
        Debug.Log("HealthShowHeal");
        healthBar.Heal();
    }

    /// <summary>
    /// Automatically shows the hurt or heal animation on the health bar based on the change in health.
    /// </summary>
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

    // Mana Management

    /// <summary>
    /// Updates the mana bar to reflect the player's current mana.
    /// </summary>
    void UpdateManaBar()
    {
        manaBar.UpdateStarFullness(mana.GetMana(), mana.GetMaxMana());
    }

    /// <summary>
    /// Shows the hurt animation on the mana bar.
    /// </summary>
    public void ManaShowHurt()
    {
        Debug.Log("ManaShowHurt");
        manaBar.Hurt();
    }

    /// <summary>
    /// Shows the heal animation on the mana bar.
    /// </summary>
    public void ManaShowHeal()
    {
        Debug.Log("ManaShowHeal");
        manaBar.Heal();
    }

    /// <summary>
    /// Automatically shows the hurt or heal animation on the mana bar based on the change in mana.
    /// </summary>
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

    // Spell Selection

    /// <summary>
    /// Loads the sprite for the selected charge type into the spell selection UI.
    /// </summary>
    /// <param name="selectedCharge">The selected charge type.</param>
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

    /// <summary>
    /// Clears the charge sprites from the spell selection UI.
    /// </summary>
    public void ClearChargeSprites()
    {
        if (castingScript.GetCurrentChargesCount() == 0)
        {
            charge1UI.sprite = null;
            charge2UI.sprite = null;
        }
    }

    // Aiming

    /// <summary>
    /// Updates the aiming arrow based on the player's input.
    /// </summary>
    void UpdateAimingArrow()
    {
        mousePos = inputManager.GetMousePosition();
        float angle = inputManager.GetAimDirection();
        aim_transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // BuffDebuff Animation Management

    /// <summary>
    /// Updates the speed buff animation based on the player's movement.
    /// </summary>
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
    /// <param name="isBuffed">Whether the speed buff is active.</param>
    public void SetSpeedBuffAnimation(bool isBuffed)
    {
        speedBuffAnimator.SetBool("run", isBuffed);
    }

    // BuffDebuff Icon Management

    /// <summary>
    /// Updates the buff/debuff icons in the UI.
    /// </summary>
    private void UpdateBuffDebuffIcons()
    {
        buffDebuffIconBar.UpdateIconBar();
    }

    /// <summary>
    /// Adds a buff/debuff icon to the UI.
    /// </summary>
    /// <param name="type">The type of buff/debuff to add.</param>
    public void AddBuffDebuffIcon(BuffDebuffManager.BuffDebuffType type)
    {
        buffDebuffIconBar.AddBuff(type);
    }

    /// <summary>
    /// Removes a buff/debuff icon from the UI.
    /// </summary>
    /// <param name="type">The type of buff/debuff to remove.</param>
    public void RemoveBuffDebuffIcon(BuffDebuffManager.BuffDebuffType type)
    {
        buffDebuffIconBar.RemoveBuff(type);
    }

    private void OnEnable()
    {
        GameManager.OnGameStatusChanged += HandleGameStatusChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStatusChanged -= HandleGameStatusChanged;
    }

    private void HandleGameStatusChanged(GameStatus newStatus)
    {
        switch (newStatus)
        {
            case GameStatus.DEATH_SCREEN:
                EnablePanel("DeathScreen");
                break;
            case GameStatus.LOADING_SCREEN:
                EnablePanel("LoadingScreen");
                break;
            case GameStatus.PAUSE_MENU:
                EnablePanel("PauseMenu");
                break;
            case GameStatus.SETTINGS_MENU:
                EnablePanel("SettingsMenu");
                break;
            case GameStatus.INGAME:
                EnablePanel("Ingame");
                break;
            case GameStatus.MAIN_MENU:
                EnablePanel("StartMenu");
                break;
            case GameStatus.DIALOGUE:
                // Handle dialogue UI (e.g., show dialogue panel)
                break;
        }
    }

    /// <summary>
    /// Enables the pause menu panel. Disables all other panels.
    /// </summary>
    [ContextMenu("Enable Pause Menu")]
    public void EnablePauseMenu() => EnablePanel("PauseMenu");

    /// <summary>
    /// Enables the start menu panel. Disables all other panels.
    /// </summary>
    [ContextMenu("Enable Start Menu")]
    public void EnableStartMenu() => EnablePanel("StartMenu");

    /// <summary>
    /// Enables the settings menu panel. Disables all other panels.
    /// </summary> 
    [ContextMenu("Enable Settings Menu")]
    public void EnableSettingsMenu() => EnablePanel("SettingsMenu");

    /// <summary>
    /// Enables the ingame panel. Disables all other panels.
    /// </summary> 
    [ContextMenu("Enable Ingame Panel")]
    public void EnableIngamePanel() => EnablePanel("Ingame");

    /// <summary>
    /// Enables the load panel. Disables all other panels.
    /// </summary>
    [ContextMenu("Enable Load Panel")]
    public void EnableLoadPanel() => EnablePanel("LoadPanel");

    /// <summary>
    /// Enables the specified panel and disables all other panels.
    /// Possible names are "PauseMenu", "StartMenu", "SettingsMenu", and "Ingame".
    /// </summary>
    /// <param name="panelName"></param>
    private void EnablePanel(string panelName)
    {
        GameObject panel = null;
        if (panelName == "PauseMenu")
        {
            panel = pauseMenuPanel;
        }
        else if (panelName == "StartMenu")
        {
            panel = startMenuPanel;
        }
        else if (panelName == "SettingsMenu")
        {
            panel = settingsMenuPanel;
        }
        else if (panelName == "Ingame")
        {
            panel = ingamePanel;
        }
        else if (panelName == "LoadPanel")
        {
            panel = loadPanel;
        }

        if (panel != null)
        {
            DisableAllPanels();
            panel?.SetActive(true);
        }
        else
        {
            Debug.LogError("Panel not found: " + panelName);
        }
    }

    private void DisableAllPanels()
    {
        pauseMenuPanel?.SetActive(false);
        startMenuPanel?.SetActive(false);
        settingsMenuPanel?.SetActive(false);
        ingamePanel?.SetActive(false);
    }

    public void ToggleContinueGameButton(bool isEnabled)
    {
        if (isEnabled)
        {
            startMenuPanel?.GetComponent<StartMenuUI>()?.EnableContinueGame();
        }
        else
        {
            startMenuPanel?.GetComponent<StartMenuUI>()?.DisableContinueGame();
        }
        
    }
}