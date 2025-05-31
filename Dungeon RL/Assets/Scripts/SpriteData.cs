using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Sprite Data", menuName = "Data/Sprite Data")]
public class SpriteData : ScriptableObject
{
    [SerializeField] private Sprite currentSprite;
    [SerializeField] private List<Sprite> spriteCollection = new List<Sprite>();
    [SerializeField] private int currentIndex = 0;
    
    // Property to access the current sprite
    public Sprite CurrentSprite
    {
        get { return currentSprite; }
        set { currentSprite = value; }
    }
    
    // Property to get the current index in the collection
    public int CurrentIndex
    {
        get { return currentIndex; }
        set 
        { 
            if (spriteCollection.Count > 0)
            {
                currentIndex = Mathf.Clamp(value, 0, spriteCollection.Count - 1);
                currentSprite = spriteCollection[currentIndex];
            }
        }
    }
    
    // Property to get the collection count
    public int CollectionCount
    {
        get { return spriteCollection.Count; }
    }
    
    // Method to set the current sprite directly
    public void SetSprite(Sprite newSprite)
    {
        currentSprite = newSprite;
    }
    
    // Method to add a sprite to the collection
    public void AddToCollection(Sprite sprite)
    {
        if (sprite != null && !spriteCollection.Contains(sprite))
        {
            spriteCollection.Add(sprite);
        }
    }
    
    // Method to remove a sprite from the collection
    public void RemoveFromCollection(Sprite sprite)
    {
        if (sprite != null && spriteCollection.Contains(sprite))
        {
            int removedIndex = spriteCollection.IndexOf(sprite);
            spriteCollection.Remove(sprite);
            
            // Adjust current index if needed
            if (currentIndex >= spriteCollection.Count && spriteCollection.Count > 0)
            {
                currentIndex = spriteCollection.Count - 1;
                currentSprite = spriteCollection[currentIndex];
            }
            else if (spriteCollection.Count == 0)
            {
                currentIndex = 0;
                currentSprite = null;
            }
        }
    }
    
    // Method to set sprite by index in collection
    public void SetSpriteByIndex(int index)
    {
        if (spriteCollection.Count > 0 && index >= 0 && index < spriteCollection.Count)
        {
            currentIndex = index;
            currentSprite = spriteCollection[currentIndex];
        }
        else
        {
            Debug.LogWarning("Invalid sprite index!");
        }
    }
    
    // Method to get next sprite in collection
    public void NextSprite()
    {
        if (spriteCollection.Count > 0)
        {
            currentIndex = (currentIndex + 1) % spriteCollection.Count;
            currentSprite = spriteCollection[currentIndex];
        }
    }
    
    // Method to get previous sprite in collection
    public void PreviousSprite()
    {
        if (spriteCollection.Count > 0)
        {
            currentIndex = (currentIndex - 1 + spriteCollection.Count) % spriteCollection.Count;
            currentSprite = spriteCollection[currentIndex];
        }
    }
    
    // Method to get a random sprite from collection
    public void RandomSprite()
    {
        if (spriteCollection.Count > 0)
        {
            currentIndex = Random.Range(0, spriteCollection.Count);
            currentSprite = spriteCollection[currentIndex];
        }
    }
    
    // Method to reset to first sprite in collection
    public void ResetToFirst()
    {
        if (spriteCollection.Count > 0)
        {
            currentIndex = 0;
            currentSprite = spriteCollection[currentIndex];
        }
    }
    
    // Method to reset to last sprite in collection
    public void ResetToLast()
    {
        if (spriteCollection.Count > 0)
        {
            currentIndex = spriteCollection.Count - 1;
            currentSprite = spriteCollection[currentIndex];
        }
    }
    
    // Method to find sprite by name in collection
    public bool SetSpriteByName(string spriteName)
    {
        for (int i = 0; i < spriteCollection.Count; i++)
        {
            if (spriteCollection[i] != null && spriteCollection[i].name == spriteName)
            {
                currentIndex = i;
                currentSprite = spriteCollection[currentIndex];
                return true;
            }
        }
        Debug.LogWarning($"Sprite with name '{spriteName}' not found in collection!");
        return false;
    }
    
    // Method to check if collection contains a specific sprite
    public bool ContainsSprite(Sprite sprite)
    {
        return sprite != null && spriteCollection.Contains(sprite);
    }
    
    // Method to clear the collection
    public void ClearCollection()
    {
        spriteCollection.Clear();
        currentIndex = 0;
        currentSprite = null;
    }
    
    // Method to get sprite at specific index without changing current
    public Sprite GetSpriteAtIndex(int index)
    {
        if (index >= 0 && index < spriteCollection.Count)
        {
            return spriteCollection[index];
        }
        return null;
    }
    
    // Method to shuffle the collection
    public void ShuffleCollection()
    {
        for (int i = 0; i < spriteCollection.Count; i++)
        {
            Sprite temp = spriteCollection[i];
            int randomIndex = Random.Range(i, spriteCollection.Count);
            spriteCollection[i] = spriteCollection[randomIndex];
            spriteCollection[randomIndex] = temp;
        }
        
        // Reset to first sprite after shuffle
        ResetToFirst();
    }
    
    // Method to check if current sprite is valid
    public bool HasValidSprite()
    {
        return currentSprite != null;
    }
    
    // Override ToString for easier debugging
    public override string ToString()
    {
        string spriteName = currentSprite != null ? currentSprite.name : "null";
        return $"SpriteData: {spriteName} (Index: {currentIndex}/{spriteCollection.Count})";
    }
}