using UnityEngine;
using System.Collections.Generic;

public class CardStackManager : MonoBehaviour
{
    [Header("Stacking Settings")]
    [SerializeField] private Vector2 stackOffset = new Vector2(8f, -8f);
    [SerializeField] private Vector2 maxStackBounds = new Vector2(150f, 200f); // Max area cards can spread within
    [SerializeField] private float minOffsetMultiplier = 0.3f; // How much to compress when many cards
    
    [Header("Events")]
    [SerializeField] private GameAction onCardStacked;
    [SerializeField] private GameAction onCardRejected;
    
    // State tracking
    private List<CardDragHandler> stackedCards = new List<CardDragHandler>();
    private string lockedCardType = "";
    private DropZone parentDropZone;
    
    // Properties
    public bool HasCards => stackedCards.Count > 0;
    public int StackSize => stackedCards.Count;
    public string CardType => lockedCardType;
    public bool IsLocked => !string.IsNullOrEmpty(lockedCardType);

    private void Awake()
    {
        parentDropZone = GetComponent<DropZone>();
    }

    public bool CanAcceptCard(GameObject cardObject)
    {
        CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
        if (cardDisplay == null) return false;
        
        LootCardData cardData = cardDisplay.GetCardData();
        if (cardData == null) return false;
        
        // If stack is empty, accept any card
        if (!HasCards) return true;
        
        // Check if card types match
        return cardData.cardID == lockedCardType;
    }

    public bool TryAddCard(CardDragHandler cardHandler)
    {
        if (!CanAcceptCard(cardHandler.gameObject))
        {
            onCardRejected?.RaiseAction(cardHandler.gameObject);
            return false;
        }
        
        CardDisplay cardDisplay = cardHandler.GetComponent<CardDisplay>();
        LootCardData cardData = cardDisplay.GetCardData();
        
        // Lock to this card type if first card
        if (!HasCards)
        {
            lockedCardType = cardData.cardID;
        }
        
        // Add to stack
        stackedCards.Add(cardHandler);
        UpdateStackVisuals();
        
        onCardStacked?.RaiseAction(cardHandler.gameObject);
        Debug.Log($"Card '{cardData.cardName}' stacked. Stack size: {StackSize}");
        
        return true;
    }

    // Remove a specific card from the stack (called when dragging starts)
    public void RemoveCardFromStack(CardDragHandler cardHandler)
    {
        if (!stackedCards.Contains(cardHandler)) return;
        
        // If this is the only card, clear the stack completely
        if (StackSize == 1)
        {
            ClearStack();
            return;
        }
        
        // Remove the card from the stack
        stackedCards.Remove(cardHandler);
        
        // Make sure the removed card is visible and reset its layer
        cardHandler.gameObject.SetActive(true);
        cardHandler.transform.SetAsLastSibling(); // Bring to front while dragging
        
        // If we removed the last card and there are still cards, unlock the type
        if (!HasCards)
        {
            lockedCardType = "";
        }
        
        // Update the remaining stack visuals
        UpdateStackVisuals();
        
        Debug.Log($"Card removed from stack. Remaining size: {StackSize}");
    }

    // Legacy method for compatibility - now delegates to RemoveCardFromStack
    public void RemoveCard(CardDragHandler cardHandler)
    {
        RemoveCardFromStack(cardHandler);
    }

    public void ClearStack()
    {
        // Make all cards visible again
        foreach (CardDragHandler cardHandler in stackedCards)
        {
            if (cardHandler != null)
            {
                cardHandler.gameObject.SetActive(true);
            }
        }
        
        stackedCards.Clear();
        lockedCardType = "";
        Debug.Log("Stack cleared");
    }

