using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PercentageTextManager : MonoBehaviour
{
    public static PercentageTextManager instance;

    public List<TextMeshProUGUI> availableTexts = new List<TextMeshProUGUI>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public TextMeshProUGUI AssignAvailableText()
    {
        if (availableTexts.Count == 0)
        {
            return null;
        }

        TextMeshProUGUI text = availableTexts[0];
        availableTexts.RemoveAt(0);
        return text;
    }
}
