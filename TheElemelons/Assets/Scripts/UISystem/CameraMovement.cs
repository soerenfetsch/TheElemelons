using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Referenz auf den Spieler, dem die Kamera folgen soll
    public Transform playerTransform;

    // Standardkamera-Größe, falls keine Anpassung erforderlich ist
    public float defaultCameraSize = 5f;
    public float minCameraSize = 3f;
    public float maxCameraSize = 10f;

    // Kamera-Komponente, um die Auflösung und die Position zu kontrollieren
    private Camera mainCamera;

    // Property mit Fallback (Kurzform von Getter and Setter)
    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("MainCamera wurde nicht initialisiert. Fallback auf Camera.main.");
                mainCamera = Camera.main; // Fallback auf Hauptkamera (Ersatzwert wenn nicht vorhanden)
            }
            return mainCamera;
        }
    }


    void Awake()
    {
        // Holen der Kamera-Komponente von diesem GameObject
        mainCamera = GetComponent<Camera>();

        // Setzen der Standardgröße der Kamera
        if (mainCamera != null)
        {
            mainCamera.orthographicSize = defaultCameraSize;
        }
        else
        {
            Debug.LogError("Kamera-Komponente fehlt! Bitte eine Kamera hinzufügen.");
        }

        // Spieler-Transform überprüfen
        if (playerTransform == null)
        {
            Debug.LogWarning("Spieler-Transform ist nicht zugewiesen.");
        }
    }

    void LateUpdate()
    {
        // Sicherstellen, dass die Kamera auf den Spieler zentriert ist
        if (playerTransform != null)
        {
            Vector3 newCameraPosition = playerTransform.position;
            newCameraPosition.z = -10f; // Z-Achse fixieren, da Kamera in 2D nach unten schauen sollte
            transform.position = newCameraPosition;
        }
        else
        {
            Debug.LogWarning("Spieler-Transform ist nicht zugewiesen.");
        }
    }

    // Methode zum Anpassen der Kameraauflösung.
    public void SetCameraSize(float newSize)
    {
        if (mainCamera != null)
        {
            // Kameragröße auf die erlaubten Werte begrenzen
            newSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
            mainCamera.orthographicSize = newSize;
            Debug.Log($"Kameragröße wurde auf {newSize} gesetzt.");
        }
        else
        {
            Debug.LogError("Kamera-Komponente nicht gefunden!");
        }
    }
}
