using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossoverPuzzleManager : MonoBehaviour
{
    public static CrossoverPuzzleManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Set Parent and Children Manager to suit the type of puzzle
    // puzzleType: 0 = demonstrate single point, 1 = demonstrate two point, 2 = solve single point, 3 = solve two point
    public void SetPuzzle(int puzzleType)
    {
        ParentManager.Instance.InstaniateChromosomeRodToggles(puzzleType);
        ParentManager.Instance.UnselectAllToggles();
        ChildrenManager.Instance.SetPuzzle(puzzleType >= 2);
    }
}
