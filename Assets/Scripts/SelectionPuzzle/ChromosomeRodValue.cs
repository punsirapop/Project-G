using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChromosomeRodValue : MonoBehaviour
{
    [SerializeField] private int _Value;
    [SerializeField] private bool _IsPercentage;
    [SerializeField] private TextMeshProUGUI _FitnessText;

    public void SetValue(int newValue, bool newIsPercentage = false)
    {
        _Value = newValue;
        _IsPercentage = newIsPercentage;
        string valueText = _Value.ToString();
        valueText += _IsPercentage ? "%" : "";
        _FitnessText.text = valueText;
    }
}
