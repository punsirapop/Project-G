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
    [SerializeField] private TextMeshProUGUI _NameText;
    [SerializeField] private TextMeshProUGUI _ValueText;
    [SerializeField] private GameObject _Weight1Bar;
    [SerializeField] private GameObject _Weight2Bar;

    [SerializeField] private ItemHolder _Holder;

    #region Data setter and getter
    public void SetKnapsack(KnapsackSO knapsackSO)
    {
        _Name = knapsackSO.Name;
        _Weight1Limit = knapsackSO.Weight1Limit;
        _Weight2Limit = knapsackSO.Weight2Limit;
        if (_Weight2Limit <= 0)
        {
            _Weight2Bar.SetActive(false);
        }
        _RenderInfo();
    }

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

    public Transform GetItemHolder()
    {
        return _Holder.transform;
    }
    #endregion

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
        int[] properties = _Holder.GetTotalProperties();
        _TotalWeight1 = properties[1];
        _TotalWeight2 = properties[2];
        if ((_TotalWeight1 > _Weight1Limit) || (_TotalWeight2 > _Weight2Limit))
        {
            _TotalValue = 0;
        }
        else 
        {
            _TotalValue = properties[0];
        }
        
        _RenderInfo();
    }

    public int GetDimension()
    {
        if (_Weight2Limit > 0)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    // Render the information into the UI
    private void _RenderInfo()
    {
        // Rendering text
        _NameText.text = _Name;
        _ValueText.text = _TotalValue.ToString();
        _Weight1Bar.transform.Find("WeightText").GetComponent<TextMeshProUGUI>().text = _TotalWeight1.ToString() + "/" + _Weight1Limit.ToString();
        _Weight2Bar.transform.Find("WeightText").GetComponent<TextMeshProUGUI>().text = _TotalWeight2.ToString() + "/" + _Weight2Limit.ToString();
        // Fill in the bar proportion to the total weight
        Image bar1 = _Weight1Bar.transform.Find("Bar").GetComponent<Image>();
        float barRatio = (float)_TotalWeight1 / (float)_Weight1Limit;
        bar1.fillAmount = barRatio;
        // Change bar color to red if the weight exceed the limit
        bar1.color = (barRatio > 1) ? Color.red : Color.blue;
        // Fill in bar2 the same way
        if (_Weight2Limit > 0)
        {
            Image bar2 = _Weight2Bar.transform.Find("Bar").GetComponent<Image>();
            barRatio = (float)_TotalWeight2 / (float)_Weight2Limit;
            bar2.fillAmount = barRatio;
            bar2.color = (barRatio > 1) ? Color.red : Color.blue;
        }
    }

    #region Experimental function for gradually add the red color to the bar
    //// Render the information into the UI
    //private void _RenderInfo()
    //{
    //    _NameText.text = _Name;
    //    _ValueText.text = _TotalValue.ToString();
    //    // Fill in the bar proportion to the total weight
    //    Image bar1 = _Weight1Bar.transform.Find("Bar").GetComponent<Image>();
    //    float barRatio = (float)_TotalWeight1 / (float)_Weight1Limit;
    //    if (barRatio > 1)
    //    {
    //        barRatio = 1;
    //    }
    //    bar1.fillAmount = barRatio;
    //    bar1.color = _CalculateBarColor(barRatio, 0.66f);

    //    // Fill in bar2 the same way
    //    if (_Weight2Limit > 0)
    //    {
    //        Image bar2 = _Weight2Bar.transform.Find("Bar").GetComponent<Image>();
    //        barRatio = (float)_TotalWeight2 / (float)_Weight2Limit;
    //        if (barRatio > 1)
    //        {
    //            barRatio = 1;
    //        }
    //        bar2.fillAmount = barRatio;
    //        bar2.color = _CalculateBarColor(barRatio, 0.66f);
    //    }
    //}

    //// Shift color to red when barRatio is closer to 1
    //// Return Color32 value of the color that bar should be
    //private Color32 _CalculateBarColor(float barRatio, float colorThershold)
    //{
    //    if (barRatio > colorThershold)
    //    {
    //        float colorRatio = (float)((barRatio - colorThershold) / (1 - colorThershold));
    //        byte redRatio = (byte)(255 * colorRatio);
    //        byte greenRatio = (byte)(255 * (1 - colorRatio));
    //        return new Color32(redRatio, greenRatio, 0, 255);
    //    }
    //    else
    //    {
    //        return new Color32(0, 255, 0, 255);
    //    }
    //}
    #endregion
}
