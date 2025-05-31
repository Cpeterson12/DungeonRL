using UnityEngine;

[System.Serializable]
public enum CardType
{
    Loot,
    Monster,
    Skill,
    Event
}

[CreateAssetMenu(fileName = "New Card", menuName = "Card Game/Base Card")]
public abstract class BaseCard : ScriptableObject
{
    [Header("Basic Card Info")]
    public StringData cardName;
    public StringData description;
    public SpriteData cardArt;
    public CardType cardType;
    public IntData goldValue;
    
    [Header("Visual")]
    public SpriteData cardBorderSprite;
    public SpriteData cardBackgroundSprite;
    
    // Abstract method that each card type will implement differently
    public abstract void ExecuteCardEffect();
    
    // Virtual method for card validation (can be overridden)
    public virtual bool CanUseCard()
    {
        return true;
    }
    
    // Helper method to get card info as formatted string
    public virtual string GetCardInfo()
    {
        string name = cardName != null ? cardName.data : "Unnamed Card";
        string desc = description != null ? description.data : "No description";
        int gold = goldValue != null ? goldValue.data : 0;
        
        return $"{name}\nValue: {gold} gold\n{desc}";
    }
}