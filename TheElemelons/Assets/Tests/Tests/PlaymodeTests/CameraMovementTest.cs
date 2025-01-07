using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class CameraMovementTests
{
    private GameObject cameraObject;
    private CameraMovement cameraMovement;
    private Camera mainCamera;
    private GameObject playerObject;

    [SetUp]
    public void Setup()
    {
        // Create camera GameObject with CameraMovement component
        cameraObject = new GameObject("Camera");
        mainCamera = cameraObject.AddComponent<Camera>();
        cameraMovement = cameraObject.AddComponent<CameraMovement>();

        // Create player GameObject
        playerObject = new GameObject("Player");
        cameraMovement.playerTransform = playerObject.transform;
    }

    [TearDown]
    public void Teardown()
    {
        // Destroy test objects
        Object.Destroy(cameraObject);
        Object.Destroy(playerObject);
    }

    [UnityTest]
    public IEnumerator TestDefaultCameraSizeInitialization()
    {
        yield return null; // Wait for Awake to run

        // Verify that the default camera size is set
        Assert.AreEqual(cameraMovement.defaultCameraSize, mainCamera.orthographicSize);
    }

    [UnityTest]
    public IEnumerator TestClampedCameraSize()
    {
        // Set camera size below minimum
        cameraMovement.SetCameraSize(1f);
        yield return null; // Allow for any game loop logic if necessary
        Assert.AreEqual(cameraMovement.minCameraSize, mainCamera.orthographicSize);

        // Set camera size above maximum
        cameraMovement.SetCameraSize(15f);
        yield return null;
        Assert.AreEqual(cameraMovement.maxCameraSize, mainCamera.orthographicSize);

        // Set camera size within range
        cameraMovement.SetCameraSize(7f);
        yield return null;
        Assert.AreEqual(7f, mainCamera.orthographicSize);
    }

    [UnityTest]
    public IEnumerator TestPlayerTransformFollowLogic()
    {
        // Set player position
        playerObject.transform.position = new Vector3(5f, 3f, 0f);

        yield return null; // Wait for LateUpdate to run

        // Verify that the camera follows the player while fixing the Z-axis
        Vector3 expectedPosition = new Vector3(5f, 3f, -10f);
        Assert.AreEqual(expectedPosition, cameraObject.transform.position);
    }

    [UnityTest]
    public IEnumerator TestFallbackForMainCamera()
    {
        // Remove the main camera component
        Object.Destroy(mainCamera);

        yield return null; // Wait for property fallback logic

        // Access MainCamera to trigger fallback
        Camera fallbackCamera = cameraMovement.MainCamera;

        // Verify that the fallback mechanism uses Camera.main
        Assert.AreEqual(Camera.main, fallbackCamera);
    }

    [UnityTest]
    public IEnumerator TestHandleUnassignedPlayerTransform()
    {
        // Unassign playerTransform
        cameraMovement.playerTransform = null;

        yield return null; // Wait for LateUpdate to run

        // Verify that a warning is logged
        LogAssert.Expect(LogType.Warning, "Spieler-Transform ist nicht zugewiesen.");
    }

    // should be tested in EditMode!
    // [UnityTest]
    // public IEnumerator TestHandleMissingCameraComponent()
    // {
    //     // Remove the main camera component before Awake is called
    //     Object.DestroyImmediate(mainCamera);

    //     // Manually call Awake to test its behavior
    //     cameraMovement.Awake();

    //     // Verify that an error is logged
    //     LogAssert.Expect(LogType.Error, "Kamera-Komponente fehlt! Bitte eine Kamera hinzufügen.");

    //     yield return null; // Allow Unity to process the log
    // }
}
