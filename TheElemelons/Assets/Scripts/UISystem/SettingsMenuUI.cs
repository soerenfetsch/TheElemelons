using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
<<<<<<< Updated upstream
/// Manages the UI for the settings menu, primarily focusing on key rebinding functionality.
=======
/// Controls the settings menu UI, including key binding management and navigation.
/// Allows rebinding of keys for player actions and returning to the previous menu.
>>>>>>> Stashed changes
/// </summary>
public class SettingsMenuUI : MonoBehaviour
{
    /// <summary>
<<<<<<< Updated upstream
    /// The parent transform under which key binding entries will be instantiated.
    /// </summary>
    public Transform contentPanel;
    /// <summary>
    /// The GameObject containing the back button UI element.
    /// </summary>
    public GameObject backButtonObject;
    /// <summary>
    /// The prefab used to create individual key binding entries in the UI.
    /// </summary>
    public GameObject keyBindingEntryPrefab;
    /// <summary>
    /// Reference to the InputManager to handle key binding logic.
=======
    /// The panel that contains all key binding entries.
    /// </summary>
    public Transform contentPanel;

    /// <summary>
    /// The back button GameObject.
    /// </summary>
    public GameObject backButtonObject;

    /// <summary>
    /// The prefab for a single key binding entry row.
    /// </summary>
    public GameObject keyBindingEntryPrefab;

    /// <summary>
    /// Reference to the InputManager for key binding logic.
>>>>>>> Stashed changes
    /// </summary>
    public InputManager inputManager;

    /// <summary>
<<<<<<< Updated upstream
    /// The font asset to be used for text elements within the settings menu.
    /// </summary>
    public TMP_FontAsset font;
    /// <summary>
    /// Reference to the UI Button component for the back button.
    /// </summary>
    private Button backButton;
    /// <summary>
    /// Flag to indicate if a key rebinding process is currently active.
    /// </summary>
    private bool isRebinding = false;
    /// <summary>
    /// A dictionary mapping action names to their corresponding UI GameObject entries.
    /// </summary>
    private Dictionary<string, GameObject> keyBindingEntries = new Dictionary<string, GameObject>();

    // TODO: Get actions from InputManager
    /// <summary>
    /// List of action names for which key bindings can be configured.
    /// Note: This is currently a hardcoded list and marked with a TODO to fetch from InputManager.
    /// </summary>
=======
    /// The font asset used for key binding entry text.
    /// </summary>
    public TMP_FontAsset font;

    // Reference to the back button component.
    private Button backButton;
    // Whether a key is currently being rebound.
    private bool isRebinding = false;
    // Dictionary mapping action names to their entry GameObjects.
    private Dictionary<string, GameObject> keyBindingEntries = new Dictionary<string, GameObject>();

    // List of actions that can be rebound.
>>>>>>> Stashed changes
    private List<string> actions = new List<string> {
        "MoveLeft", "MoveRight", "MoveUp", "MoveDown",
        "CastSpell", "ClearCharges",
        "SelectFire", "SelectWater", "SelectEarth", "SelectAir",
        "ZoomIn", "ZoomOut",
        "Interact"
    };

