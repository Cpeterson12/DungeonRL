using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardDragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Drag Settings")]
    [SerializeField] private bool isDraggable = true;
    [SerializeField] private Vector2 dragOffset = Vector2.zero;
    
    [Header("Visual Feedback")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float dragScale = 1.2f;
    [SerializeField] private float dragAlpha = 0.8f;
    
    [Header("Events")]
    [SerializeField] private GameAction onCardStartDrag;
    [SerializeField] private GameAction onCardEndDrag;
    [SerializeField] private GameAction onCardSelected;
    
    [Header("Starting Position")]
    [SerializeField] private DropZone startingDropZone;
    [SerializeField] private bool autoDetectStartingZone = true;
    
    // Cached components
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private GraphicRaycaster graphicRaycaster;
    private Camera uiCamera;
    
    // State tracking
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private int originalSiblingIndex;
    private float originalAlpha;
    private bool isDragging;
    private DropZone currentDropZone;
    private DropZone assignedDropZone;
    
    // Properties
    public bool IsDragging => isDragging;
    public bool IsDraggable { get => isDraggable; set => isDraggable = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        graphicRaycaster = parentCanvas.GetComponent<GraphicRaycaster>();
        uiCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceCamera ? parentCanvas.worldCamera : null;
        
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        originalAlpha = canvasGroup.alpha;
    }

    private void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
        originalScale = transform.localScale;
        InitializeStartingPosition();
    }
    
    private void Update()
    {
        // Only follow zone position if not dragging and card is active (not hidden in stack)
        if (!isDragging && assignedDropZone != null && gameObject.activeInHierarchy)
        {
            Vector2 targetPosition = assignedDropZone.GetSnapPosition();
            if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) > 1f)
            {
                rectTransform.anchoredPosition = targetPosition;
                originalPosition = targetPosition;
            }
        }
    }

    private void InitializeStartingPosition()
    {
        DropZone targetZone = startingDropZone ?? (autoDetectStartingZone ? FindOverlappingDropZone() : null);
        
        if (targetZone != null && targetZone.TryDropCard(gameObject))
        {
            assignedDropZone = targetZone;
            UpdateOriginalPosition();
        }
    }

    private DropZone FindOverlappingDropZone()
    {
        DropZone[] allDropZones = FindObjectsOfType<DropZone>();
        Vector3 cardCenter = rectTransform.TransformPoint(rectTransform.rect.center);
        
        foreach (DropZone dropZone in allDropZones)
        {
            RectTransform zoneRect = dropZone.GetComponent<RectTransform>();
            if (zoneRect != null && RectTransformUtility.RectangleContainsScreenPoint(zoneRect, cardCenter, uiCamera))
            {
                return dropZone;
            }
        }
        return null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        // Store current state
        originalPosition = rectTransform.anchoredPosition;
        originalSiblingIndex = transform.GetSiblingIndex();
        isDragging = true;
        
        // Remove from current zone's stack (this handles unstacking properly)
        if (assignedDropZone?.StackManager != null)
        {
            assignedDropZone.StackManager.RemoveCardFromStack(this);
        }
        assignedDropZone = null;
        
        // Apply visual feedback
        SetVisualState(dragScale, dragAlpha, true);
        
        // Raise events
        onCardSelected?.RaiseAction(gameObject);
        onCardStartDrag?.RaiseAction(gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable || !isDragging) return;
        
        // Update card position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            graphicRaycaster.transform as RectTransform, eventData.position, uiCamera, out Vector2 localPosition))
        {
            rectTransform.anchoredPosition = localPosition + dragOffset;
        }
        
        // Detect drop zones
        UpdateDropZoneDetection(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        isDragging = false;
        
        // Try to drop in a zone, otherwise return to original position
        if (!TryDropInCurrentZone())
        {
            SnapToPosition(originalPosition);
        }
        
        // Clean up and restore visual state
        CleanupDropZone();
        SetVisualState(originalScale.x, originalAlpha, false);
        onCardEndDrag?.RaiseAction(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging && isDraggable)
            transform.localScale = originalScale * hoverScale;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
            transform.localScale = originalScale;
    }

    private void UpdateDropZoneDetection(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, results);
        
        DropZone newDropZone = null;
        foreach (RaycastResult result in results)
        {
            DropZone dropZone = result.gameObject.GetComponentInParent<DropZone>();
            if (dropZone != null)
            {
                newDropZone = dropZone;
                break;
            }
        }
        
        // Handle zone transitions
        if (newDropZone != currentDropZone)
        {
            currentDropZone?.OnCardExit(gameObject);
            currentDropZone = newDropZone;
            currentDropZone?.OnCardEnter(gameObject);
        }
    }

    private bool TryDropInCurrentZone()
    {
        if (currentDropZone != null && currentDropZone.TryDropCard(gameObject))
        {
            assignedDropZone = currentDropZone;
            UpdateOriginalPosition();
            SnapToPosition(currentDropZone.GetSnapPosition());
            return true;
        }
        return false;
    }

    private void CleanupDropZone()
    {
        if (currentDropZone != null)
        {
            currentDropZone.OnCardExit(gameObject);
            currentDropZone = null;
        }
    }

    private void SetVisualState(float scale, float alpha, bool toFront)
    {
        transform.localScale = originalScale * scale;
        canvasGroup.alpha = alpha;
        
        if (toFront)
            transform.SetAsLastSibling();
        else if (Vector2.Distance(rectTransform.anchoredPosition, originalPosition) < 10f)
            transform.SetSiblingIndex(originalSiblingIndex);
    }

    private void SnapToPosition(Vector2 targetPosition)
    {
        StartCoroutine(AnimateToPosition(targetPosition));
    }
    
    private IEnumerator AnimateToPosition(Vector2 targetPosition)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsedTime = 0f;
        const float animationDuration = 0.3f;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / animationDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        rectTransform.anchoredPosition = targetPosition;
        UpdateOriginalPosition();
    }

    private void UpdateOriginalPosition()
    {
        originalPosition = rectTransform.anchoredPosition;
    }
    
    // Public utility methods
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
        canvasGroup.blocksRaycasts = draggable;
    }
    
    public void AssignToDropZone(DropZone dropZone)
    {
        if (dropZone != null)
        {
            startingDropZone = dropZone;
            assignedDropZone = dropZone;
            UpdateOriginalPosition();
        }
    }
    
    public DropZone GetAssignedDropZone() => assignedDropZone;
    
    public void UnassignFromDropZone()
    {
        if (assignedDropZone?.StackManager != null)
        {
            assignedDropZone.StackManager.RemoveCardFromStack(this);
        }
        assignedDropZone = null;
        UpdateOriginalPosition();
    }
}