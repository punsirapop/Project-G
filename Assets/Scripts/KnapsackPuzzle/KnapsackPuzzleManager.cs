using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnapsackPuzzleManager : MonoBehaviour
{
    public static KnapsackPuzzleManager Instance;

    // Data of all factory
    [SerializeField] private FactorySO[] _FactoriesData;
    public FactorySO[] FactoriesData => _FactoriesData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
