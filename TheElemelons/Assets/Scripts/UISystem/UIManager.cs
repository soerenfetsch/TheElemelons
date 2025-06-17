using UnityEngine;

/// <summary>
/// Central UI manager for all in-game and menu UI elements.
/// Handles health/mana bars, spell selection UI, buff/debuff icons, aiming, and panel switching.
/// Provides methods for updating UI in response to player actions, buffs, and game state changes.
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of UIManager.
    /// </summary>
    public static UIManager Instance { get; private set; }

    /// <summary>
    /// Reference to the player GameObject.
    /// </summary>
    public GameObject playerObject;

    // Player component references
    Health health;
    Mana mana;
    HealthBar healthBar;
    ManaBar manaBar;
    BuffDebuffIconBar buffDebuffIconBar;
    PlayerMovement playerMovement;
    InputManager inputManager;

    [Header("Aim")]
    /// <summary>
    /// Current mouse position in world coordinates.
    /// </summary>
    public Vector2 mousePos;
    /// <summary>
    /// Reference to the main camera.
    /// </summary>
    public Camera cam;
    /// <summary>
    /// Transform used as the pivot for aiming.
    /// </summary>
    public Transform pivot;
    /// <summary>
    /// Transform for the aiming arrow.
    /// </summary>
    public Transform aim_transform;

    [Header("Spell Selection UI")]
    /// <summary>
    /// SpriteRenderer for the first charge slot.
    /// </summary>
    public SpriteRenderer charge1UI;
    /// <summary>
    /// SpriteRenderer for the second charge slot.
    /// </summary>
    public SpriteRenderer charge2UI;
    /// <summary>
    /// Sprite for fire charge.
    /// </summary>
    public Sprite fireSprite;
    /// <summary>
    /// Sprite for water charge.
    /// </summary>
    public Sprite waterSprite;
    /// <summary>
    /// Sprite for earth charge.
    /// </summary>
    public Sprite earthSprite;
    /// <summary>
    /// Sprite for wind charge.
    /// </summary>
    public Sprite windSprite;
    // Reference to the player's Casting script.
    Casting castingScript;

    [Header("ManaHealthBar Management")]
    float lastMana;
    float lastHealth;

    [Header("BuffDebuff Animation Management")]
    /// <summary>
    /// Animator for speed buff visual effect.
    /// </summary>
    public Animator speedBuffAnimator;
    /// <summary>
    /// Prefab for the confused (question mark) effect.
    /// </summary>
    public GameObject questionMark;
    /// <summary>
    /// Prefab for the ice crystal (freeze) effect.
    /// </summary>
    public GameObject iceCrystal;
    /// <summary>
    /// Prefab for the earth skin buff effect.
    /// </summary>
    public GameObject earthSkinEffect;
    /// <summary>
    /// Prefab for the petrified player effect.
    /// </summary>
    public GameObject petrifiedPlayerEffect;

    [Header("UI Canvas Panels")]
    /// <summary>
    /// Pause menu panel GameObject.
    /// </summary>
    public GameObject pauseMenuPanel;
    /// <summary>
    /// Start menu panel GameObject.
    /// </summary>
    public GameObject startMenuPanel;
    /// <summary>
    /// Settings menu panel GameObject.
    /// </summary>
    public GameObject settingsMenuPanel;
    /// <summary>
    /// In-game HUD panel GameObject.
    /// </summary>
    public GameObject ingamePanel;
    /// <summary>
    /// Death screen panel GameObject.
    /// </summary>
    public GameObject deathScreenPanel;
    /// <summary>
    /// Load panel GameObject.
    /// </summary>
    public GameObject loadPanel;

    [Header("Bossbar")]
    /// <summary>
    /// Boss health bar GameObject.
    /// </summary>
    public GameObject bossBar;

    /// <summary>
    /// Unity Awake method. Sets up the singleton instance and persists across scenes.
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
    /// Unity Start method. Initializes references to player components and UI elements.
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
        bossBar = ingamePanel?.transform.Find("BossBar")?.gameObject;
        ToggleBossBar(false, null);
    }

    /// <summary>
    /// Unity Update method. Updates health/mana bars, buff/debuff icons, and speed buff animation.
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
    /// Unity FixedUpdate method. Updates the aiming arrow based on player input.
    /// </summary>
    void FixedUpdate()
    {
        if (GameManager.Instance.CurrentGameStatus != GameStatus.INGAME)
        {
            return;
        }
        UpdateAimingArrow();
    }

    // Updates the health bar UI to match the player's current health.
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

    // Updates the mana bar UI to match the player's current mana.
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

    // Updates the aiming arrow to match the player's current aim direction.
    void UpdateAimingArrow()
    {
        mousePos = inputManager.GetMousePosition();
        float angle = inputManager.GetAimDirection();
        aim_transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Updates the speed buff animation based on the player's movement.
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

    /// <summary>
    /// Instantiate question mark as visual effect when confused.
    /// </summary>
    /// <param name="isBuffed">Whether the confused effect is active.</param>
    /// <param name="target">The target GameObject.</param>
    public void SetConfusedEffect(bool isBuffed, GameObject target)
    {
        if (isBuffed)
        {
            Vector3 pos = target.transform.position;
            GameObject questionMarkEffect = Instantiate(questionMark, pos, Quaternion.Euler(0, 0, 0));
            questionMarkEffect.transform.parent = target.transform;
        }
        else
        {
            Destroy(target.transform.Find("QuestionMark(Clone)").gameObject);
        }
    }

    /// <summary>
    /// Initialize ice crystal effect when frozen.
    /// </summary>
    /// <param name="isBuffed">Whether the freeze effect is active.</param>
    /// <param name="target">The target GameObject.</param>
    public void SetFreezeEffect(bool isBuffed, GameObject target)
    {
        if (isBuffed)
        {
            Vector3 pos = target.transform.position;
            GameObject freezeEffect = Instantiate(iceCrystal, pos, Quaternion.Euler(0, 0, 0));
            freezeEffect.GetComponent<IceCrystal>().SetVictim(target);
        }
        else
        {
            Destroy(target.transform.Find("IceCrystalEffect(Clone)").gameObject);
        }
    }

    /// <summary>
    /// Initialize earth skin effect when buffed.
    /// </summary>
    /// <param name="isBuffed">Whether the earth skin effect is active.</param>
    /// <param name="target">The target GameObject.</param>
    public void SetEarthSkinEffect(bool isBuffed, GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            if (isBuffed)
            {
                Instantiate(earthSkinEffect, playerObject.transform.position, Quaternion.Euler(0, 0, 0));
            }
            else
            {
                StartCoroutine(playerObject.transform.Find("EarthSkinEffect(Clone)").GetComponent<EarthSkinBuffEffect>().Expire());
            }
        }
    }

    /// <summary>
    /// Initialize petrified effect when buffed.
    /// </summary>
    /// <param name="isBuffed">Whether the petrified effect is active.</param>
    /// <param name="target">The target GameObject.</param>
    public void SetPetrifiedEffect(bool isBuffed, GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            if (isBuffed)
            {
                Instantiate(petrifiedPlayerEffect, playerObject.transform.position, Quaternion.Euler(0, 0, 0));
            }
            else
            {
                StartCoroutine(playerObject.transform.Find("StonedPlayerEffect(Clone)").GetComponent<Petrified_Player>().Expire());
            }
        }
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

    /// <summary>
    /// Registers for game status change events.
    /// </summary>
    private void OnEnable()
    {
        GameManager.OnGameStatusChanged += HandleGameStatusChanged;
    }

    /// <summary>
    /// Unregisters from game status change events.
    /// </summary>
    private void OnDisable()
    {
        GameManager.OnGameStatusChanged -= HandleGameStatusChanged;
    }

    /// <summary>
    /// Handles switching UI panels based on the current game status.
    /// </summary>
    /// <param name="newStatus">The new game status.</param>
    private void HandleGameStatusChanged(GameStatus newStatus)
    {
        Debug.Log($"UIManager: Game status changed to {newStatus}");
        switch (newStatus)
        {
            case GameStatus.DEATH_SCREEN:
                EnablePanel("DeathScreen");
                break;
            case GameStatus.LOADING_SCREEN:
                EnablePanel("LoadPanel");
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
    /// Enables the death screen panel. Disables all other panels.
    /// </summary>
    [ContextMenu("Enable Death Menu")]
    public void EnableDeathScreen() => EnablePanel("DeathScreen");

    /// <summary>
    /// Enables the in-game HUD panel. Disables all other panels.
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
    /// Possible names are "PauseMenu", "StartMenu", "SettingsMenu","DeathScreen", and "Ingame".
    /// </summary>
    /// <param name="panelName">The name of the panel to enable.</param>
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
        else if (panelName == "DeathScreen")
        {
            panel = deathScreenPanel;
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

    // Disables all UI panels.
    private void DisableAllPanels()
    {
        pauseMenuPanel?.SetActive(false);
        startMenuPanel?.SetActive(false);
        settingsMenuPanel?.SetActive(false);
        ingamePanel?.SetActive(false);
        deathScreenPanel?.SetActive(false);
        loadPanel?.SetActive(false);
    }

    /// <summary>
    /// Toggles the visibility of the continue game button in the start menu.
    /// </summary>
    /// <param name="isEnabled">True to enable the button, false to disable it.</param>
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

    /// <summary>
    /// Toggles the visibility of the boss bar and locks its health content to the boss's health.
    /// </summary>
    /// <param name="isEnabled">True to show the boss bar, false to hide it.</param>
    /// <param name="bossHealth">The boss's health component.</param>
    public void ToggleBossBar(bool isEnabled, Health bossHealth)
    {
        if (bossBar == null)
        {
            Debug.LogError("Boss bar not found.");
            return;
        }
        if (isEnabled)
        {
            bossBar.SetActive(true);
            bossBar.GetComponent<BossHealthbar>().bossHealth = bossHealth;
        }
        else
        {
            bossBar.GetComponent<BossHealthbar>().bossHealth = null;
            bossBar.SetActive(false);
        }
    }
}