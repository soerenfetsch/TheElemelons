using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGame : MonoBehaviour
{
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

    public void StartGameStage(GameProgressionStage stage)
    {
        Debug.Log("Starting game stage: " + stage);

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
            case GameProgressionStage.CASTLE_END:
                HandleCastleEndStage();
                break;
            case GameProgressionStage.CASTLE_LATE:
                HandleCastleLateStage();
                break;
            case GameProgressionStage.EARTH_LATE:
                HandleEarthLateStage();
                break;
            default:
                Debug.LogError("Unknown game stage: " + stage);
                break;
        }
    }

    private void HandleCastleIntroStage()
    {
        // TODO: Play the castle intro cutscene

        // TODO: Start the king dialogue about stolen elemelons

        // If the scene is not loaded, load it
        if (SceneManager.GetActiveScene().name != "Castle_Hub_Scene")
        {
            GameManager.Instance.ChangeGameStatus(GameStatus.LOADING_SCREEN);
            SceneManager.LoadScene("Castle_Hub_Scene");
            GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
        }

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
        // If the scene is not loaded, load it
        if (SceneManager.GetActiveScene().name != "Castle_Hub_Scene")
        {
            GameManager.Instance.ChangeGameStatus(GameStatus.LOADING_SCREEN);
            SceneManager.LoadScene("Castle_Hub_Scene");
            GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
        }

        EnableKingRepetitiveDialogue();

        GameObject portal = GameObject.Find("EarthPortal");
        if (portal != null)
        {
            portal.GetComponent<Portal>().OnPortalEnter += () =>
            {
                GameManager.Instance.ChangeProgressionStage(GameProgressionStage.EARTH_EARLY);
            };
        }
        else
        {
            Debug.LogError("EarthPortal GameObject not found in the scene.");
        }
    }

    private void HandleEarthEarlyStage()
    {
        // If the scene is not loaded, load it
        if (SceneManager.GetActiveScene().name != "Earth_Dungeon_Scene")
        {
            GameManager.Instance.ChangeGameStatus(GameStatus.LOADING_SCREEN);
            SceneManager.LoadScene("Earth_Dungeon_Scene");
            GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
        }
        // TODO: Allow the boss room to be accessed when all monsters are defeated

        // TODO: Add the boss room scene when the room is entered

        // TODO: Add the event to progress to castle end when the boss is defeated

        // TODO: Add event to go back to castle_early stage when the player is defeated
    }

    private void HandleCastleEndStage()
    {
        // If the scene is not loaded, load it
        if (SceneManager.GetActiveScene().name != "Castle_Hub_Scene")
        {
            GameManager.Instance.ChangeGameStatus(GameStatus.LOADING_SCREEN);
            SceneManager.LoadScene("Castle_Hub_Scene");
            GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
        }

        // TODO: Add elemelon retrieval dialogue to the King

        // TODO: Add event to progress to earth_late when dialogue is finished
    }

    private void HandleCastleLateStage()
    {
        // If the scene is not loaded, load it
        if (SceneManager.GetActiveScene().name != "Castle_Hub_Scene")
        {
            GameManager.Instance.ChangeGameStatus(GameStatus.LOADING_SCREEN);
            SceneManager.LoadScene("Castle_Hub_Scene");
            GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
        }

        // TODO: If the earth elemelon is not loaded, load it into the scene

        // TODO: Add the repetitive king dialogue to the king

        // TODO: Add the event to the earth portal to start the earth late stage
    }

    private void HandleEarthLateStage()
    {
        // If the scene is not loaded, load it
        if (SceneManager.GetActiveScene().name != "Earth_Dungeon_Scene")
        {
            GameManager.Instance.ChangeGameStatus(GameStatus.LOADING_SCREEN);
            SceneManager.LoadScene("Earth_Dungeon_Scene");
            GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
        }

        // TODO: Allow the boss room to be accessed when all monsters are defeated

        // TODO: Add the boss room scene when the room is entered

        // TODO: Add the event to progress to castle late when the boss or the player is defeated

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
}
