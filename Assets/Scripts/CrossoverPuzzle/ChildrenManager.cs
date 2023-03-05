using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildrenManager : MonoBehaviour
{
    public static ChildrenManager Instance;

    [SerializeField] private GameObject[] _CrossoverPoints;
    [SerializeField] private Transform _ChromoHolder;
    [SerializeField] private GameObject _BitHolderHelperPrefab;
    private int[] _DraggedIndexes;

    public void SetDraggedIndexes(int[] indexes)
    {
        _DraggedIndexes = indexes;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        // Coloring the crossover points
        foreach (GameObject crossoverPoint in _CrossoverPoints)
        {
            bool isOn = crossoverPoint.GetComponent<Toggle>().isOn;
            Image[] images = crossoverPoint.GetComponentsInChildren<Image>();
            Color32 color = isOn ? new Color32(255, 255, 0, 255) : new Color32(255, 255, 255, 255);
            foreach (Image image in images)
            {
                image.color = color;
            }
        }
    }

    // Add/remove ChromoButton
    public void UpdateChromosomeRods()
    {
        // Destroy all current chromosomeRods
        foreach (Transform child in _ChromoHolder)
        {
            Destroy(child.gameObject);
        }
        // Create chromosomeRods that are selected in the parent panel
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

    // return the array of all selected crossover point indexes
    public int[] GetCrossoverPoints()
    {
        int selectedCount = 0;
        foreach (GameObject crossoverPoint in _CrossoverPoints)
        {
            bool isOn = crossoverPoint.GetComponent<Toggle>().isOn;
            selectedCount += isOn ? 1 : 0;
        }
        int[] crossoverPoints = new int[selectedCount];
        int pointIndex = 0;
        for (int i = 0; i < _CrossoverPoints.Length; i++)
        {
            if (_CrossoverPoints[i].GetComponent<Toggle>().isOn)
            {
                crossoverPoints[pointIndex] = i;
                pointIndex++;
            }
        }
        return crossoverPoints;
    }
}
