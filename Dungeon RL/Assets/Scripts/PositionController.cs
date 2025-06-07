using UnityEngine;

public class PositionController : MonoBehaviour
{
    [Header("Position Data")]
    [SerializeField] private Vector2Data positionData;
    
    [Header("Settings")]
    [SerializeField] private bool useX = true;
    [SerializeField] private bool useY = true;
    [SerializeField] private bool isUI = true; // Use RectTransform instead of Transform
    
    // Cached components
    private RectTransform rectTransform;
    private Transform myTransform;

    private void Awake()
    {
        // Cache the appropriate transform component
        if (isUI)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        else
        {
            myTransform = transform;
        }
    }

    private void Update()
    {
        if (positionData != null)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        if (isUI && rectTransform != null)
        {
            // Update UI element position using RectTransform
            Vector2 currentPos = rectTransform.anchoredPosition;
            Vector2 newPos = new Vector2(
                useX ? positionData.X : currentPos.x,
                useY ? positionData.Y : currentPos.y
            );
            rectTransform.anchoredPosition = newPos;
        }
        else if (!isUI && myTransform != null)
        {
            // Update world space position using Transform
            Vector3 currentPos = myTransform.position;
            Vector3 newPos = new Vector3(
                useX ? positionData.X : currentPos.x,
                useY ? positionData.Y : currentPos.y,
                currentPos.z
            );
            myTransform.position = newPos;
        }
    }

    // Method to set position data from external scripts
    public void SetPositionData(Vector2Data newData)
    {
        positionData = newData;
    }

    // Method to immediately apply position (useful for initialization)
    public void ApplyPosition()
    {
        if (positionData != null)
        {
            UpdatePosition();
        }
    }
}