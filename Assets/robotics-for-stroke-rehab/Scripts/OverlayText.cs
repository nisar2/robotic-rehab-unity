using UnityEngine;
using UnityEngine.UI; // Or use TMPro if you're using TextMeshPro
using TMPro;

public class OverlayText : MonoBehaviour
{
    public Transform targetObject; // The 3D object in the scene
    public TextMeshProUGUI uiText; // The UI Text element (for TextMeshPro)
    // public Text uiText; // Use this line if you're using Unity's default Text component

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Convert the 3D position of the object to a screen position
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetObject.position);

        // Check if the object is in front of the camera
        if (screenPosition.z > 0)
        {
            // Set the position of the UI text element
            uiText.transform.position = screenPosition;
        }
        else
        {
            // Optionally, you can hide the UI text element if the object is behind the camera
            uiText.transform.position = new Vector3(-1000, -1000, 0); // Move it offscreen
        }
    }
}
