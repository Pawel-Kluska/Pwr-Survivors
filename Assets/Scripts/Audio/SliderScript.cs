using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderScript : MonoBehaviour
{

    public TextMeshProUGUI text;
    // Start is called before the first frame update
    
    public void OnValueChanged(float value)
    {
        text.text = (value * 100).ToString("0") + "%";
    }
}
