using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the settings menu UI. This class displays key binding entries
/// and provides a back button to return to the previous UI state.
/// It calls the necessary GameManager or InputManager methods when user actions occur.
/// Implements IUIPanel to allow centralized UI control.
/// </summary>
public class SettingsMenuUI : MonoBehaviour, IUIPanel
{
    [Header("UI References")]
    public Transform contentPanel;
    public GameObject backButtonObject;
    public GameObject keyBindingEntryPrefab;
    public InputManager inputManager;

    private Button backButton;
    private Dictionary<string, GameObject> keyBindingEntries = new Dictionary<string, GameObject>();

    // List of actions to display key bindings for.
    private List<string> actions = new List<string>
    {
        "MoveLeft", "MoveRight", "MoveUp", "MoveDown",
        "CastSpell", "ClearCharges",
        "SelectFire", "SelectWater", "SelectEarth", "SelectAir",
        "ZoomIn", "ZoomOut",
        "Interact"
    };

    /// <summary>
    /// Initializes the settings UI. Checks for necessary components and builds the key binding entries.
    /// </summary>
    private void Start()
    {
        if (inputManager == null)
        {
            Logger.LogError("SettingsMenuUI: InputManager not set.");
            return;
        }
        if (contentPanel == null)
        {
            Logger.LogError("SettingsMenuUI: ContentPanel not set.");
            return;
        }
        if (keyBindingEntryPrefab == null)
        {
            Logger.LogError("SettingsMenuUI: KeyBindingEntryPrefab not set.");
            return;
        }

        InitializeKeyBindings();
        InitializeBackButton();
    }

    /// <summary>
    /// Initializes the back button and assigns its click event.
    /// </summary>
    private void InitializeBackButton()
    {
        backButton = backButtonObject.GetComponent<Button>();
        if (backButton == null)
        {
            Logger.LogError("SettingsMenuUI: BackButton not found.");
            return;
        }
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnDestroy()
    {
        // Remove back button listeners to avoid memory leaks.
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
    }

    /// <summary>
    /// Handles the back button click.
    /// Returns to the previous menu (here, the Main Menu state) via GameManager.
    /// </summary>
    private void OnBackButtonClicked()
    {
        Logger.Log("SettingsMenuUI: Back button clicked");
        GameManager.Instance.GoToMainMenu();
    }

    /// <summary>
    /// Initializes key binding entries for each action by instantiating UI prefabs.
    /// </summary>
    private void InitializeKeyBindings()
    {
        foreach (string action in actions)
        {
            // Retrieve the current key binding from the InputManager.
            string keyName = inputManager.GetKeyBinding(action).ToString();
            GameObject entry = CreateKeyBindingEntry(action, keyName);
            if (entry != null)
            {
                keyBindingEntries[action] = entry;
            }
        }
    }

    /// <summary>
    /// Instantiates a key binding entry prefab, sets the action and key texts,
    /// and attaches a click listener for rebinding.
    /// </summary>
    /// <param name="actionName">The name of the action.</param>
    /// <param name="keyName">The currently bound key (as string).</param>
    /// <returns>The instantiated key binding entry GameObject, or null on error.</returns>
    private GameObject CreateKeyBindingEntry(string actionName, string keyName)
    {
        GameObject entry = Instantiate(keyBindingEntryPrefab, contentPanel);

        // Navigate through the child hierarchy.
        Transform actionTextTransform = entry.transform.Find("KeyBindingAction/ActionText");
        Transform buttonTextTransform = entry.transform.Find("KeyBindingButton/ButtonText");
        Transform keyBindButtonTransform = entry.transform.Find("KeyBindingButton");

        if (actionTextTransform == null)
        {
            Logger.LogError($"SettingsMenuUI: ActionText not found under KeyBindingAction in {entry.name}");
            return null;
        }
        if (buttonTextTransform == null)
        {
            Logger.LogError($"SettingsMenuUI: ButtonText not found under KeyBindingButton in {entry.name}");
            return null;
        }
        if (keyBindButtonTransform == null)
        {
            Logger.LogError($"SettingsMenuUI: KeyBindingButton not found in {entry.name}");
            return null;
        }

        TMP_Text actionText = actionTextTransform.GetComponent<TMP_Text>();
        TMP_Text buttonText = buttonTextTransform.GetComponent<TMP_Text>();

        if (actionText != null)
            actionText.text = actionName;
        if (buttonText != null)
            buttonText.text = keyName;

        Button keyButton = keyBindButtonTransform.GetComponent<Button>();
        if (keyButton != null)
        {
            keyButton.onClick.AddListener(() => OnKeyBindButtonClicked(actionName));
        }

        return entry;
    }

    /// <summary>
    /// Called when a key binding entry is clicked to rebind a key.
    /// For now, this method simulates key rebinding by logging the action.
    /// </summary>
    /// <param name="actionName">The action for which key binding is being changed.</param>
    private void OnKeyBindButtonClicked(string actionName)
    {
        Logger.Log($"SettingsMenuUI: Initiating rebinding for action: {actionName}");
        // Simulate key rebinding; in a full implementation, call InputManager's rebind functionality:
        // inputManager.StartRebinding(actionName, newKey => UpdateKeyBindingEntry(actionName, newKey));
        // Here we simulate with a placeholder value:
        UpdateKeyBindingEntry(actionName, "NewKey");
    }

    /// <summary>
    /// Updates the UI entry for a given action with the new key value.
    /// </summary>
    /// <param name="actionName">The action name.</param>
    /// <param name="newKey">The new key (as a string).</param>
    public void UpdateKeyBindingEntry(string actionName, string newKey)
    {
        if (keyBindingEntries.TryGetValue(actionName, out GameObject entry))
        {
            Transform buttonTextTransform = entry.transform.Find("KeyBindingButton/ButtonText");
            if (buttonTextTransform != null)
            {
                TMP_Text buttonText = buttonTextTransform.GetComponent<TMP_Text>();
                if (buttonText != null)
                    buttonText.text = newKey;
            }
        }
    }

    #region IUIPanel Implementation

    /// <summary>
    /// Shows the settings menu panel.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the settings menu panel.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
