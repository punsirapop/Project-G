using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildrenManager : MonoBehaviour
{
    public static ChildrenManager Instance;
    [SerializeField] private GameObject[] _CrossoverPoints;
    [SerializeField] private Transform _ChromoButtonHolder;
    [SerializeField] private GameObject _BitHolderHelperPrefab;

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

    public void UpdateChromoButton()
    {
        // Add/remove ChromoButton
        foreach (Transform child in _ChromoButtonHolder)
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
            Instantiate(chromoButton.GetBitHolder(), _ChromoButtonHolder).AddComponent<BitHolderHelper>();
        }
    }

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
        string t = "";
        foreach (int i in crossoverPoints)
        {
            t += i.ToString() + ", ";
        }
        Debug.Log(t);
        return crossoverPoints;
    }
}
