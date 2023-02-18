using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/FactoryConfig")]
public class FactoryConfig : ScriptableObject
{
    // Knapsack configuration of each factory
    [SerializeField] private KnapsackSO[] _Factory1Knapsacks;
    public KnapsackSO[] Factory1Knapsacks => _Factory1Knapsacks;
    [SerializeField] private KnapsackSO[] _Factory2Knapsacks;
    public KnapsackSO[] Factory2Knapsacks => _Factory2Knapsacks;
    [SerializeField] private KnapsackSO[] _Factory3Knapsacks;
    public KnapsackSO[] Factory3Knapsacks => _Factory3Knapsacks;
    [SerializeField] private KnapsackSO[] _Factory4Knapsacks;
    public KnapsackSO[] Factory4Knapsacks => _Factory4Knapsacks;

    // Items set configuration
    // Use set1 (1D items) to factory1 and factory3
    // Use set2 (2D items) to factory2 and factory4
    [SerializeField] private ItemSO[] _Set1Items;
    public ItemSO[] Set1Items => _Set1Items;
    [SerializeField] private ItemSO[] _Set2Items;
    public ItemSO[] Set2Items => _Set2Items;
    //public ItemSO[] Factory1Items => _Set1Items;
    //public ItemSO[] Factory2Items => _Set2Items;
    //public ItemSO[] Factory3Items => _Set1Items;
    //public ItemSO[] Factory4Items => _Set2Items;
}
