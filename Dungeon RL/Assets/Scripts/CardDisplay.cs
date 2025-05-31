using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI goldValueText;
    public Image cardArtImage;
    public Image cardBackgroundImage;
    public Image cardBorderImage;
    
    [Header("Monster-Specific UI (optional)")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    
    [Header("Current Card Data")]
    public BaseCard currentCard;
    
    // Method to set up the card display with ScriptableObject data
    public void SetupCard(BaseCard cardData)
    {
        currentCard = cardData;
        UpdateCardDisplay();
    }
    
    // Method to refresh the display (useful if data changes)
    public void UpdateCardDisplay()
    {
        if (currentCard == null) return;
        
        // Update basic card info
        if (cardNameText != null && currentCard.cardName != null)
            cardNameText.text = currentCard.cardName.data;
            
        if (descriptionText != null && currentCard.description != null)
            descriptionText.text = currentCard.description.data;
            
        if (goldValueText != null && currentCard.goldValue != null)
            goldValueText.text = currentCard.goldValue.data.ToString();
            
        // Update card art
        if (cardArtImage != null && currentCard.cardArt != null && currentCard.cardArt.CurrentSprite != null)
            cardArtImage.sprite = currentCard.cardArt.CurrentSprite;
            
        // Update card background
        if (cardBackgroundImage != null && currentCard.cardBackgroundSprite != null && currentCard.cardBackgroundSprite.CurrentSprite != null)
            cardBackgroundImage.sprite = currentCard.cardBackgroundSprite.CurrentSprite;
            
        // Update card border
        if (cardBorderImage != null && currentCard.cardBorderSprite != null && currentCard.cardBorderSprite.CurrentSprite != null)
            cardBorderImage.sprite = currentCard.cardBorderSprite.CurrentSprite;
        
        // If this is a monster card, update monster-specific info
        if (currentCard is MonsterCard monster)
        {
            UpdateMonsterDisplay(monster);
        }
    }
    
    private void UpdateMonsterDisplay(MonsterCard monster)
    {
        if (healthText != null && monster.health != null)
            healthText.text = monster.health.data.ToString("F0"); // F0 removes decimal places for display
            
        if (damageText != null && monster.damage != null)
            damageText.text = monster.damage.data.ToString("F0");
    }
    
    // Optional: Update display when the ScriptableObject is assigned in the inspector
    private void OnValidate()
    {
        if (currentCard != null)
        {
            UpdateCardDisplay();
        }
    }
}