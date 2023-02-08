using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Knapsack : MonoBehaviour
{
    private string _Name;
    [SerializeField] TextMeshProUGUI NameLabel;
    
    // Start is called before the first frame update
    void Start()
    {
        _Name = "K1";
        NameLabel.text = _Name;
    }
}
