using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance;

    [SerializeField] private GameObject _ChromosomeRodValuePrefab;
    [SerializeField] private Color32[] _Colors;
    private GameObject[] _Population;
    public GameObject[] Population => _Population;
    private int[] _InitialFitness;

    [SerializeField] private Transform _ChromosomeHolder;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        CreatePopulation();
    }

    public void CreatePopulation()
    {
        // Destroy all object in this panel (if any)
        foreach (Transform child in _ChromosomeHolder)
        {
            Destroy(child.gameObject);
        }
        // Create new random population
        _Population = new GameObject[6];
        _InitialFitness = new int[6];
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
            _Population[i] = Instantiate(_ChromosomeRodValuePrefab, _ChromosomeHolder);
            _Population[i].GetComponentInChildren<ChromosomeRod>().SetChromosome(newContent, newColor);
            _Population[i].GetComponentInChildren<ChromosomeRod>().RenderRod();
            _Population[i].GetComponentInChildren<ChromosomeRodValue>().SetValue(newFitness);
            _InitialFitness[i] = newFitness;
        }
    }

    // Set the population fitness to the given new fitness array
    public void SetPopulationFitness(int[] newPopulationFitness)
    {
        if (newPopulationFitness.Length != _Population.Length)
        {
            return;
        }
        for (int i = 0; i < _Population.Length; i++)
        {
            _Population[i].GetComponentInChildren<ChromosomeRodValue>().SetValue(newPopulationFitness[i]);
        }
    }

    public void ResetPopulationFitness()
    {
        for (int i = 0; i < _Population.Length; i++)
        {
            _Population[i].GetComponentInChildren<ChromosomeRodValue>().SetValue(_InitialFitness[i]);
        }
    }
}
