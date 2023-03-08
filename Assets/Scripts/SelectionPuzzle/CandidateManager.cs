using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandidateManager : MonoBehaviour
{
    public static CandidateManager Instance;
    [SerializeField] private Transform _ChromosomeHolder;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Destroy all object in this panel (if any)
        foreach (Transform child in _ChromosomeHolder)
        {
            Destroy(child.gameObject);
        }
    }

    // Copy the number of chromosome from the population equal to the given candidateCount
    public void AddCandidate(int candidateCount)
    {
        GameObject[] population = PopulationManager.Instance.Population;
        if (candidateCount > population.Length)
        {
            return;
        }
        // Copy the whole population if it's the same amount
        if (candidateCount == population.Length)
        {
            foreach (GameObject individual in population)
            {
                GameObject newIidividual = Instantiate(individual, _ChromosomeHolder);
                newIidividual.AddComponent<Button>();
                ChromosomeRod newIndividualRod = newIidividual.GetComponentInChildren<ChromosomeRod>();
                newIidividual.GetComponent<Button>().onClick.AddListener(() => SelectedParentManager.Instance.AddSelectedChromosome(newIndividualRod));
            }
        }
    }
}
