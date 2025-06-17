using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main controller for executing progression-stage-specific content, scene transitions,
/// and global game logic. Handles scene loading, player state resets, and event-driven
/// progression for dungeons, bosses, and hub.
/// </summary>
public class MainGame : MonoBehaviour
{
    /// <summary>
    /// Prefab for the collectable earth elemelon (used in boss rooms).
    /// </summary>
    public GameObject earthElemelonCollectablePrefab;
    /// <summary>
    /// Prefab for the displayable earth elemelon (used in the castle hub).
    /// </summary>
    public GameObject earthElemelonDisplayablePrefab;

    /// <summary>
    /// Singleton instance of MainGame.
    /// </summary>
    public static MainGame Instance { get; private set; }

    /// <summary>
    /// List of king's introductory dialogues.
    /// </summary>
    public List<DialogueData> kingIntroDialoguesList;
    /// <summary>
    /// List of king's repetitive dialogues.
    /// </summary>
    public List<DialogueData> kingRepetitiveDialoguesList;
    /// <summary>
    /// List of king's end dialogues.
    /// </summary>
    public List<DialogueData> kingEndDialoguesList;

    // Tracks if the king's dialogue is currently active (for progression).
    private static bool kingDialogueActive = false;

    /// <summary>
    /// Unity Awake method. Sets up the singleton instance and persists across scenes.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    /// <summary>
    /// Starts the specified game progression stage, loading the correct scene and initializing content.
    /// </summary>
    /// <param name="stage">The progression stage to start.</param>
    /// <param name="isStartScene">If true, treat as a start scene (e.g., main menu).</param>
    public void StartGameStage(GameProgressionStage stage, bool isStartScene = false)
    {
        Debug.Log("Starting game stage: " + stage);
        StartCoroutine(StartGameStageAsync(stage, isStartScene));
    }

    // Coroutine to load the scene and execute the logic for the given progression stage.
    private IEnumerator StartGameStageAsync(GameProgressionStage stage, bool isStartScene = false)
    {
        // Load scene of the game stage
        string sceneToLoad = GetSceneForStage(stage);
        yield return StartCoroutine(LoadScene(sceneToLoad));

        ResetPlayerCombatStatus();

        GameStatus sceneStatus = isStartScene ? GameStatus.MAIN_MENU : GameStatus.INGAME;
        GameManager.Instance.ChangeGameStatus(sceneStatus);

        // Scene is now loaded; handle stage-specific logic
        switch (stage)
        {
            case GameProgressionStage.INVALID:
                HandleInvalidStage();
                break;
            case GameProgressionStage.CASTLE_INTRODUCTION:
                HandleCastleIntroStage();
                break;
            case GameProgressionStage.CASTLE_EARLY:
                HandleCastleEarlyStage();
                break;
            case GameProgressionStage.EARTH_EARLY:
                HandleEarthEarlyStage();
                break;
            case GameProgressionStage.EARTH_EARLY_BOSS:
                HandleEarthEarlyBossStage();
                break;
            case GameProgressionStage.CASTLE_END:
                HandleCastleEndStage();
                break;
            case GameProgressionStage.CASTLE_LATE:
                HandleCastleLateStage();
                break;
            case GameProgressionStage.EARTH_LATE:
                HandleEarthLateStage();
                break;
            case GameProgressionStage.EARTH_LATE_BOSS:
                HandleEarthLateBossStage();
                break;
            default:
                Debug.LogError("Unknown game stage: " + stage);
                break;
        }
        yield return null;
    }

    // Handles logic for the INVALID stage (main menu).
    private void HandleInvalidStage()
    {
        Debug.Log("Main Menu Stage started.");
    }

    // Handles logic for the castle introduction stage (e.g., cutscene).
    private void HandleCastleIntroStage()
    {
        // TODO: Play the castle intro cutscene
    }

    // Handles logic for the early castle hub stage.
    private void HandleCastleEarlyStage()
    {
        // Add event to the earth portal to start the earth early stage
        Portal portal = FindAnyObjectByType<Portal>();
        if (portal != null)
        {
            portal.OnPortalEnter += () =>
            {
                GameManager.Instance.ChangeProgressionStage(GameProgressionStage.EARTH_EARLY);
                portal.OnPortalEnter -= portal.OnPortalEnter;
            };
        }
        else
        {
            Debug.LogError("EarthPortal GameObject not found in the scene.");
        }
    }

