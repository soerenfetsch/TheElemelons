using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InputManagerTests
{
    private GameObject playerGameObject;
    private InputManager inputManager;
    private PlayerMovement playerMovement;
    private Camera mainCamera;

    [SetUp]
    public void Setup()
    {
        // Create a Player GameObject with InputManager and PlayerMovement
        playerGameObject = new GameObject("Player");
        playerMovement = playerGameObject.AddComponent<PlayerMovement>();
        inputManager = playerGameObject.AddComponent<InputManager>();

        // Add a mock Main Camera
        mainCamera = new GameObject("Main_Camera").AddComponent<Camera>();
        mainCamera.tag = "MainCamera";

        // Ensure the InputManager references the mock camera
        inputManager.cam = mainCamera;

        // Initialize public fields
        playerMovement.movement = Vector2.zero;
        playerMovement.mousePos = Vector2.zero;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(playerGameObject);
        Object.DestroyImmediate(mainCamera.gameObject);
    }

    [Test]
    public void InputManager_InitializesKeyBindings()
    {
        // Assert key bindings are correctly initialized
        Assert.AreEqual(KeyCode.A, inputManager.keys["moveLeft"], "Key binding for moveLeft is incorrect.");
        Assert.AreEqual(KeyCode.D, inputManager.keys["moveRight"], "Key binding for moveRight is incorrect.");
        Assert.AreEqual(KeyCode.S, inputManager.keys["moveDown"], "Key binding for moveDown is incorrect.");
        Assert.AreEqual(KeyCode.W, inputManager.keys["moveUp"], "Key binding for moveUp is incorrect.");
    }

    [UnityTest]
    public IEnumerator InputManager_HandlesMousePositionCorrectly()
{
    // Arrange
    Vector3 mockMousePosition = new Vector3(10f, 5f, 0f);
    Vector3 expectedWorldPosition = mainCamera.ScreenToWorldPoint(mockMousePosition);

    // Mock the ScreenToWorldPoint result
    mainCamera.transform.position = new Vector3(0, 0, -10); // Ensure the camera is at a reasonable position

    // Simulate mouse position indirectly by manipulating the camera’s world point
    yield return null; // Wait for frame to process

    // Act
    inputManager.cam = mainCamera; // Ensure InputManager uses the mock camera
    yield return null;
    // Assert
    Assert.AreEqual(expectedWorldPosition, playerMovement.mousePos, "Mouse position is not correctly converted to world position.");
}
    [UnityTest]
    public IEnumerator InputManager_HandlesHorizontalMovementInput()
    {
        // Simulate pressing the "moveLeft" key
        Input.GetKeyDown(KeyCode.A);
        yield return null;

        // Assert
        Assert.AreEqual(-1, playerMovement.movement.x, "Player should move left when 'moveLeft' key is pressed.");

        // Simulate releasing the "moveLeft" key and pressing "moveRight"
        Input.GetKeyUp(KeyCode.A);
        Input.GetKeyDown(KeyCode.D);
        yield return null;

        // Assert
        Assert.AreEqual(1, playerMovement.movement.x, "Player should move right when 'moveRight' key is pressed.");

        // Simulate releasing both keys
        Input.GetKeyUp(KeyCode.D);
        yield return null;

        // Assert
        Assert.AreEqual(0, playerMovement.movement.x, "Player's horizontal movement should reset to 0 when no keys are pressed.");
    }

    [UnityTest]
    public IEnumerator InputManager_HandlesVerticalMovementInput()
    {
        // Simulate pressing the "moveUp" key
        Input.GetKeyDown(KeyCode.W);
        yield return null;

        // Assert
        Assert.AreEqual(1, playerMovement.movement.y, "Player should move up when 'moveUp' key is pressed.");

        // Simulate releasing the "moveUp" key and pressing "moveDown"
        Input.GetKeyUp(KeyCode.W);
        Input.GetKeyDown(KeyCode.S);
        yield return null;

        // Assert
        Assert.AreEqual(-1, playerMovement.movement.y, "Player should move down when 'moveDown' key is pressed.");

        // Simulate releasing both keys
        Input.GetKeyUp(KeyCode.S);
        yield return null;

        // Assert
        Assert.AreEqual(0, playerMovement.movement.y, "Player's vertical movement should reset to 0 when no keys are pressed.");
    }
}