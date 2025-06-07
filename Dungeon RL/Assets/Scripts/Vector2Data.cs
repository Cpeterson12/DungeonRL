using UnityEngine;

[CreateAssetMenu(fileName = "New Vector2 Data", menuName = "Data/Vector2 Data")]
public class Vector2Data : ScriptableObject
{
    [SerializeField] private Vector2 data = Vector2.zero;
    
    // Property to access the data value
    public Vector2 Data
    {
        get { return data; }
        set { data = value; }
    }
    
    // Property to get/set X component
    public float X
    {
        get { return data.x; }
        set { data.x = value; }
    }
    
    // Property to get/set Y component
    public float Y
    {
        get { return data.y; }
        set { data.y = value; }
    }
    
    // Method to set the value directly
    public void SetValue(Vector2 newValue)
    {
        data = newValue;
    }
    
    // Method to set individual components
    public void SetValue(float x, float y)
    {
        data = new Vector2(x, y);
    }
    
    // Method to add a vector to the current data
    public void AddValue(Vector2 valueToAdd)
    {
        data += valueToAdd;
    }
    
    // Method to subtract a vector from the current data
    public void SubtractValue(Vector2 valueToSubtract)
    {
        data -= valueToSubtract;
    }
    
    // Method to multiply the current data by a scalar
    public void MultiplyValue(float multiplier)
    {
        data *= multiplier;
    }
    
    // Method to reset the data to zero
    public void ResetValue()
    {
        data = Vector2.zero;
    }
    
    // Method to reset the data to a specific value
    public void ResetToValue(Vector2 resetValue)
    {
        data = resetValue;
    }
    
    // Method to normalize the vector
    public void Normalize()
    {
        data = data.normalized;
    }
    
    // Method to get the magnitude
    public float GetMagnitude()
    {
        return data.magnitude;
    }
    
    // Method to clamp the magnitude
    public void ClampMagnitude(float maxLength)
    {
        data = Vector2.ClampMagnitude(data, maxLength);
    }
    
    // Override ToString for easier debugging
    public override string ToString()
    {
        return $"Vector2Data: ({data.x}, {data.y})";
    }
}