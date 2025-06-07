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
    [SerializeField] private Color rejectedColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private Vector2 zoneSize = new Vector2(200f, 300f);
    
    [Header("Events")]
    [SerializeField] private GameAction onCardDropped;
    [SerializeField] private GameAction onCardEnterZone;
    [SerializeField] private GameAction onCardExitZone;
    
    // Components
    private RectTransform rectTransform;
    private Image backgroundImage;
    private CardStackManager stackManager;
    private bool isCardHovering;
    
    // Properties
    public string ZoneName => zoneName;
    public bool AcceptAnyCard => acceptAnyCard;
    public bool IsCardHovering => isCardHovering;
    public CardStackManager StackManager => stackManager;
    public Vector2 SnapOffset => snapOffset;
    public bool HasCards => stackManager.HasCards;
    public int CardCount => stackManager.StackSize;
    public string CurrentCardType => stackManager.CardType;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = zoneSize;
        
        stackManager = GetComponent<CardStackManager>() ?? gameObject.AddComponent<CardStackManager>();
        SetupVisualFeedback();
    }

    private void SetupVisualFeedback()
    {
        backgroundImage = GetComponent<Image>() ?? gameObject.AddComponent<Image>();
        backgroundImage.color = showVisualFeedback ? normalColor : Color.clear;
        backgroundImage.raycastTarget = true;
        
        if (backgroundImage.sprite == null)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            backgroundImage.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
        }
    }

    public bool CanAcceptCard(GameObject card) => acceptAnyCard && stackManager.CanAcceptCard(card);

    public Vector2 GetSnapPosition() => rectTransform.anchoredPosition + snapOffset;

    public void OnCardEnter(GameObject card)
    {
        isCardHovering = true;
        
        if (CanAcceptCard(card))
        {
            UpdateVisualState(hoverColor);
            onCardEnterZone?.RaiseAction(card);
        }
        else
        {
            UpdateVisualState(rejectedColor);
        }
    }

    public void OnCardExit(GameObject card)
    {
        isCardHovering = false;
        UpdateVisualState(showVisualFeedback ? normalColor : Color.clear);
        onCardExitZone?.RaiseAction(card);
    }

    public bool TryDropCard(GameObject card)
    {
        CardDragHandler cardHandler = card.GetComponent<CardDragHandler>();
        if (cardHandler == null) return false;
        
        if (stackManager.TryAddCard(cardHandler))
        {
            onCardDropped?.RaiseAction(card);
            OnCardExit(card); // Reset visual state
            return true;
        }
        
        return false;
    }

    public void RemoveCard(CardDragHandler cardHandler) => stackManager.RemoveCardFromStack(cardHandler);

    private void UpdateVisualState(Color color)
    {
        if (backgroundImage != null)
            backgroundImage.color = color;
    }

    [ContextMenu("Clear Zone")]
    public void ClearZone() => stackManager.ClearStack();
}