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
    public void UpdateChromoButton()
    {
        foreach (Transform child in _ChromoHolder)
        {
            Destroy(child.gameObject);
        }
        ChromoButton[] selectedChromo = ParentManager.Instance.GetSelectedChromo();
        foreach (ChromoButton chromoButton in selectedChromo)
        {
            if (chromoButton == null)
            {
                break;
            }
            Instantiate(chromoButton.GetBitHolder(), _ChromoHolder).AddComponent<BitHolderHelper>();
        }
    }

    // Swapping part of object by creating new object corresponding to crossover points
    // Hard-code it to perform on only a pair of object (2 chromosomes only)
    public void UpdateSwapping()
    {
        // Destroy all current chromo
        foreach (Transform child in _ChromoHolder)
        {
            Destroy(child.gameObject);
        }
        // Create new chromo with proper bit content corresponding to crossover points
        BitHolderHelper[] bitHolders = this.GetComponentsInChildren<BitHolderHelper>();
        GameObject[] newChromo = new GameObject[bitHolders.Length];
        for (int i = 0; i < bitHolders.Length; i++)
        {
            newChromo[i] = Instantiate(_BitHolderHelperPrefab, _ChromoHolder);
        }
        // Add proper bit content to new chromosome
        for (int i = 0; i < bitHolders[0].BitLength; i++)
        {
            GameObject new1;
            GameObject new2;
            if ((i >= _DraggedIndexes[0]) && ((i <= _DraggedIndexes[1])))
            {
                new1 = Instantiate(bitHolders[1].GetBitObjectAtIndex(i), newChromo[0].transform);
                new2 = Instantiate(bitHolders[0].GetBitObjectAtIndex(i), newChromo[1].transform);
            }
            else
            {
                new1 = Instantiate(bitHolders[0].GetBitObjectAtIndex(i), newChromo[0].transform);
                new2 = Instantiate(bitHolders[1].GetBitObjectAtIndex(i), newChromo[1].transform);
            }
            new1.GetComponent<Image>().enabled = true;
            new2.GetComponent<Image>().enabled = true;
            new1.GetComponent<LayoutElement>().enabled = true;
            new2.GetComponent<LayoutElement>().enabled = true;
        }
        // Add BitHolderHelper to new chromo
        foreach (GameObject chromo in newChromo)
        {
            chromo.AddComponent<BitHolderHelper>();
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
