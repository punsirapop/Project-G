using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Knapsack : MonoBehaviour
{
    // Information of the Knapsack
    private string _Name;
    public string Name => _Name;
    private int _TotalValue;
    private int _Weight1Limit;
    private int _Weight2Limit;
    private int _TotalWeight1;
    private int _TotalWeight2;

    // UI element to display the information
    [SerializeField] TextMeshProUGUI NameText;
    [SerializeField] TextMeshProUGUI ValueText;
    [SerializeField] GameObject _Weight1Bar;
    [SerializeField] GameObject _Weight2Bar;

    [SerializeField] ItemHolder Holder;

    public void SetKnapsack(string name, int w1, int w2)
    {
        _Name = name;
        _Weight1Limit = w1;
        _Weight2Limit = w2;
        if (_Weight2Limit <= 0)
        {
            _Weight2Bar.SetActive(false);
        }
        _RenderInfo();
    }

    void Awake()
    {
        SetKnapsack("K1", 1000, 1000);
    }

    void Start()
    {
        _TotalValue = 0;
        _TotalWeight1 = 0;
        _TotalWeight2 = 0;
    }

    void Update()
    {
        int[] properties = Holder.GetTotalProperties();
        _TotalValue = properties[0];
        _TotalWeight1 = properties[1];
        _TotalWeight2 = properties[2];
        _RenderInfo();
    }

    // Render the information into the UI
    private void _RenderInfo()
    {
        NameText.text = _Name;
        ValueText.text = _TotalValue.ToString();
        // Fill in the bar proportion to the total weight
        Image bar1 = _Weight1Bar.transform.Find("Bar").GetComponent<Image>();
        bar1.fillAmount = (float)_TotalWeight1 / (float)_Weight1Limit;
        if (_Weight2Limit > 0)
        {
            Image bar2 = _Weight2Bar.transform.Find("Bar").GetComponent<Image>();
            bar2.fillAmount = (float)_TotalWeight2 / (float)_Weight2Limit;
        }
    }
}
