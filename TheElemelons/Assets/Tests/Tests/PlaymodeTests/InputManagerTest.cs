using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class MockCasting : MonoBehaviour
{
    public bool CastSpellCalled { get; private set; } = false;

    public void CastSpell()
    {
        CastSpellCalled = true; // Track invocation
    }
}

public class InputManagerPlayModeTests
{
    private GameObject playerGameObject;

    [SetUp]
    public void SetUp()
    {
        playerGameObject = new GameObject("Player");
        var casting = playerGameObject.AddComponent<Casting>();
        var playerMovement = playerGameObject.AddComponent<PlayerMovement>();

        // Disable the casting and player movement scripts
        casting.enabled = false;
        playerMovement.enabled = false;

        // Add a mock Main Camera
        Camera mainCamera = new GameObject("Main_Camera").AddComponent<Camera>();
        mainCamera.tag = "MainCamera";
        playerGameObject.SetActive(true);
    }

    [UnityTest]
    public IEnumerator InputManager_InitializesKeyBindingsCorrectly()
    {
        // Arrange
        GameObject inputManagerObj = new GameObject("InputManager");
        InputManager inputManager = inputManagerObj.AddComponent<InputManager>();

        // Act
        yield return null; // Allow Awake() to execute.

        // Assert
        Assert.AreEqual(KeyCode.A, inputManager.GetKeyBinding("MoveLeft"), "MoveLeft key binding is incorrect.");
        Assert.AreEqual(KeyCode.D, inputManager.GetKeyBinding("MoveRight"), "MoveRight key binding is incorrect.");
        Assert.AreEqual(KeyCode.W, inputManager.GetKeyBinding("MoveUp"), "MoveUp key binding is incorrect.");
        Assert.AreEqual(KeyCode.S, inputManager.GetKeyBinding("MoveDown"), "MoveDown key binding is incorrect.");
        Assert.AreEqual(KeyCode.Mouse0, inputManager.GetKeyBinding("CastSpell"), "CastSpell key binding is incorrect.");

        // Clean up
        Object.Destroy(inputManagerObj);
    }

    [UnityTest]
    public IEnumerator InputManager_UpdatesKeyBindingsCorrectly()
    {
        // Arrange
        GameObject gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();

        GameObject inputManagerObj = new GameObject("InputManager");
        InputManager inputManager = inputManagerObj.AddComponent<InputManager>();
        yield return null; // Allow Awake() to execute.

        // Act
        inputManager.SaveKeyBinding("MoveLeft", KeyCode.LeftArrow);
        KeyCode updatedKey = inputManager.GetKeyBinding("MoveLeft");

        // Assert
        Assert.AreEqual(KeyCode.LeftArrow, updatedKey, "MoveLeft key binding was not updated.");

        // Clean up
        Object.Destroy(inputManagerObj);
        Object.Destroy(gmObj);
    }


    [TearDown]
    public void TearDown()
    {
        foreach (var obj in Object.FindObjectsOfType<GameObject>())
        {
            Object.DestroyImmediate(obj);
        }
        Debug.Log("Teardown complete: All game objects destroyed.");
    }
}

