using UnityEngine;

[CreateAssetMenu(fileName = "New Int Data", menuName = "Data/Int Data")]
public class IntData : ScriptableObject
{
    [SerializeField] public int data = 0;

    // Property to access the data value
    public int Data
    {
        get { return data; }
        set { data = value; }
    }

    // Method to set the value directly
    public void SetValue(int newValue)
    {
        data = newValue;
    }

    // Method to add a value to the current data
    public void AddValue(int valueToAdd)
    {
        data += valueToAdd;
    }

    // Method to subtract a value from the current data
    public void SubtractValue(int valueToSubtract)
    {
        data -= valueToSubtract;
    }

    // Method to multiply the current data by a value
    public void MultiplyValue(int multiplier)
    {
        data *= multiplier;
    }

    // Method to divide the current data by a value (integer division)
    public void DivideValue(int divisor)
    {
        if (divisor != 0)
        {
            data /= divisor;
        }
        else
        {
            Debug.LogWarning("Cannot divide by zero!");
        }
    }

    // Method to get the modulo (remainder) of division
    public void ModuloValue(int divisor)
    {
        if (divisor != 0)
        {
            data %= divisor;
        }
        else
        {
            Debug.LogWarning("Cannot perform modulo with zero!");
        }
    }

    // Method to increment by 1
    public void Increment()
    {
        data++;
    }

    // Method to decrement by 1
    public void Decrement()
    {
        data--;
    }

    // Method to reset the data to zero
    public void ResetValue()
    {
        data = 0;
    }

    // Method to reset the data to a specific value
    public void ResetToValue(int resetValue)
    {
        data = resetValue;
    }

    // Method to clamp the value between a min and max
    public void ClampValue(int minValue, int maxValue)
    {
        data = Mathf.Clamp(data, minValue, maxValue);
    }

    // Method to get absolute value
    public void AbsoluteValue()
    {
        data = Mathf.Abs(data);
    }

    // Method to raise to a power (using Mathf.Pow and converting back to int)
    public void PowerValue(int exponent)
    {
        data = Mathf.RoundToInt(Mathf.Pow(data, exponent));
    }

    // Override ToString for easier debugging
    public override string ToString()
    {
        return $"IntData: {data}";
    }
}