using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hit = new List<ARRaycastHit>();
    [SerializeField]
    GameObject spawnablePrefab;

    public Camera arCam;
    GameObject spawnedObject;

    private Vector3 originalScale;
    private Vector3 initialScaleAtPinch;
    private float initialDistanceBetweenFingers;

    private Vector2 initialTouchPosition;  // For detecting touch drag
    private float rotationSpeed = 0.2f;    // Adjust rotation sensitivity

    // UI Buttons for color change
    public Button roseButton;
    public Button lavenderButton;
    public Button pinkButton;

    void Start()
    {
        spawnedObject = null;

        // Set up button listeners
        roseButton.onClick.AddListener(ChangeToRose);
        lavenderButton.onClick.AddListener(ChangeToLavender);
        pinkButton.onClick.AddListener(ChangeToPink);
    }

    void Update()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
    }

    private void HandleTouchInput()
    {
        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hit))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        spawnedObject = hit.collider.gameObject;
                        initialTouchPosition = Input.GetTouch(0).position;  // Record touch position when interacting with object
                    }
                    else if (spawnedObject == null)
                    {
                        SpawnPrefab(m_Hit[0].pose.position);
                    }
                }
            }

            if (Input.touchCount == 2 && spawnedObject != null)
            {
                HandlePinchToScale();
            }

            // Handle object rotation when dragging
            if (spawnedObject != null && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                HandleTouchRotation();
            }
        }
    }

    private void HandlePinchToScale()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
        {
            initialDistanceBetweenFingers = Vector2.Distance(touch1.position, touch2.position);
            initialScaleAtPinch = spawnedObject.transform.localScale;
        }
        else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
            float currentDistanceBetweenFingers = Vector2.Distance(touch1.position, touch2.position);
            float scaleFactor = currentDistanceBetweenFingers / initialDistanceBetweenFingers;
            spawnedObject.transform.localScale = initialScaleAtPinch * scaleFactor;
        }
    }

    private void HandleTouchRotation()
    {
        Vector2 currentTouchPosition = Input.GetTouch(0).position;
        Vector2 touchDelta = currentTouchPosition - initialTouchPosition;

        float rotationX = touchDelta.y * rotationSpeed; // Rotate along the x-axis (vertical drag)
        float rotationY = -touchDelta.x * rotationSpeed; // Rotate along the y-axis (horizontal drag)

        spawnedObject.transform.Rotate(rotationX, rotationY, 0, Space.World);

        initialTouchPosition = currentTouchPosition; // Update the last touch position
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        // Set the y-coordinate to 0.6f while keeping the x and z the same
        spawnPosition.y = 0.6f;

        // Instantiate the prefab at the modified position
        spawnedObject = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);

        // Store the original scale
        originalScale = spawnedObject.transform.localScale;
    }

    // Methods to change the color of the spawned object
    private void ChangeToRose()
    {
        ChangeCubeColor(new Color(1.0f, 0.5f, 0.5f)); // Rose color
    }

    private void ChangeToLavender()
    {
        ChangeCubeColor(new Color(0.8f, 0.6f, 0.9f)); // Lavender color
    }

    private void ChangeToPink()
    {
        ChangeCubeColor(new Color(1.0f, 0.75f, 0.8f)); // Pink color
    }

    private void ChangeCubeColor(Color color)
    {
        if (spawnedObject != null)
        {
            Renderer renderer = spawnedObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color; // Apply color to the spawned object
            }
            else
            {
                Debug.LogWarning("No Renderer found on the spawned object.");
            }
        }
        else
        {
            Debug.LogWarning("No spawned object to change color.");
        }
    }
}
