using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryProduction : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown[] _Types;
    [SerializeField] private GameObject[] _Adjustors;
    [SerializeField] private TextMeshProUGUI _BreedGenText;
    [SerializeField] private TextMeshProUGUI _CostText;
    private int _BreedGen;

    void Start()
    {
        _BreedGen = 1;
    }

    void Update()
    {
        foreach (GameObject adjustor in _Adjustors)
        {
            adjustor.GetComponentInChildren<TextMeshProUGUI>().text = adjustor.GetComponentInChildren<Slider>().value.ToString();
        }
        _BreedGenText.text = _BreedGen.ToString();
        _CostText.text = (_BreedGen * 1000).ToString();

    }

    public void IncreaseBreedGen()
    {
        _BreedGen++;
    }

    public void DecreaseBreedGen()
    {
        _BreedGen--;
        _BreedGen = (_BreedGen < 0) ? 0 : _BreedGen;
    }

    // Produce a new set of chromosome using Genetic Algorithm
    public void Produce()
    {
        WeaponChromosome[] currentPopulation = FactoryManager.Instance.GetAllWeapon();
        int populationCount = currentPopulation.Length;
        WeaponChromosome[] nextPopulation = new WeaponChromosome[populationCount];
        int newPopulationIndex = 0;
        // Calculate the number of elite and parent
        float elitismRate = _Adjustors[0].GetComponentInChildren<Slider>().value;
        int elitismCount = (int) (populationCount * elitismRate / 100);
        int parentCount = populationCount - elitismCount;
        // If the number of parent is odd number, make it even
        if (parentCount % 2 == 1)
        {
            parentCount--;
            elitismCount++;
        }
        // Elitism
        List<WeaponChromosome> currentPopulationList = new();
        currentPopulationList.AddRange(currentPopulation);
        currentPopulationList.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));
        for (int eliteIndex = 0; eliteIndex < elitismCount; eliteIndex++)
        {
            nextPopulation[newPopulationIndex] = currentPopulationList[eliteIndex];
            newPopulationIndex++;
        }
        foreach (WeaponChromosome c in nextPopulation)
        {
            if (c == null)
            {
                break;
            }
            Debug.Log(c.ToString());
        }
        // Parent Selection
        // WIP

        // Crossvoer
        // WIP

        // Mutation
        // WIP
    }
}