    // Handles logic for the early earth dungeon stage.
    private void HandleEarthEarlyStage()
    {
        // Allow the boss room to be accessed when all monsters are defeated
        Portal endPortal = FindObjectOfType<Portal>();
        MonsterManager monsterManager = FindObjectOfType<MonsterManager>();
        if (endPortal == null)
        {
            Debug.LogError("EndPortal GameObject not found in the scene.");
            return;
        }
        if (monsterManager == null)
        {
            Debug.LogError("MonsterManager GameObject not found in the scene.");
            return;
        }
        if (endPortal.OnPortalEnter == null)
        {
            endPortal.OnPortalEnter += () =>
            {
                // First check that all the monsters in the room are defeated
                if (!monsterManager.IsEndPortalEnabled())
                {
                    Debug.Log("End portal is not enabled yet.");
                    return;
                }
                GameManager.Instance.ChangeProgressionStage(GameProgressionStage.EARTH_EARLY_BOSS);
                endPortal.OnPortalEnter -= endPortal.OnPortalEnter;
            };
        }
        else
        {
            Debug.LogError("EndPortal GameObject not found in the scene.");
        }
    }

    // Handles logic for the earth dungeon boss room stage.
    private void HandleEarthEarlyBossStage()
    {
        // Teleport the player to the start location of the boss room
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(-3, -3, 0);
        }
        else
        {
            Debug.LogError("Player GameObject not found in the scene.");
        }

