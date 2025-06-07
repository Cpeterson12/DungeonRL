using UnityEngine;
using System.Collections.Generic;

public class CardStackManager : MonoBehaviour
{
    [Header("Stacking Settings")]
    [SerializeField] private Vector2 stackOffset = new Vector2(2f, -2f);
    [SerializeField] private int maxVisualStacks = 3;
    
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
        
        // Find the card's position in the stack
        int cardIndex = stackedCards.IndexOf(cardHandler);
        
        // Remove the card
        stackedCards.RemoveAt(cardIndex);
        
        // Make sure the removed card is visible (in case it was hidden)
        cardHandler.gameObject.SetActive(true);
        
        // If we removed the last card and there are still cards, unlock the type
        if (!HasCards)
        {
            lockedCardType = "";
        }
        
        // Update the remaining stack
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
        
        // Hide all cards except the top one
        for (int i = 0; i < stackedCards.Count - 1; i++)
        {
            if (stackedCards[i] != null)
            {
                stackedCards[i].gameObject.SetActive(false);
            }
        }
        
        // Make sure the top card is visible and positioned correctly
        if (stackedCards.Count > 0 && stackedCards[stackedCards.Count - 1] != null)
        {
            CardDragHandler topCard = stackedCards[stackedCards.Count - 1];
            topCard.gameObject.SetActive(true);
            UpdateTopCardDisplay(topCard);
            PositionTopCard(topCard);
        }
    }

    private void UpdateTopCardDisplay(CardDragHandler topCard)
    {
        CardDisplay cardDisplay = topCard.GetComponent<CardDisplay>();
        if (cardDisplay?.GetCardData() != null)
        {
            cardDisplay.GetCardData().stackLevel = StackSize;
            cardDisplay.RefreshDisplay();
        }
    }

    private void PositionTopCard(CardDragHandler topCard)
    {
        if (parentDropZone == null) return;
        
        Vector2 stackPosition = GetStackSnapPosition();
        RectTransform cardRect = topCard.GetComponent<RectTransform>();
        if (cardRect != null)
        {
            cardRect.anchoredPosition = stackPosition;
        }
    }

    public Vector2 GetStackSnapPosition()
    {
        if (parentDropZone == null) return Vector2.zero;
        
        RectTransform dropZoneRect = parentDropZone.GetComponent<RectTransform>();
        Vector2 basePosition = dropZoneRect.anchoredPosition + parentDropZone.SnapOffset;
        
        return basePosition + (stackOffset * Mathf.Min(StackSize, maxVisualStacks));
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