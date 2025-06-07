using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour
{
    [Header("Drop Zone Settings")]
    [SerializeField] private string zoneName = "Drop Zone";
    [SerializeField] private bool acceptAnyCard = true;
    [SerializeField] private Vector2 snapOffset = Vector2.zero;
    
    [Header("Visual Feedback")]
    [SerializeField] private bool showVisualFeedback = true;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.1f);
    [SerializeField] private Color hoverColor = new Color(0f, 1f, 0f, 0.3f);
    [SerializeField] private Vector2 zoneSize = new Vector2(200f, 300f);
    
    [Header("Events")]
    [SerializeField] private GameAction onCardDropped;
    [SerializeField] private GameAction onCardEnterZone;
    [SerializeField] private GameAction onCardExitZone;
    
    [Header("Starting Cards")]
    [SerializeField] private GameObject[] startingCards;
    
    // Components and state
    private RectTransform rectTransform;
    private Image backgroundImage;
    private bool isCardHovering;
    
    // Properties
    public string ZoneName => zoneName;
    public bool AcceptAnyCard => acceptAnyCard;
    public Vector2 SnapPosition => rectTransform.anchoredPosition + snapOffset;
    public bool IsCardHovering => isCardHovering;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = zoneSize;
        SetupVisualFeedback();
    }

    private void Start()
    {
        AssignStartingCards();
    }

    private void SetupVisualFeedback()
    {
        backgroundImage = GetComponent<Image>() ?? gameObject.AddComponent<Image>();
        backgroundImage.color = showVisualFeedback ? normalColor : Color.clear;
        backgroundImage.raycastTarget = true;
        
        // Create simple white sprite if needed
        if (backgroundImage.sprite == null)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            backgroundImage.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
        }
    }

    private void AssignStartingCards()
    {
        foreach (GameObject cardObj in startingCards)
        {
            if (cardObj?.GetComponent<CardDragHandler>() is CardDragHandler cardHandler)
            {
                AssignCardToZone(cardHandler);
            }
        }
    }

    private void AssignCardToZone(CardDragHandler cardHandler)
    {
        Vector2 snapPosition = GetSnapPosition();
        cardHandler.GetComponent<RectTransform>().anchoredPosition = snapPosition;
        cardHandler.SetOriginalPosition(snapPosition);
        cardHandler.AssignToDropZone(this); // Make sure card follows this zone
    }

    public bool CanAcceptCard(GameObject card) => acceptAnyCard;

    public Vector2 GetSnapPosition() => rectTransform.anchoredPosition + snapOffset;

    public void OnCardEnter(GameObject card)
    {
        if (!CanAcceptCard(card)) return;
        
        isCardHovering = true;
        UpdateVisualState(hoverColor);
        onCardEnterZone?.RaiseAction(card);
    }

    public void OnCardExit(GameObject card)
    {
        isCardHovering = false;
        UpdateVisualState(showVisualFeedback ? normalColor : Color.clear);
        onCardExitZone?.RaiseAction(card);
    }

    public bool TryDropCard(GameObject card)
    {
        if (!CanAcceptCard(card)) return false;
        
        onCardDropped?.RaiseAction(card);
        OnCardExit(card); // Reset visual state
        return true;
    }

    private void UpdateVisualState(Color color)
    {
        if (backgroundImage != null)
            backgroundImage.color = color;
    }

    public void SetZoneSize(Vector2 newSize)
    {
        zoneSize = newSize;
        if (rectTransform != null)
            rectTransform.sizeDelta = zoneSize;
    }

    public void SetVisualFeedback(bool enabled)
    {
        showVisualFeedback = enabled;
        UpdateVisualState(enabled ? normalColor : Color.clear);
    }

    public void AssignCard(GameObject cardObject)
    {
        if (cardObject?.GetComponent<CardDragHandler>() is CardDragHandler cardHandler)
        {
            AssignCardToZone(cardHandler);
        }
    }

    // Debug visualization
    private void OnDrawGizmos()
    {
        if (rectTransform == null) return;
        
        Gizmos.color = isCardHovering ? Color.green : Color.white;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(zoneSize.x, zoneSize.y, 1f);
        Gizmos.DrawWireCube(center, size);
    }

    private void OnDrawGizmosSelected()
    {
        if (rectTransform == null) return;
        
        // Draw zone area
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Vector3 center = transform.position;
        Vector3 size = new Vector3(zoneSize.x, zoneSize.y, 1f);
        Gizmos.DrawCube(center, size);
        
        // Draw snap position
        Gizmos.color = Color.red;
        Vector3 snapPos = center + new Vector3(snapOffset.x, snapOffset.y, 0f);
        Gizmos.DrawWireSphere(snapPos, 10f);
    }
}