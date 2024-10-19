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

    private Vector2 initialTouchPosition; 
    private float rotationSpeed = 0.2f;    

    
    public Button roseButton;
    public Button lavenderButton;
    public Button pinkButton;

    void Start()
    {
        spawnedObject = null;

        
        roseButton.onClick.AddListener(ChangeToRose);
        lavenderButton.onClick.AddListener(ChangeToLavender);
        pinkButton.onClick.AddListener(ChangeToPink);
    }

    void Update()
    {
        
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
                        initialTouchPosition = Input.GetTouch(0).position;  
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

        float rotationX = touchDelta.y * rotationSpeed; 
        float rotationY = -touchDelta.x * rotationSpeed; 

        spawnedObject.transform.Rotate(rotationX, rotationY, 0, Space.World);

        initialTouchPosition = currentTouchPosition; 
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        
        spawnPosition.y = 0.6f;

        
        spawnedObject = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);

        
        originalScale = spawnedObject.transform.localScale;
    }


    private void ChangeToRose()
    {
        ChangeCubeColor(new Color(1.0f, 0.5f, 0.5f)); 
    }

    private void ChangeToLavender()
    {
        ChangeCubeColor(new Color(0.8f, 0.6f, 0.9f)); 
    }

    private void ChangeToPink()
    {
        ChangeCubeColor(new Color(1.0f, 0.75f, 0.8f)); 
    }

    private void ChangeCubeColor(Color color)
    {
        if (spawnedObject != null)
        {
            Renderer renderer = spawnedObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color; 
            }
            else
            {
                Debug.Log("No Renderer found on the spawned object.");
            }
        }
        else
        {
            Debug.Log("No spawned object to change color.");
        }
    }
}