using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildrenManager : MonoBehaviour
{
    public static ChildrenManager Instance;

    [SerializeField] private CrossoverPoint[] _CrossoverPoints;
    [SerializeField] private Transform _ChromoHolder;
    [SerializeField] private GameObject _CreatePointButton;
    private int[] _DraggedIndexes;

    public void SetDraggedIndexes(int[] indexes)
    {
        _DraggedIndexes = indexes;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Add/remove ChromosomeRod
    public void UpdateChromosomeRods()
    {
        // Destroy all current ChromosomeRods
        foreach (Transform child in _ChromoHolder)
        {
            Destroy(child.gameObject);
        }
        // Create ChromosomeRods that are selected in the parent panel
        ChromosomeRod[] selectedRods = ParentManager.Instance.GetSelectedChromosomeRods();
        foreach (ChromosomeRod selectedRod in selectedRods)
        {
            GameObject newRodObject = Instantiate(selectedRod.gameObject, _ChromoHolder);
            newRodObject.AddComponent<CanvasGroup>();
            newRodObject.AddComponent<ChromosomeRodDraggable>();
        }
    }

    // Swapping part of object by creating new object corresponding to crossover points
    // Hard-code it to perform on only a pair of object (2 chromosomes only)
    public void UpdateSwapping()
    {
        ChromosomeRod[] parents = this.GetComponentsInChildren<ChromosomeRod>();
        int value0;
        int value1;
        Color32 color0;
        Color32 color1;
        // Swap the properties of chromosomeRods within crossover section
        for (int i = _DraggedIndexes[0]; i <= _DraggedIndexes[1]; i++)
        {
            // Swap bit value
            value0 = parents[0].GetValueAtIndex(i);
            value1 = parents[1].GetValueAtIndex(i);
            parents[0].SetValueAtIndex(i, value1);
            parents[1].SetValueAtIndex(i, value0);
            // Swap color
            color0 = parents[0].GetColorAtIndex(i);
            color1 = parents[1].GetColorAtIndex(i);
            parents[0].SetColorAtIndex(i, color1);
            parents[1].SetColorAtIndex(i, color0);
            // Re-render the chromosomeRods
            parents[0].RenderRod();
            parents[1].RenderRod();
        }
    }

    // Return the array of all selected crossover point indexes
    public int[] GetCrossoverPoints()
    {
        int selectedCount = 0;
        foreach (CrossoverPoint crossoverPoint in _CrossoverPoints)
        {
            selectedCount += crossoverPoint.isOn ? 1 : 0;
        }
        int[] crossoverPoints = new int[selectedCount];
        int pointIndex = 0;
        for (int i = 0; i < _CrossoverPoints.Length; i++)
        {
            if (_CrossoverPoints[i].isOn)
            {
                crossoverPoints[pointIndex] = i;
                pointIndex++;
            }
        }
        return crossoverPoints;
    }

    // Set element on the panel depend on it's is solve level puzzle or not
    public void SetPuzzle(bool isSolve)
    {
        // Enable random point button only if it IS NOT a solving puzzle
        _CreatePointButton.SetActive(!isSolve);
        foreach (CrossoverPoint crossoverPoint in _CrossoverPoints)
        {
            // The crossover points can be directly interactable only if it IS a solving puzzle
            crossoverPoint.SetInteractable(isSolve);
            // Set all point to off
            crossoverPoint.SetIsOn(false);
        }
    }

    // Randomly select one crossover point
    public void SelectRandomCrossoverPoint()
    {
        // Calculate the number of selected crossover points
        int selectedCount = 0;
        foreach (CrossoverPoint crossoverPoint in _CrossoverPoints)
        {
            selectedCount += crossoverPoint.isOn ? 1 : 0;
        }
        // If all point have already been selected, do nothing
        if (selectedCount >= _CrossoverPoints.Length)
        {
            return;
        }
        // Else, randomly select unselected point
        else
        {
            // Random index among unselected points
            int randomIndex = UnityEngine.Random.Range(0, _CrossoverPoints.Length - selectedCount);
            int unselectedCount = 0;
            foreach (CrossoverPoint crossoverPoint in _CrossoverPoints)
            {
                if (!crossoverPoint.isOn)
                {
                    if ((randomIndex == unselectedCount))
                    {
                        crossoverPoint.SetIsOn(true);
                        break;
                    }
                    unselectedCount++;
                }
            }
        }
    }

    // Unselect all crossover points
    public void UnselectAllCrossoverPoints()
    {
        foreach (CrossoverPoint crossoverPoint in _CrossoverPoints)
        {
            crossoverPoint.SetIsOn(false);
        }
    }

    // Return the value of all bred children chromosome
    public int[][] GetBredChildren()
    {
        ChromosomeRod[] bredChromosomeRods = this.GetComponentsInChildren<ChromosomeRod>();
        int[][] bredChildren = new int[bredChromosomeRods.Length][];
        for (int i = 0; i < bredChromosomeRods.Length; i++)
        {
            bredChildren[i] = bredChromosomeRods[i].GetChromosomeValue();
        }
        return bredChildren;
    }

    // Return the color of all bred children
    public Color32[] GetBredChildColor()
    {
        return this.GetComponentsInChildren<ChromosomeRod>()[0].GetColorValue();
    }
}
