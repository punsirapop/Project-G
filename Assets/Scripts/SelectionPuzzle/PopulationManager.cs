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
        // Variables for keep track of generated content and fitness value
        int[][] generatedContents = new int[6][];
        _InitialFitness = new int[6];
        // Create new random population
        _Population = new GameObject[6];
        for (int i = 0; i < 6; i++)
        {
            int[] newContent = new int[5];
            int newFitness;
            bool isContentUnique;
            bool isFitnessUnique;
            // Create new random unique content
            do
            {
                // Check if newContent already exist in the population
                newContent = _RandomBitstring(5);
                isContentUnique = true;
                foreach (int[] content in generatedContents)
                {
                    if (content == null)
                    {
                        break;
                    }
                    if (_IsContentEqual(content, newContent))
                    {
                        isContentUnique = false;
                        break;
                    }
                }
            } while (!isContentUnique);
            // Add new unique content to the population
            generatedContents[i] = newContent;
            // Create array of color for ChromosomeRod setting
            Color32[] newColor = new Color32[5];
            for (int j = 0; j < newContent.Length; j++)
            {
                newColor[j] = _Colors[i];
            }
            // Create new random unique fitness value
            do
            {
                newFitness = Random.Range(1, 100);
                isFitnessUnique = true;
                foreach (int fitness in _InitialFitness)
                {
                    if (fitness == newFitness)
                    {
                        isFitnessUnique = false;
                        break;
                    }
                }
            } while (!isFitnessUnique);
            // Add new unique fitness value to the populaiton
            _InitialFitness[i] = newFitness;
            // Create actual gameObject
            _Population[i] = Instantiate(_ChromosomeRodValuePrefab, _ChromosomeHolder);
            _Population[i].GetComponentInChildren<ChromosomeRod>().SetChromosome(newContent, newColor);
            _Population[i].GetComponentInChildren<ChromosomeRod>().RenderRod();
            _Population[i].GetComponentInChildren<ChromosomeRodValue>().SetValue(newFitness);
        }
    }

    // Return an array of random bitstring for the given lenght
    private int[] _RandomBitstring(int lenght)
    {
        int[] newBitstring = new int[lenght];
        for (int i = 0; i < newBitstring.Length; i++)
        {
            newBitstring[i] = Random.Range(0, 2);
        }
        return newBitstring;
    }

    // Return true if both the given array content are the same
    private bool _IsContentEqual(int[] content1, int[] content2)
    {
        if (content1.Length != content2.Length)
        {
            return false;
        }
        for (int i = 0; i < content1.Length; i++)
        {
            if (content1[i] != content2[i])
            {
                return false;
            }
        }
        return true;
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

    // Return the correct ranking of the chromosome
    public int[] GetProperRank()
    {
        // Sort fitness value in descending order
        List<int> sortedList = new List<int>(new int[_InitialFitness.Length]);
        for (int i = 0; i < _InitialFitness.Length; i++)
        {
            sortedList[i] = _InitialFitness[i];
        }
        sortedList.Sort((a, b) => b.CompareTo(a));
        // Calculate the proper rank number for the given value
        int[] properRank = new int[_InitialFitness.Length];
        for (int j = 0; j < _InitialFitness.Length; j++)
        {
            properRank[j] = sortedList.FindIndex(x => x == _InitialFitness[j]) + 1;
        }
        return properRank;
    }
}
