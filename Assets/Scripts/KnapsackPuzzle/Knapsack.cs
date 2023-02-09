using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Knapsack : MonoBehaviour
{
    //private string _Name;
    public string Name;
    [SerializeField] TextMeshProUGUI NameLabel;
    
    void Start()
    {
        // Display the name of Knapsack
        NameLabel.text = Name;
    }
}
