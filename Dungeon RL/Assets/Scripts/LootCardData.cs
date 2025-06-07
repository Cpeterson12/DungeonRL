using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Card", menuName = "Cards/Loot Card Data")]
public class LootCardData : ScriptableObject
{
    [Header("Visual Elements")]
    public Sprite cardArt;
    public Sprite cardBackground;
    public Sprite cardBorder;
    
    [Header("Card Information")]
    public string cardName = "New Loot Card";
    public string cardID = ""; // Unique identifier for stacking (e.g., "great_sword", "heavy_armor")
    public CardType cardType = CardType.Item;
    [TextArea(2, 4)]
    public string description = "";
    public int stackLevel = 1;
    
    [Header("Player Bonuses (when card is in play)")]
    [Space(5)]
    [Tooltip("Bonus damage added to player attacks")]
    public float damageBonus = 0f;
    
    [Tooltip("Bonus health added to player")]
    public float healthBonus = 0f;
    
    [Tooltip("Multiplier for critical hit damage")]
    public float critDamageBonus = 0f;
    
    [Tooltip("Bonus added to player's attack dice rolls")]
    public int attackDiceBonus = 0;
    
    [Tooltip("Bonus added to player's defense dice rolls")]
    public int defenseDiceBonus = 0;
    
    [Tooltip("Bonus speed value for turn order")]
    public int speedBonus = 0;
    
    // Check if this card can stack with another card
    public bool CanStackWith(LootCardData otherCard)
    {
        if (otherCard == null) return false;
        
        // Cards can stack if they have the same cardID or same name and type
        return (cardID == otherCard.cardID && !string.IsNullOrEmpty(cardID)) ||
               (cardName == otherCard.cardName && cardType == otherCard.cardType);
    }
    
    // Helper method to check if this card provides any bonuses
    public bool HasAnyBonuses()
    {
        return damageBonus != 0f || 
               healthBonus != 0f || 
               critDamageBonus != 0f || 
               attackDiceBonus != 0 || 
               defenseDiceBonus != 0 || 
               speedBonus != 0;
    }
    
    // Helper method to get a summary of what this card does
    public string GetBonusSummary()
    {
        string summary = "";
        
        if (damageBonus != 0f) summary += $"Damage {(damageBonus > 0 ? "+" : "")}{damageBonus}\n";
        if (healthBonus != 0f) summary += $"Health {(healthBonus > 0 ? "+" : "")}{healthBonus}\n";
        if (critDamageBonus != 0f) summary += $"Crit Damage {(critDamageBonus > 0 ? "+" : "")}{critDamageBonus}x\n";
        if (attackDiceBonus != 0) summary += $"Attack Dice {(attackDiceBonus > 0 ? "+" : "")}{attackDiceBonus}\n";
        if (defenseDiceBonus != 0) summary += $"Defense Dice {(defenseDiceBonus > 0 ? "+" : "")}{defenseDiceBonus}\n";
        if (speedBonus != 0) summary += $"Speed {(speedBonus > 0 ? "+" : "")}{speedBonus}\n";
        
        return summary.TrimEnd('\n');
    }
    
    // Override ToString for easier debugging
    public override string ToString()
    {
        return $"{cardName} (Level {stackLevel})";
    }
}

// Enum for card types to help with stacking logic
public enum CardType
{
    Weapon,
    Armor, 
    Potion,
    Scroll,
    Accessory,
    Item
}