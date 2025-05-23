using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGame : MonoBehaviour
{
    public GameObject earthElemelonPrefab; // Prefab for the earth elemelon
    public static MainGame Instance { get; private set; }
    public List<DialogueData> kingIntroDialoguesList;
    public List<DialogueData> kingRepetitiveDialoguesList;

    private static bool kingDialogueActive = false;

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

    private void Start()
    {
        // Add event when the player is defeated
        CombatManager.OnPlayerDeath += () =>
        {
            GameManager.Instance.ChangeGameStatus(GameStatus.DEATH_SCREEN);
            UIManager.Instance.ToggleBossBar(false, null);
        };
    }

    public void StartGameStage(GameProgressionStage stage, bool isStartScene = false)
    {
        Debug.Log("Starting game stage: " + stage);
        StartCoroutine(StartGameStageAsync(stage, isStartScene));
    }

    private IEnumerator StartGameStageAsync(GameProgressionStage stage, bool isStartScene = false)
    {
        // Load scene of the game stage
        string sceneToLoad = GetSceneForStage(stage);
        yield return StartCoroutine(LoadScene(sceneToLoad));

        ResetPlayerCombatStatus();

        GameStatus sceneStatus = isStartScene ? GameStatus.MAIN_MENU : GameStatus.INGAME;
        GameManager.Instance.ChangeGameStatus(sceneStatus);

        // Scene is now loaded
        switch (stage)
        {
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

    private void HandleCastleIntroStage()
    {
        // TODO: Play the castle intro cutscene

        // TODO: Start the king dialogue about stolen elemelons

        // Make the initial king dialogue available and subscribe the stage change to it
        EnableKingIntroDialogue();
        Action<string, Sprite, Sprite> onKingDialogueStarted = null;
        Action onKingDialogueEnded = null;

        onKingDialogueStarted = (string speaker, Sprite sprite1, Sprite sprite2) =>
        {
            if (speaker == "King")
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
                DialogueManager.OnDialogueStarted -= onKingDialogueStarted;
                DialogueManager.OnDialogueEnded -= onKingDialogueEnded;

                GameManager.Instance.ChangeProgressionStage(GameProgressionStage.CASTLE_EARLY);
            }
        };

        // Subscribe both
        DialogueManager.OnDialogueStarted += onKingDialogueStarted;
        DialogueManager.OnDialogueEnded += onKingDialogueEnded;

    }

    private void HandleCastleEarlyStage()
    {
        EnableKingRepetitiveDialogue();

        GameObject portal = GameObject.Find("EarthPortal");
        if (portal != null)
        {
            portal.GetComponent<Portal>().OnPortalEnter += () =>
            {
                GameManager.Instance.ChangeProgressionStage(GameProgressionStage.EARTH_EARLY);
                portal.GetComponent<Portal>().OnPortalEnter -= portal.GetComponent<Portal>().OnPortalEnter;
            };
        }
        else
        {
            Debug.LogError("EarthPortal GameObject not found in the scene.");
        }
    }

    private void HandleEarthEarlyStage()
    {
        // Allow the boss room to be accessed when all monsters are defeated
        GameObject endPortalGameObject = GameObject.Find("EndPortal");
        Portal endPortal = endPortalGameObject?.GetComponent<Portal>();
        MonsterManager monsterManager = GameObject.Find("MonsterManager").GetComponent<MonsterManager>();
        if (endPortalGameObject != null && endPortal.OnPortalEnter == null)
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

    private void HandleEarthEarlyBossStage()
    {
        // Teleport the player to the start location of the boss room
        GameObject player = GameObject.Find("Player");
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
                GameObject earthElemelon = Instantiate(earthElemelonPrefab, witchObject.transform.position, Quaternion.identity);
                // If the earth elemelon is picked up, the progression stage is changed
                SetupEarthElemelonPickup(earthElemelon);
            };
        }
        else
        {
            Debug.LogError("Witch GameObject not found in the scene.");
        }
    }

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

    private void HandleCastleEndStage()
    {
        // TODO: Add elemelon retrieval dialogue to the King

        // TODO: Add event to progress to earth_late when dialogue is finished
    }

    private void HandleCastleLateStage()
    {
        // TODO: If the earth elemelon is not loaded, load it into the scene

        // TODO: Add the repetitive king dialogue to the king

        // TODO: Add the event to the earth portal to start the earth late stage
    }

    private void HandleEarthLateStage()
    {
        // TODO: Allow the boss room to be accessed when all monsters are defeated

        // TODO: Add the boss room scene when the room is entered
    }

    private void HandleEarthLateBossStage()
    {
        // If the boss is defeated, load the castle late stage
    }

    private static IEnumerator LoadScene(string sceneToLoad)
    {
        if (!string.IsNullOrEmpty(sceneToLoad) && SceneManager.GetActiveScene().name != sceneToLoad)
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
        yield return null;
    }

    private static void ResetPlayerPosition()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(0, 0, 0); // Reset to initial position
        }
        else
        {
            Debug.LogError("Player GameObject not found in the scene.");
        }
    }

    private static string GetSceneForStage(GameProgressionStage stage)
    {
        switch (stage)
        {
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

    private void EnableKingIntroDialogue()
    {
        // Find king in the scene and add the dialogue
        GameObject king = GameObject.Find("King");
        if (king != null)
        {
            king.GetComponent<King>().basic_dialogues_list = kingIntroDialoguesList;
        }
        else
        {
            Debug.LogError("King GameObject not found in the scene.");
        }
    }

    private void EnableKingRepetitiveDialogue()
    {
        // Find king in the scene and add the dialogue
        GameObject king = GameObject.Find("King");
        if (king != null)
        {
            king.GetComponent<King>().basic_dialogues_list = kingRepetitiveDialoguesList;
        }
        else
        {
            Debug.LogError("King GameObject not found in the scene.");
        }
    }

    private static void ResetPlayerCombatStatus()
    {
        // Find player in the scene and reset it
        GameObject player = GameObject.Find("Player");
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
}
