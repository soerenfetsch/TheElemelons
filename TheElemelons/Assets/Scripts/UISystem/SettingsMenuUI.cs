using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the UI for the settings menu, primarily focusing on key rebinding functionality.
/// </summary>
public class SettingsMenuUI : MonoBehaviour
{
    /// <summary>
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
    /// </summary>
    public InputManager inputManager;

    /// <summary>
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
    private List<string> actions = new List<string> {
        "MoveLeft", "MoveRight", "MoveUp", "MoveDown",
        "CastSpell", "ClearCharges",
        "SelectFire", "SelectWater", "SelectEarth", "SelectAir",
        "ZoomIn", "ZoomOut",
        "Interact"
    };

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Validates required references and initializes the key bindings and back button.
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
    /// Initializes the back button, retrieving its component and adding a listener for click events.
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
    /// Handles the click event for the back button.
    /// Returns to the previous game state if no rebinding process is active.
    /// </summary>
    private void OnBackButtonClicked()
    {
        if (isRebinding) return;
        GameManager.Instance.ChangeGameStatus(GameManager.Instance.PreviousGameStatus);
    }

    /// <summary>
    /// Initializes the UI for all key bindings defined in the 'actions' list.
    /// Creates a UI entry for each action.
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
    /// Creates and configures a UI entry for a specific key binding.
    /// </summary>
    /// <param name="actionName">The name of the action to bind.</param>
    /// <param name="keyName">The current key bound to the action.</param>
    /// <returns>The GameObject representing the created UI entry, or null if an error occurred.</returns>
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
    /// Handles the click event for a key binding button.
    /// Initiates the rebinding process for the specified action.
    /// </summary>
    /// <param name="actionName">The name of the action to rebind.</param>
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
    /// Updates the displayed key for a specific action in the UI.
    /// </summary>
    /// <param name="actionName">The name of the action whose UI entry needs updating.</param>
    /// <param name="newKey">The new key to display.</param>
    public void UpdateKeyBindingEntry(string actionName, string newKey)
    {
        if (keyBindingEntries.TryGetValue(actionName, out GameObject entry))
        {
            TMP_Text txt = entry.transform.Find("KeyBindingButton/ButtonText").GetComponent<TMP_Text>();
            txt.text = newKey;
        }
    }

}
