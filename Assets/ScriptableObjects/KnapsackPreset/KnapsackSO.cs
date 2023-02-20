using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Knapsack")]
public class KnapsackSO : ScriptableObject
{
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] private int _Weight1Limit;
    public int Weight1Limit => _Weight1Limit;
    [SerializeField] private int _Weight2Limit;
    public int Weight2Limit => _Weight2Limit;
}
