using UnityEngine;

/// <summary>
/// Handles camera movement and size adjustments in a 2D game.
/// Follows the player and allows zooming within defined limits.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    /// <summary>
    /// Reference to the player transform for camera tracking.
    /// </summary>
    public Transform playerTransform;

    /// <summary>
    /// Default size of the camera.
    /// </summary>
    public float defaultCameraSize = 5f;

    /// <summary>
    /// Minimum allowed size for the camera.
    /// </summary>
    public float minCameraSize = 3f;

    /// <summary>
    /// Maximum allowed size for the camera.
    /// </summary>
    public float maxCameraSize = 10f;

    /// <summary>
    /// Reference to the main camera component.
    /// </summary>
    private Camera mainCamera;

    /// <summary>
    /// Provides access to the main camera with a fallback to Camera.main if not initialized.
    /// </summary>
    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("MainCamera not initialized. Falling back to Camera.main.");
                mainCamera = Camera.main;
            }
            return mainCamera;
        }
    }

    /// <summary>
    /// Unity Awake method. Initializes the camera and checks for required references.
    /// </summary>
    void Awake()
    {
        // Retrieve the Camera component from this GameObject
        mainCamera = GetComponent<Camera>();

        // Set the default camera size
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = defaultCameraSize;
        }
        else
        {
            Debug.LogError("Camera component missing! Please attach a Camera component.");
        }

        // Check for the player transform
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform is not assigned.");
        }
    }

    /// <summary>
    /// Unity LateUpdate method. Ensures the camera follows the player.
    /// </summary>
    void LateUpdate()
    {
        if (playerTransform != null)
        {
            // Center the camera on the player
            Vector3 newCameraPosition = playerTransform.position;
            newCameraPosition.z = -10f; // Fix the Z-axis for 2D view
            transform.position = newCameraPosition;
        }
        else
        {
            Debug.LogWarning("Player Transform is not assigned.");
        }
    }

    /// <summary>
    /// Adjusts the camera size within the defined limits.
    /// </summary>
    /// <param name="newSize">The new size to set for the camera.</param>
    public void SetCameraSize(float newSize)
    {
        if (mainCamera != null)
        {
            // Clamp the camera size to the allowed range
            newSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
            mainCamera.orthographicSize = newSize;
            Debug.Log($"Camera size set to {newSize}.");
        }
        else
        {
            Debug.LogError("Camera component not found!");
        }
    }
}
