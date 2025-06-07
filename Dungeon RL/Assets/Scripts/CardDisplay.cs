using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("Card Data")]
    [SerializeField] private LootCardData cardData;
    
    [Header("UI Elements")]
    [SerializeField] private Image cardBackground;
    [SerializeField] private Image cardArt;
    [SerializeField] private Image cardBorder;

    [Header("TextMeshPro ")]
    [SerializeField] private TextMeshProUGUI nameTextTMP;
    [SerializeField] private TextMeshProUGUI descriptionTextTMP;
    
    [Header("Settings")]
    [SerializeField] private bool updateInRealTime = true;

    private void Start()
    {
        UpdateCardDisplay();
    }

    private void Update()
    {
        // Update in real-time for testing/development
        if (updateInRealTime)
        {
            UpdateCardDisplay();
        }
    }

    public void SetCardData(LootCardData newCardData)
    {
        cardData = newCardData;
        UpdateCardDisplay();
    }

    private void UpdateCardDisplay()
    {
        if (cardData == null) return;

        // Update visual elements
        UpdateSprite(cardBackground, cardData.cardBackground);
        UpdateSprite(cardArt, cardData.cardArt);
        UpdateSprite(cardBorder, cardData.cardBorder);

        // Update text elements
        UpdateNameText();
        UpdateDescriptionText();
    }

    private void UpdateSprite(Image imageComponent, Sprite spriteData)
    {
        if (imageComponent != null && spriteData != null)
        {
            imageComponent.sprite = spriteData;
        }
    }

    private void UpdateNameText()
    {
        string displayName = GetDisplayName();
        
        // Update TextMeshPro component
        if (nameTextTMP != null)
        {
            nameTextTMP.text = displayName;
        }
    }

    private void UpdateDescriptionText()
    {
        string displayDescription = GetDisplayDescription();
        
        // Update TextMeshPro component
        if (descriptionTextTMP != null)
        {
            descriptionTextTMP.text = displayDescription;
        }
    }

    private string GetDisplayName()
    {
        if (cardData.stackLevel > 1)
        {
            return $"{cardData.cardName} +{cardData.stackLevel - 1}";
        }
        return cardData.cardName;
    }

    private string GetDisplayDescription()
    {
        string description = cardData.description;
        
        // Add bonus summary if the card has any bonuses
        if (cardData.HasAnyBonuses())
        {
            string bonuses = cardData.GetBonusSummary();
            if (!string.IsNullOrEmpty(bonuses))
            {
                description += "\n\n" + bonuses;
            }
        }
        
        return description;
    }

    // Public method to refresh display (useful for external scripts)
    public void RefreshDisplay()
    {
        UpdateCardDisplay();
    }

    // Method to get the current card data (useful for other scripts)
    public LootCardData GetCardData()
    {
        return cardData;
    }

    // Context menu for testing in editor
    [ContextMenu("Update Card Display")]
    private void ForceUpdateDisplay()
    {
        UpdateCardDisplay();
    }
}