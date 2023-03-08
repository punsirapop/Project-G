using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject _ChromosomeRodFitnessPrefab;
    [SerializeField] private Color32[] _Colors;

    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            int[] newContent = new int[5];
            int newFitness = Random.Range(0, 100);
            Color32[] newColor = new Color32[5];
            for (int j = 0; j < newContent.Length; j++)
            {
                newContent[j] = Random.Range(0, 2);
                newColor[j] = _Colors[i];
            }
            GameObject newRodFitness = Instantiate(_ChromosomeRodFitnessPrefab, this.transform);
            newRodFitness.GetComponentInChildren<ChromosomeRod>().SetChromosome(newContent, newColor);
            newRodFitness.GetComponentInChildren<ChromosomeRod>().RenderRod();
            newRodFitness.GetComponentInChildren<ChromosomeRodValue>().SetValue(newFitness);
        }
    }
}
