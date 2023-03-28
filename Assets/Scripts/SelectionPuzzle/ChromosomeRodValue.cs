using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChromosomeRodValue : MonoBehaviour
{
    [SerializeField] public Color32 Color => GetComponent<ChromosomeRod>().GetColorAtIndex(1);
    [SerializeField] private int _Value;
    [SerializeField] public int Value => _Value;
    [SerializeField] private float _Percentage;
    //[SerializeField] private bool _IsPercentage;
    [SerializeField] private GameObject _ValueBox;

    // Set fitness value
    public void SetValue(int newValue)
    {
        _Value = newValue;
        //_IsPercentage = false;
        string valueText = (_Value < 0) ? "?" : _Value.ToString();
        _ValueBox.GetComponentInChildren<TextMeshProUGUI>().text = valueText;
    }

    // Overloading function for set percentage value
    public void SetValue(float newValue)
    {
        _Percentage = newValue;
        //_IsPercentage = true;
        _ValueBox.GetComponentInChildren<TextMeshProUGUI>().text = ((int)_Percentage).ToString() + "%";
    }

    // Enable rank assinment
    public void EnableRank()
    {
        SetValue(-1);
        _ValueBox.AddComponent<Button>().onClick.AddListener(() => _SetRank());
    }

    // Assign the rank to this chromosome
    private void _SetRank()
    {
        SetValue(CandidateManager.Instance.RankCounter);
        CandidateManager.Instance.AddRank();
        _ValueBox.GetComponent<Button>().enabled = false;
    }
}
