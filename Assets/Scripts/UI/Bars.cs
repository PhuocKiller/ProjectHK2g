using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bars : MonoBehaviour
{
    [SerializeField]
    Image fillBar, fadeFillBar;
    [SerializeField]
    TextMeshProUGUI valueText;


    public virtual void UpdateBar(float currentValue, float maxValue)
    {
        fillBar.fillAmount = currentValue / maxValue;
        valueText.text = ((int)currentValue).ToString() + "/" + ((int)maxValue).ToString();
    }
    public void UpdateFadeBar(float currentValue, float maxValue)
    {
        fadeFillBar.fillAmount = currentValue / maxValue;
    }
    private void Update()
    {

    }
}
