using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChromosomeRodValue : MonoBehaviour
{
    [SerializeField] public Color32 Color => GetComponent<ChromosomeRod>().GetColorAtIndex(1);
    [SerializeField] private int _Value;
    [SerializeField] public int Value => _Value;
    [SerializeField] private float _Percentage;
    [SerializeField] private bool _IsPercentage;
    [SerializeField] private TextMeshProUGUI _FitnessText;

    // Set fitness value
    public void SetValue(int newValue)
    {
        _Value = newValue;
        _IsPercentage = false;
        _FitnessText.text = _Value.ToString();
    }

    // Overloading function for set percentage value
    public void SetValue(float newValue)
    {
        _Percentage = newValue;
        _IsPercentage = true;
        _FitnessText.text = ((int)_Percentage).ToString() + "%";
    }
}
