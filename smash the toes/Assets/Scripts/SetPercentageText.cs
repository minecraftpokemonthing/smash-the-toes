using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SetPercentageText : MonoBehaviour
{
    TextMeshProUGUI percentageText;

    // Start is called before the first frame update
    void Start()
    {
        percentageText = PercentageTextManager.instance.AssignAvailableText();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (percentageText != null)
            percentageText.text = GetComponent<PlayerLocomotion>().percentage.ToString("0.0") + "%";
    }

    public void TookDamage()
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (percentageText != null)
        {
            PercentageTextManager.instance.availableTexts.Add(percentageText);
        }
    }
}
