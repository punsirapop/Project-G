using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryProduction : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown[] _Types;
    [SerializeField] private GameObject[] _Adjustors;
    [SerializeField] private TextMeshProUGUI _BreedGenText;
    [SerializeField] private TextMeshProUGUI _CostText;
    private int _BreedGen;

    void Start()
    {
        _BreedGen = 1;
    }

    void Update()
    {
        foreach (GameObject adjustor in _Adjustors)
        {
            adjustor.GetComponentInChildren<TextMeshProUGUI>().text = adjustor.GetComponentInChildren<Slider>().value.ToString();
        }
        _BreedGenText.text = _BreedGen.ToString();
        _CostText.text = (_BreedGen * 1000).ToString();

    }

    public void IncreaseBreedGen()
    {
        _BreedGen++;
    }

    public void DecreaseBreedGen()
    {
        _BreedGen--;
        _BreedGen = (_BreedGen < 0) ? 0 : _BreedGen;
    }
}