    /// <summary>
<<<<<<< Updated upstream
    /// Called when the script instance is being loaded.
    /// Validates required references and initializes the key bindings and back button.
=======
    /// Unity Start method. Initializes key bindings and the back button.
>>>>>>> Stashed changes
    /// </summary>
    public void Start()
    {
        if (inputManager == null)
        {
            Debug.LogError("InputManager not set in SettingsMenuUI.");
            return;
        }
        if (contentPanel == null)
        {
            Debug.LogError("ContentPanel not set in SettingsMenuUI.");
            return;
        }
        if (keyBindingEntryPrefab == null)
        {
            Debug.LogError("KeyBindingEntryPrefab not set in SettingsMenuUI.");
            return;
        }

        InitializeKeyBindings();
        InitializeBackButton();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Initializes the back button, retrieving its component and adding a listener for click events.
=======
    /// Initializes the back button and attaches its click event.
>>>>>>> Stashed changes
    /// </summary>
    private void InitializeBackButton()
    {
        backButton = backButtonObject.GetComponent<Button>();
        if (backButton == null)
        {
            Debug.LogError("BackButton not found in SettingsMenuUI.");
            return;
        }
        // Add on click listener to return to last menu
        backButton.onClick.AddListener(() => OnBackButtonClicked());
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the click event for the back button.
    /// Returns to the previous game state if no rebinding process is active.
=======
    /// Handles the back button click, returning to the previous menu if not rebinding.
>>>>>>> Stashed changes
    /// </summary>
    private void OnBackButtonClicked()
    {
        if (isRebinding) return;
        GameManager.Instance.ChangeGameStatus(GameManager.Instance.PreviousGameStatus);
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Initializes the UI for all key bindings defined in the 'actions' list.
    /// Creates a UI entry for each action.
=======
    /// Initializes the key binding entries for all actions.
>>>>>>> Stashed changes
    /// </summary>
    private void InitializeKeyBindings()
    {
        foreach (string action in actions)
        {
            string keyName = inputManager.GetKeyBinding(action).ToString();
            GameObject entry = CreateKeyBindingEntry(action, keyName);
            keyBindingEntries[action] = entry;
        }
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Creates and configures a UI entry for a specific key binding.
    /// </summary>
    /// <param name="actionName">The name of the action to bind.</param>
    /// <param name="keyName">The current key bound to the action.</param>
    /// <returns>The GameObject representing the created UI entry, or null if an error occurred.</returns>
=======
    /// Instantiates a key binding entry row and sets up its UI and click event.
    /// </summary>
    /// <param name="actionName">The action name.</param>
    /// <param name="keyName">The current key binding as a string.</param>
    /// <returns>The created entry GameObject.</returns>
>>>>>>> Stashed changes
    private GameObject CreateKeyBindingEntry(string actionName, string keyName)
    {
        GameObject entry = Instantiate(keyBindingEntryPrefab, contentPanel);

        // Navigate correctly through the hierarchy
        Transform actionTextTransform = entry.transform.Find("KeyBindingAction/ActionText");
        Transform buttonTextTransform = entry.transform.Find("KeyBindingButton/ButtonText");
        Transform keyBindButtonTransform = entry.transform.Find("KeyBindingButton");

        // Check if any of these are null to catch errors early
        if (actionTextTransform == null)
        {
            Debug.LogError($"ActionText not found under KeyBindingAction in {entry.name}");
            return null;
        }
        if (buttonTextTransform == null)
        {
            Debug.LogError($"ButtonText not found under KeyBindingButton in {entry.name}");
            return null;
        }
        if (keyBindButtonTransform == null)
        {
            Debug.LogError($"KeyBindingButton not found in {entry.name}");
            return null;
        }

        // Set text values
        actionTextTransform.GetComponent<TMP_Text>().text = actionName;
        actionTextTransform.GetComponent<TMP_Text>().font = font;
        buttonTextTransform.GetComponent<TMP_Text>().text = keyName;
        buttonTextTransform.GetComponent<TMP_Text>().font = font;
        // Add click listener to the button
        keyBindButtonTransform.GetComponent<Button>().onClick.AddListener(() => OnKeyBindButtonClicked(actionName));

        return entry;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the click event for a key binding button.
    /// Initiates the rebinding process for the specified action.
    /// </summary>
    /// <param name="actionName">The name of the action to rebind.</param>
=======
    /// Handles the click event for rebinding a key for a specific action.
    /// </summary>
    /// <param name="actionName">The action to rebind.</param>
>>>>>>> Stashed changes
    private void OnKeyBindButtonClicked(string actionName)
    {
        if (isRebinding) return; // Prevent multiple rebinding at the same time
        Debug.Log($"Rebinding key for action: {actionName}");
        isRebinding = true;

        inputManager.StartRebinding(actionName, newKey =>
        {
            // Update the UI when the key is changed
            UpdateKeyBindingEntry(actionName, newKey.ToString());

            Debug.Log($"Key for '{actionName}' successfully rebound to {newKey}");
            isRebinding = false;
        });
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Updates the displayed key for a specific action in the UI.
    /// </summary>
    /// <param name="actionName">The name of the action whose UI entry needs updating.</param>
    /// <param name="newKey">The new key to display.</param>
=======
    /// Updates the displayed key binding for a specific action in the UI.
    /// </summary>
    /// <param name="actionName">The action name.</param>
    /// <param name="newKey">The new key as a string.</param>
>>>>>>> Stashed changes
    public void UpdateKeyBindingEntry(string actionName, string newKey)
    {
        if (keyBindingEntries.TryGetValue(actionName, out GameObject entry))
        {
            TMP_Text txt = entry.transform.Find("KeyBindingButton/ButtonText").GetComponent<TMP_Text>();
            txt.text = newKey;
        }
    }
}
