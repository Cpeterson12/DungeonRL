using UnityEngine;

[CreateAssetMenu(fileName = "New String Data", menuName = "Data/String Data")]
public class StringData : ScriptableObject
{
    [SerializeField] public string data = "";
    
    // Property to access the data value
    public string Data
    {
        get { return data; }
        set { data = value ?? ""; } // Prevent null assignment
    }
    
    // Property to get the length of the string
    public int Length
    {
        get { return data.Length; }
    }
    
    // Method to set the value directly
    public void SetValue(string newValue)
    {
        data = newValue ?? "";
    }
    
    // Method to append text to the end of the string
    public void AppendText(string textToAdd)
    {
        if (textToAdd != null)
        {
            data += textToAdd;
        }
    }
    
    // Method to prepend text to the beginning of the string
    public void PrependText(string textToAdd)
    {
        if (textToAdd != null)
        {
            data = textToAdd + data;
        }
    }
    
    // Method to insert text at a specific position
    public void InsertText(int index, string textToInsert)
    {
        if (textToInsert != null && index >= 0 && index <= data.Length)
        {
            data = data.Insert(index, textToInsert);
        }
        else
        {
            Debug.LogWarning("Invalid index or null text for insertion!");
        }
    }
    
    // Method to remove a specific substring
    public void RemoveText(string textToRemove)
    {
        if (!string.IsNullOrEmpty(textToRemove))
        {
            data = data.Replace(textToRemove, "");
        }
    }
    
    // Method to remove characters from the end
    public void RemoveFromEnd(int charactersToRemove)
    {
        if (charactersToRemove > 0 && charactersToRemove <= data.Length)
        {
            data = data.Substring(0, data.Length - charactersToRemove);
        }
        else if (charactersToRemove > data.Length)
        {
            data = "";
        }
    }
    
    // Method to remove characters from the beginning
    public void RemoveFromBeginning(int charactersToRemove)
    {
        if (charactersToRemove > 0 && charactersToRemove <= data.Length)
        {
            data = data.Substring(charactersToRemove);
        }
        else if (charactersToRemove >= data.Length)
        {
            data = "";
        }
    }
    
    // Method to replace text with other text
    public void ReplaceText(string oldText, string newText)
    {
        if (!string.IsNullOrEmpty(oldText))
        {
            data = data.Replace(oldText, newText ?? "");
        }
    }
    
    // Method to convert to uppercase
    public void ToUpperCase()
    {
        data = data.ToUpper();
    }
    
    // Method to convert to lowercase
    public void ToLowerCase()
    {
        data = data.ToLower();
    }
    
    // Method to trim whitespace from both ends
    public void TrimWhitespace()
    {
        data = data.Trim();
    }
    
    // Method to clear/reset the string
    public void Clear()
    {
        data = "";
    }
    
    // Method to reset to a specific value
    public void ResetToValue(string resetValue)
    {
        data = resetValue ?? "";
    }
    
    // Method to check if string contains specific text
    public bool Contains(string searchText)
    {
        return !string.IsNullOrEmpty(searchText) && data.Contains(searchText);
    }
    
    // Method to check if string starts with specific text
    public bool StartsWith(string prefix)
    {
        return !string.IsNullOrEmpty(prefix) && data.StartsWith(prefix);
    }
    
    // Method to check if string ends with specific text
    public bool EndsWith(string suffix)
    {
        return !string.IsNullOrEmpty(suffix) && data.EndsWith(suffix);
    }
    
    // Method to check if string is empty or null
    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(data);
    }
    
    // Method to reverse the string
    public void ReverseString()
    {
        char[] charArray = data.ToCharArray();
        System.Array.Reverse(charArray);
        data = new string(charArray);
    }
    
    // Method to limit string to maximum length
    public void LimitLength(int maxLength)
    {
        if (maxLength >= 0 && data.Length > maxLength)
        {
            data = data.Substring(0, maxLength);
        }
    }
    
    // Override ToString for easier debugging
    public override string ToString()
    {
        return $"StringData: \"{data}\"";
    }
}