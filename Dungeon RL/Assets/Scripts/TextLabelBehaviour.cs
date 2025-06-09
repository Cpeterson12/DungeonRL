using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLabelBehaviour : MonoBehaviour
{ 
    private TextMeshProUGUI label;
    public UnityEvent startEvent;

    private void Start()
    {
        label = GetComponent<TextMeshProUGUI>();
        startEvent.Invoke();
    }

    public void UpdateLabel(IntData obj)
    {
        label.text = obj.data.ToString(CultureInfo.InvariantCulture);
    }
    
    public void UpdateLabel(FloatData obj)
    {
        label.text = obj.data.ToString(CultureInfo.InvariantCulture);
    }

    
}