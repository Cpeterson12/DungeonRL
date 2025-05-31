using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Card Game/Monster Card")]
public class MonsterCard : BaseCard
{
    [Header("Monster Stats")]
    public FloatData health;
    public FloatData damage;
    public IntData attackBonus; // Added to dice roll
    public IntData defenseBonus; // Added to defense roll
    
    [Header("Monster Behavior")]
    public StringData specialAbilityDescription;
    
    [Header("Rewards")]
    public IntData bountyGold;
    
    private void OnValidate()
    {
        // Ensure this is always a Monster card type
        cardType = CardType.Monster;
    }
    
    public override void ExecuteCardEffect()
    {
        // This would be called when the monster is defeated
        Debug.Log($"{cardName.data} defeated! Earned {bountyGold.data} gold.");
        
        // TODO: In a real implementation, this would interact with GameManager
        // to actually award the gold to the player
    }
    
    public override bool CanUseCard()
    {
        // Monsters can't be "used" by the player in the traditional sense
        // But this could check if the monster should appear based on certain conditions
        return true;
    }
    
    public override string GetCardInfo()
    {
        string info = base.GetCardInfo();
        
        if (health != null)
            info += $"\nHP: {health.data}";
        if (damage != null)
            info += $"\nDamage: {damage.data}";
        if (bountyGold != null)
            info += $"\nBounty: {bountyGold.data} gold";
        
        if (specialAbilityDescription != null && !string.IsNullOrEmpty(specialAbilityDescription.data))
        {
            info += $"\nSpecial: {specialAbilityDescription.data}";
        }
        
        return info;
    }
    
    // Combat-specific methods for dice rolling
    public int RollAttack()
    {
        int diceRoll = Random.Range(1, 7); // d6
        return diceRoll + attackBonus.data;
    }
    
    public int RollDefense()
    {
        int diceRoll = Random.Range(1, 7); // d6
        return diceRoll + defenseBonus.data;
    }
    
    // Helper method to check if monster is still alive
    public bool IsAlive()
    {
        return health.data > 0;
    }
    
    // Method to take damage
    public void TakeDamage(float damageAmount)
    {
        health.SubtractValue(damageAmount);
        Debug.Log($"{cardName.data} takes {damageAmount} damage! Health: {health.data}");
        
        if (!IsAlive())
        {
            Debug.Log($"{cardName.data} is defeated!");
            ExecuteCardEffect();
        }
    }
}