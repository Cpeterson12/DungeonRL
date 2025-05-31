using UnityEngine;

[CreateAssetMenu(fileName = "New Float Data", menuName = "Data/Float Data")]
public class FloatData : ScriptableObject
{
    [SerializeField] public float data = 0f;
    
    // Property to access the data value
    public float Data
    {
        get { return data; }
        set { data = value; }
    }
    
    // Method to set the value directly
    public void SetValue(float newValue)
    {
        data = newValue;
    }
    
    // Method to add a value to the current data
    public void AddValue(float valueToAdd)
    {
        data += valueToAdd;
    }
    
    // Method to subtract a value from the current data
    public void SubtractValue(float valueToSubtract)
    {
        data -= valueToSubtract;
    }
    
    // Method to multiply the current data by a value
    public void MultiplyValue(float multiplier)
    {
        data *= multiplier;
    }
    
    // Method to divide the current data by a value
    public void DivideValue(float divisor)
    {
        if (divisor != 0f)
        {
            data /= divisor;
        }
        else
        {
            Debug.LogWarning("Cannot divide by zero!");
        }
    }
    
    // Method to reset the data to zero
    public void ResetValue()
    {
        data = 0f;
    }
    
    // Method to reset the data to a specific value
    public void ResetToValue(float resetValue)
    {
        data = resetValue;
    }
    
    // Method to clamp the value between a min and max
    public void ClampValue(float minValue, float maxValue)
    {
        data = Mathf.Clamp(data, minValue, maxValue);
    }
    
    // Override ToString for easier debugging
    public override string ToString()
    {
        return $"FloatData: {data}";
    }
}