    private void UpdateStackVisuals()
    {
        if (StackSize == 0) return;
        
        // Calculate dynamic offset that gets smaller with more cards
        Vector2 dynamicOffset = CalculateDynamicOffset();
        
        // Position and layer all cards in the stack
        for (int i = 0; i < stackedCards.Count; i++)
        {
            if (stackedCards[i] != null)
            {
                CardDragHandler card = stackedCards[i];
                
                // Make sure all cards are visible
                card.gameObject.SetActive(true);
                
                // Set layering - higher index = on top
                card.transform.SetSiblingIndex(card.transform.parent.childCount - stackedCards.Count + i);
                
                // Position card with progressive offset
                Vector2 cardPosition = GetBaseStackPosition() + (dynamicOffset * i);
                RectTransform cardRect = card.GetComponent<RectTransform>();
                if (cardRect != null)
                {
                    cardRect.anchoredPosition = cardPosition;
                }
                
                // Update stack level display for each card
                UpdateCardDisplay(card, i + 1);
            }
        }
    }

    // Calculate offset that gets smaller as stack grows to stay within bounds
    private Vector2 CalculateDynamicOffset()
    {
        if (StackSize <= 1) return Vector2.zero;
        
        // Calculate how much we need to compress the offset
        float compressionFactor = Mathf.Lerp(1f, minOffsetMultiplier, (StackSize - 1) / 8f); // Compress over 8 cards
        
        // Make sure we don't exceed the drop zone bounds
        Vector2 maxAllowedOffset = maxStackBounds / Mathf.Max(StackSize - 1, 1);
        Vector2 compressedOffset = stackOffset * compressionFactor;
        
        // Use the smaller of the two to ensure we stay in bounds
        return new Vector2(
            Mathf.Min(Mathf.Abs(compressedOffset.x), Mathf.Abs(maxAllowedOffset.x)) * Mathf.Sign(stackOffset.x),
            Mathf.Min(Mathf.Abs(compressedOffset.y), Mathf.Abs(maxAllowedOffset.y)) * Mathf.Sign(stackOffset.y)
        );
    }

    // Get the base position for the stack (where the first card sits)
    private Vector2 GetBaseStackPosition()
    {
        if (parentDropZone == null) return Vector2.zero;
        
        RectTransform dropZoneRect = parentDropZone.GetComponent<RectTransform>();
        return dropZoneRect.anchoredPosition + parentDropZone.SnapOffset;
    }

    // Update individual card display with its position in the stack
    private void UpdateCardDisplay(CardDragHandler card, int positionInStack)
    {
        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
        if (cardDisplay?.GetCardData() != null)
        {
            // Each card shows its own position in the stack and the total stack size
            cardDisplay.GetCardData().stackLevel = StackSize;
            cardDisplay.RefreshDisplay();
        }
    }

    public Vector2 GetStackSnapPosition()
    {
        if (parentDropZone == null) return Vector2.zero;
        
        // Return where the TOP card of the stack will be positioned
        Vector2 basePosition = GetBaseStackPosition();
        Vector2 dynamicOffset = CalculateDynamicOffset();
        
        // The top card position is base + offset * (stack size - 1)
        return basePosition + (dynamicOffset * Mathf.Max(StackSize - 1, 0));
    }

    public List<LootCardData> GetAllCardData()
    {
        List<LootCardData> cardDataList = new List<LootCardData>();
        
        foreach (CardDragHandler cardHandler in stackedCards)
        {
            if (cardHandler != null)
            {
                CardDisplay cardDisplay = cardHandler.GetComponent<CardDisplay>();
                if (cardDisplay?.GetCardData() != null)
                {
                    cardDataList.Add(cardDisplay.GetCardData());
                }
            }
        }
        
        return cardDataList;
    }

    public void GetTotalBonuses(out float totalDamage, out float totalHealth, out float totalCritDamage, 
                               out int totalAttackDice, out int totalDefenseDice, out int totalSpeed)
    {
        totalDamage = 0f;
        totalHealth = 0f;
        totalCritDamage = 0f;
        totalAttackDice = 0;
        totalDefenseDice = 0;
        totalSpeed = 0;
        
        foreach (LootCardData cardData in GetAllCardData())
        {
            totalDamage += cardData.damageBonus;
            totalHealth += cardData.healthBonus;
            totalCritDamage += cardData.critDamageBonus;
            totalAttackDice += cardData.attackDiceBonus;
            totalDefenseDice += cardData.defenseDiceBonus;
            totalSpeed += cardData.speedBonus;
        }
    }
}