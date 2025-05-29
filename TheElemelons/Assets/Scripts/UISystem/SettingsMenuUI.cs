using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    public Transform contentPanel;
    public GameObject backButtonObject;
    public GameObject keyBindingEntryPrefab;
    public InputManager inputManager;

    public TMP_FontAsset font;
    private Button backButton;
    private bool isRebinding = false;
    private Dictionary<string, GameObject> keyBindingEntries = new Dictionary<string, GameObject>();

    // TODO: Get actions from InputManager
    private List<string> actions = new List<string> {
        "MoveLeft", "MoveRight", "MoveUp", "MoveDown",
        "CastSpell", "ClearCharges",
        "SelectFire", "SelectWater", "SelectEarth", "SelectAir",
        "ZoomIn", "ZoomOut",
        "Interact"
    };

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

    private void OnBackButtonClicked()
    {
        if (isRebinding) return;
        GameManager.Instance.ChangeGameStatus(GameManager.Instance.PreviousGameStatus);
    }

    private void InitializeKeyBindings()
    {
        foreach (string action in actions)
        {
            string keyName = inputManager.GetKeyBinding(action).ToString();
            GameObject entry = CreateKeyBindingEntry(action, keyName);
            keyBindingEntries[action] = entry;
        }
    }

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
        buttonTextTransform.GetComponent<TMP_Text>().text = keyName;

        // Add click listener to the button
        keyBindButtonTransform.GetComponent<Button>().onClick.AddListener(() => OnKeyBindButtonClicked(actionName));

        return entry;
    }

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

    public void UpdateKeyBindingEntry(string actionName, string newKey)
    {
        if (keyBindingEntries.TryGetValue(actionName, out GameObject entry))
        {
            TMP_Text txt = entry.transform.Find("KeyBindingButton/ButtonText").GetComponent<TMP_Text>();
            txt.text = newKey;
            txt.font = font;
        }
    }

}
