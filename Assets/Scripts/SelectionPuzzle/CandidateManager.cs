using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CandidateManager : MonoBehaviour
{
    public static CandidateManager Instance;

    // Enumeration specify how the parent chromosomes are selected
    public enum Operation {
        None,                   // Type to return when there is no operation in the log, use as a null for Operation
        // Batch operation that apply on the number of chromosomes
        Group,                  // Clicking on the Group button
        Chance,                 // Clicking on the Chance button
        Rank,                   // Clicking on the Rank button
        Wheel,                  // Clicking on the Wheel button
        InverseFitness,         // Clicking on the ~Fitness button
        // Single operation that apply on the single individual chromosome; This generate the parent in the selected parent panel
        PickBestInGroup,        // Clicking on the best fitness chromosome in the group (preferred operation of Tournament-based selection)
        PickNotBestInGroup,     // Clicking on the chromosome which is not the best fitness (not preferred)
        PickInChance,           // Clicking on the chromosome with calculated selected chance (not preferred)
        SpinWheel,              // Clicking on the created wheel to random chromosome from the wheel (preferred for RW and RB selection)
        SetAllRank              // All the chromosome in candidate panel are already be assigned with the rank
    }
    // Preferred operation log for selecting 6 parents, this will be checked by SelectionPuzzleManager
    // TB: {Group, PickBestInGroup, Group, PickBestInGroup, Group, PickBestInGroup, Group, PickBestInGroup, Group, PickBestInGroup, Group, PickBestInGroup}
    private List<Operation> _TournamentOperations = new List<Operation>
    {
        Operation.Group, Operation.PickBestInGroup, Operation.Group, Operation.PickBestInGroup, Operation.Group, Operation.PickBestInGroup,
        Operation.Group, Operation.PickBestInGroup, Operation.Group, Operation.PickBestInGroup, Operation.Group, Operation.PickBestInGroup
    };
    public List<Operation> TournamentOperations => _TournamentOperations;
    // RW: {Chance, Wheel, SpinWheel, SpinWheel, SpinWheel, SpinWheel, SpinWheel, SpinWheel}
    private List<Operation> _RouletteWheelOperations = new List<Operation>
    {
        Operation.Chance, Operation.Wheel, Operation.SpinWheel, Operation.SpinWheel, Operation.SpinWheel, Operation.SpinWheel, Operation.SpinWheel, Operation.SpinWheel
    };
    public List<Operation> RouletteWheelOperations => _RouletteWheelOperations;
    // RB: {Rank, SetAllRank, InverseFitness, Chance, Wheel, SpinWheel, SpinWheel, SpinWheel, SpinWheel, SpinWheel, SpinWheel}
    private List<Operation> _RankOperations = new List<Operation>
    {
        Operation.Rank, Operation.SetAllRank, Operation.InverseFitness,
        Operation.Chance, Operation.Wheel, Operation.SpinWheel, Operation.SpinWheel, Operation.SpinWheel, Operation.SpinWheel, Operation.SpinWheel, Operation.SpinWheel
    };
    public List<Operation> RankOperations => _RankOperations;
    [SerializeField] private List<Operation> _OperationLog;
    public List<Operation> OperationLog => _OperationLog;
    [SerializeField] private GameObject _ChromosomeHolder;
    [SerializeField] private GameObject _RouletteWheel;
    private int _RankCounter;
    public int RankCounter => _RankCounter;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        ClearPanel();
        ClearLog();
    }

    public void ClearPanel()
    {
        // Destroy all object in this panel (if any)
        foreach (Transform child in _ChromosomeHolder.transform)
        {
            Destroy(child.gameObject);
        }
        _ChromosomeHolder.SetActive(true);
        _RouletteWheel.SetActive(false);
    }

    public void ClearLog()
    {
        _OperationLog.Clear();
    }

    public Operation GetLastOperation()
    {
        if (_OperationLog.Count == 0)
        {
            return Operation.None;
        }
        else
        {
            return _OperationLog[_OperationLog.Count - 1];
        }
    }

    // Add operation from the button only if the previous operation is not the same.
    public void AddButtonOperationLog(Operation newOperation)
    {
        if (_OperationLog.Count == 0)
        {
            _OperationLog.Add(newOperation);
        }
        // Prevent the operation log is added twice in the case of player's mouse is double click
        else if (_OperationLog[_OperationLog.Count - 1] != newOperation)
        {
            _OperationLog.Add(newOperation); ;
        }
    }

    // Copy the number of chromosome from the population equal to the given candidateCount
    public void CreateGroup(int candidateCount)
    {
        ClearPanel();
        // If the given number of candidate exceed the number of population, do nothing
        GameObject[] populationPool = PopulationManager.Instance.Population;
        if (candidateCount > populationPool.Length)
        {
            return;
        }
        // If the given number of candidate not exceed the number of population
        // random picking chromosome from the population pool
        else
        {
            // Randompicking chromosome from pool
            for (int candidateIndex = 0; candidateIndex < candidateCount; candidateIndex++)
            {
                int randomIndex = Random.Range(0, populationPool.Length);
                // Instantiate the actual GameObject of the candidate
                GameObject newIndividual = Instantiate(populationPool[randomIndex], _ChromosomeHolder.transform);
                newIndividual.AddComponent<Button>().onClick.AddListener(() => _AddParentFromGroup(newIndividual));
                // Remove selected index from the pool
                GameObject[] newPool = new GameObject[populationPool.Length - 1];
                for (int i = 0; i < newPool.Length; i++)
                {
                    newPool[i] = (i >= randomIndex) ? populationPool[i + 1] : populationPool[i];
                }
                populationPool = newPool;
            }
        }
        // Record the operation
        AddButtonOperationLog(Operation.Group);
    }

    // Add clicked chromosome from group mode in the selected parent panel and record the operation
    private void _AddParentFromGroup(GameObject clickedChromosome)
    {
        SelectedParentManager.Instance.AddSelectedChromosome(clickedChromosome.GetComponentInChildren<ChromosomeRod>());
        ChromosomeRodValue[] currentRodValues = GetComponentsInChildren<ChromosomeRodValue>(true);
        int bestFitnessValue = 0;
        foreach (ChromosomeRodValue chromosome in currentRodValues)
        {
            bestFitnessValue = (chromosome.Value > bestFitnessValue) ? chromosome.Value : bestFitnessValue;
        }
        if (clickedChromosome.GetComponent<ChromosomeRodValue>().Value == bestFitnessValue)
        {
            _OperationLog.Add(Operation.PickBestInGroup);
        }
        else
        {
            _OperationLog.Add(Operation.PickNotBestInGroup);
        }
    }

    // Create the chance to be selected of each chromosome
    public void CreateChance()
    {
        ClearPanel();
        // If the given number of candidate exceed the number of population, do nothing
        GameObject[] population = PopulationManager.Instance.Population;
        // Calculate total fitness
        int totalFitness = 0;
        foreach (GameObject individual in population)
        {
            totalFitness += individual.GetComponentInChildren<ChromosomeRodValue>().Value;
        }
        // Calculate the actual GameObject with the proportion percentage
        foreach (GameObject individual in population)
        {
            GameObject newIndividual = Instantiate(individual, _ChromosomeHolder.transform);
            newIndividual.AddComponent<Button>().onClick.AddListener(() => _AddParentFromChance(newIndividual));

            ChromosomeRodValue newIndividualRodValue = newIndividual.GetComponent<ChromosomeRodValue>();
            float percentage = (float)newIndividualRodValue.Value / (float)totalFitness * 100f;
            newIndividualRodValue.SetValue(percentage);
        }
        // Record the operation
        AddButtonOperationLog(Operation.Chance);
    }

    // Add clicked chromosome from chance mode in the selected parent panel
    // This is not the preferred operation for any parent selection method
    private void _AddParentFromChance(GameObject clickedChromosome)
    {
        SelectedParentManager.Instance.AddSelectedChromosome(clickedChromosome.GetComponentInChildren<ChromosomeRod>());
        _OperationLog.Add(Operation.PickInChance);
    }

    // Create Roulette Wheel using the fitness of each individual in population
    public void CreateWheel()
    {
        _ChromosomeHolder.SetActive(false);
        _RouletteWheel.SetActive(true);
        ChromosomeRodValue[] currentRodValues = GetComponentsInChildren<ChromosomeRodValue>(true);
        _RouletteWheel.GetComponent<RouletteWheel>().SetWheel(currentRodValues);
        // Record the operation
        AddButtonOperationLog(Operation.Wheel);
    }

    // Add chromosome from the wheel to the SelectedParent panel
    public void AddParentFromWheel(int index)
    {
        SelectedParentManager.Instance.AddSelectedChromosome(GetComponentsInChildren<ChromosomeRod>(true)[index]);
        // Record the operation
        _OperationLog.Add(Operation.SpinWheel);
    }

    // Copy all population and enable rank assignment
    public void CreateRank()
    {
        ClearPanel();
        GameObject[] population = PopulationManager.Instance.Population;
        // Calculate the actual GameObject with the proportion percentage
        foreach (GameObject individual in population)
        {
            GameObject newIndividual = Instantiate(individual, _ChromosomeHolder.transform);
            ChromosomeRodValue newIndividualRodValue = newIndividual.GetComponent<ChromosomeRodValue>();
            newIndividualRodValue.EnableRank();
        }
        _RankCounter = 1;
        // Record the operation
        AddButtonOperationLog(Operation.Rank);
    }

    public void AddRank()
    {
        _RankCounter++;
        if (_RankCounter > GetComponentsInChildren<ChromosomeRodValue>().Length)
        {
            _OperationLog.Add(Operation.SetAllRank);
            SelectionButtonManager.Instance.SetButtons();
        }
    }

    // Assign reverse rank to as the population fitness
    public void AssignInverseRankAsFitness()
    {
        ChromosomeRodValue[] currentCandidates = GetComponentsInChildren<ChromosomeRodValue>();
        int[] newPopulationFitness = new int[currentCandidates.Length];
        for (int i = 0; i < newPopulationFitness.Length; i++)
        {
            newPopulationFitness[i] = _RankCounter - currentCandidates[i].Value;
        }
        PopulationManager.Instance.SetPopulationFitness(newPopulationFitness);
        // Record the operation
        AddButtonOperationLog(Operation.InverseFitness);
    }
}