        // If the boss is defeated, drop the earth elemelon in the boss room
        GameObject witchObject = GameObject.Find("Witch");
        Witch witch = witchObject?.GetComponent<Witch>();
        if (witch != null)
        {
            witch.OnWitchDeath += () =>
            {
                // Spawn the earth elemelon
                GameObject earthElemelon = Instantiate(earthElemelonCollectablePrefab, witchObject.transform.position, Quaternion.identity);
                // If the earth elemelon is picked up, the progression stage is changed
                SetupEarthElemelonPickup(earthElemelon);
            };
        }
        else
        {
            Debug.LogError("Witch GameObject not found in the scene.");
        }
    }

    // Sets up the event for collecting the earth elemelon to trigger progression.
    private static void SetupEarthElemelonPickup(GameObject earthElemelon)
    {
        CollectableEarthElemelon collectableEarthElemelon = earthElemelon.GetComponent<CollectableEarthElemelon>();
        if (collectableEarthElemelon != null)
        {
            collectableEarthElemelon.OnEarthElemelonCollected += () =>
            {
                GameManager.Instance.ChangeProgressionStage(GameProgressionStage.CASTLE_END);
                collectableEarthElemelon.OnEarthElemelonCollected -= collectableEarthElemelon.OnEarthElemelonCollected;
            };
        }
        else
        {
            Debug.LogError("CollectableEarthElemelon component not found on the earth elemelon prefab.");
        }
    }

    // Handles logic for the castle end stage (after returning from earth dungeon).
    private void HandleCastleEndStage()
    {
        Action<DialogueData> onKingDialogueStarted = null;
        Action onKingDialogueEnded = null;

        onKingDialogueStarted = (DialogueData dialogueData) =>
        {
            if (dialogueData.speakerName == "King")
            {
                Debug.Log("King dialogue started");
                kingDialogueActive = true;
            }
        };

        onKingDialogueEnded = () =>
        {
            if (kingDialogueActive)
            {
                Debug.Log("King dialogue ended");
                kingDialogueActive = false;

                // Unsubscribe both handlers
                DialogueManager.OnDialogueStart -= onKingDialogueStarted;
                DialogueManager.OnDialogueEnded -= onKingDialogueEnded;

                GameManager.Instance.ChangeProgressionStage(GameProgressionStage.CASTLE_LATE);
            }
        };

        // Subscribe both
        DialogueManager.OnDialogueStart += onKingDialogueStarted;
        DialogueManager.OnDialogueEnded += onKingDialogueEnded;
    }

    // Handles logic for the late castle hub stage.
    private void HandleCastleLateStage()
    {
        // Add the event to the earth portal to start the earth late stage
        Portal portal = FindAnyObjectByType<Portal>();
        if (portal != null)
        {
            portal.OnPortalEnter += () =>
            {
                GameManager.Instance.ChangeProgressionStage(GameProgressionStage.EARTH_LATE);
                portal.OnPortalEnter -= portal.OnPortalEnter;
            };
        }
        else
        {
            Debug.LogError("EarthPortal GameObject not found in the scene.");
        }
    }

    // Handles logic for the late earth dungeon stage.
    private void HandleEarthLateStage()
    {
        // Allow the boss room to be accessed when all monsters are defeated
        Portal endPortal = FindObjectOfType<Portal>();
        MonsterManager monsterManager = FindObjectOfType<MonsterManager>();
        if (endPortal == null)
        {
            Debug.LogError("EndPortal GameObject not found in the scene.");
            return;
        }
        if (monsterManager == null)
        {
            Debug.LogError("MonsterManager GameObject not found in the scene.");
            return;
        }
        if (endPortal.OnPortalEnter == null)
        {
            endPortal.OnPortalEnter += () =>
            {
                // First check that all the monsters in the room are defeated
                if (!monsterManager.IsEndPortalEnabled())
                {
                    Debug.Log("End portal is not enabled yet.");
                    return;
                }
                GameManager.Instance.ChangeProgressionStage(GameProgressionStage.EARTH_LATE_BOSS);
                endPortal.OnPortalEnter -= endPortal.OnPortalEnter;
            };
        }
        else
        {
            Debug.LogError("EndPortal GameObject not found in the scene.");
        }
    }

    // Handles logic for the late earth dungeon boss room stage.
    private void HandleEarthLateBossStage()
    {
        // Teleport the player to the start location of the boss room
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(-3, -3, 0);
        }
        else
        {
            Debug.LogError("Player GameObject not found in the scene.");
        }

        // If the boss is defeated, progress to the next stage after a delay
        GameObject witchObject = GameObject.Find("Witch");
        Witch witch = witchObject?.GetComponent<Witch>();
        if (witch != null)
        {
            witch.OnWitchDeath += () =>
            {
                // Change to castle late stage after some delay
                StartCoroutine(ChangeProgressionStageAfterDelay(GameProgressionStage.CASTLE_LATE, 2f));
            };
        }
        else
        {
            Debug.LogError("Witch GameObject not found in the scene.");
        }
    }

    // Loads the specified scene asynchronously and resets player position if needed.
    private static IEnumerator LoadScene(string sceneToLoad)
    {
        if (!string.IsNullOrEmpty(sceneToLoad) && SceneManager.GetActiveScene().name != sceneToLoad
        || (GameManager.Instance.PreviousProgressionStage == GameProgressionStage.CASTLE_INTRODUCTION &&
        GameManager.Instance.CurrentProgressionStage == GameProgressionStage.CASTLE_INTRODUCTION && sceneToLoad == "Castle_Hub_Scene"))
        {
            GameManager.Instance.ChangeGameStatus(GameStatus.LOADING_SCREEN);
            Debug.Log("Waiting for loading screen to be displayed...");
            // Wait until loading screen is displayed
            yield return null;
            yield return new WaitForEndOfFrame();

            Debug.Log("Loading scene: " + sceneToLoad);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            Debug.Log("Scene loaded: " + sceneToLoad);
            if (sceneToLoad == "Castle_Hub_Scene")
            {
                ResetPlayerPosition();
            }
            yield return new WaitForEndOfFrame();
        }

        // Also reset the player position if Castle Hub scene already loaded or if the previous progression stage was invalid
        if (GameManager.Instance.PreviousProgressionStage == GameProgressionStage.INVALID
            && sceneToLoad == "Castle_Hub_Scene")
        {
            ResetPlayerPosition();
        }
        yield return null;
    }

    // Resets the player's position to the initial spawn point in the castle hub.
    private static void ResetPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(8f, -5f, 0f); // Reset to initial position
        }
        else
        {
            Debug.LogError("Player GameObject not found in the scene.");
        }
    }

    // Returns the scene name for a given progression stage.
    private static string GetSceneForStage(GameProgressionStage stage)
    {
        switch (stage)
        {
            case GameProgressionStage.INVALID:
                return "MainMenuScene";
            case GameProgressionStage.CASTLE_INTRODUCTION:
                return "Castle_Hub_Scene";
            case GameProgressionStage.CASTLE_EARLY:
                return "Castle_Hub_Scene";
            case GameProgressionStage.CASTLE_END:
                return "Castle_Hub_Scene";
            case GameProgressionStage.CASTLE_LATE:
                return "Castle_Hub_Scene";
            case GameProgressionStage.EARTH_EARLY:
                return "Earth_Dungeon_Scene";
            case GameProgressionStage.EARTH_LATE:
                return "Earth_Dungeon_Scene";
            case GameProgressionStage.EARTH_EARLY_BOSS:
                return "BossRoom_Earth";
            case GameProgressionStage.EARTH_LATE_BOSS:
                return "BossRoom_Earth";
            default:
                return null;
        }
    }

    // Resets the player's health, mana, and clears buffs/debuffs.
    private static void ResetPlayerCombatStatus()
    {
        // Find player in the scene and reset it
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>(); // Reset health
            playerHealth.SetHealth(playerHealth.GetMaxHealth());
            Mana playerMana = player.GetComponent<Mana>(); // Reset mana
            playerMana.SetMana(playerMana.GetMaxMana());
            BuffDebuffManager.Instance.ClearEffects(player); // Clear buffs and debuffs
        }
        else
        {
            Debug.LogError("Player GameObject not found in the scene.");
        }
    }

    // Coroutine to change the progression stage after a delay (e.g., after boss death).
    private IEnumerator ChangeProgressionStageAfterDelay(GameProgressionStage stage, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.ChangeProgressionStage(stage);
    }

    /// <summary>
    /// Spawns the earth elemelon displayable prefab in the scene (e.g., in the king's room).
    /// </summary>
    private void SpawnEarthElemelon()
    {
        // Spawn the earth elemelon collectable in the scene
        if (earthElemelonDisplayablePrefab != null)
        {
            Instantiate(earthElemelonDisplayablePrefab, new Vector3(4.5f, 8.5f, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogError("EarthElemelonCollectablePrefab is not assigned.");
        }
    }
}
