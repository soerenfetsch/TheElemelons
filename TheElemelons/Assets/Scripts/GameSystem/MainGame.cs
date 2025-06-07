using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the main game flow, including stage transitions, scene loading, and special event handling.
/// Handles player and boss events, dialogue triggers, and manages the progression through the game's main stages.
/// </summary>
public class MainGame : MonoBehaviour
{
    /// <summary>
    /// Prefab for the collectable earth elemelon.
    /// </summary>
    public GameObject earthElemelonCollectablePrefab;

    /// <summary>
    /// Prefab for the displayable earth elemelon.
    /// </summary>
    public GameObject earthElemelonDisplayablePrefab;

    /// <summary>
    /// Singleton instance of MainGame.
    /// </summary>
    public static MainGame Instance { get; private set; }

    /// <summary>
    /// List of dialogue data for the King's introduction.
    /// </summary>
    public List<DialogueData> kingIntroDialoguesList;

    /// <summary>
    /// List of repetitive dialogue data for the King.
    /// </summary>
    public List<DialogueData> kingRepetitiveDialoguesList;

    /// <summary>
    /// List of dialogue data for the King's ending.
    /// </summary>
    public List<DialogueData> kingEndDialoguesList;

    // Tracks if the King dialogue is currently active.
    private static bool kingDialogueActive = false;

    // Unity Awake callback. Initializes the singleton instance and ensures persistence across scenes.
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
    /// Starts the specified game stage, loading the appropriate scene and initializing stage logic.
    /// </summary>
    /// <param name="stage">The progression stage to start.</param>
    /// <param name="isStartScene">Whether this is the start scene (optional).</param>
    public void StartGameStage(GameProgressionStage stage, bool isStartScene = false)
    {
        Debug.Log("Starting game stage: " + stage);
        StartCoroutine(StartGameStageAsync(stage, isStartScene));
    }

    // Coroutine to load the scene and handle stage-specific logic.
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

    // Handles logic for the castle introduction stage.
    private void HandleCastleIntroStage()
    {
        // TODO: Play the castle intro cutscene
    }

    // Handles logic for the early castle stage, including portal event setup.
    private void HandleCastleEarlyStage()
    {
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

    // Handles logic for the early earth stage, including boss room access.
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

    // Handles logic for the early earth boss stage, including player teleport and boss defeat event.
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

    // Sets up the event for collecting the earth elemelon and progressing the stage.
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

    // Handles logic for the castle end stage, including King dialogue event setup.
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

    // Handles logic for the late castle stage, including portal event setup.
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

    // Handles logic for the late earth stage, including boss room access.
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

    // Handles logic for the late earth boss stage, including player teleport and boss defeat event.
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

    // Resets the player's position to the initial spawn point.
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

    // Waits for a delay, then changes the progression stage.
    private IEnumerator ChangeProgressionStageAfterDelay(GameProgressionStage stage, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.ChangeProgressionStage(stage);
    }

    // Spawns the earth elemelon displayable in the scene.
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
