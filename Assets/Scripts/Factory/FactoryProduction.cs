using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryProduction : MonoBehaviour
{
    // UI elements
    [SerializeField] TextMeshProUGUI CurrentPopDisplay;
    [SerializeField] TextMeshProUGUI CurrentGenDisplay;
    [SerializeField] private TMP_Dropdown[] _Types;
    [SerializeField] private GameObject[] _Adjustors;
    [SerializeField] private TextMeshProUGUI _BreedGenText;
    [SerializeField] private TextMeshProUGUI _CostText;
    [SerializeField] private Button[] _AddBreedGenButtons;
    [SerializeField] private Button _BreedButton;
    // Information
    private int _BreedGen;
    private FactorySO _CurrentFactory => PlayerManager.CurrentFactoryDatabase;

    void Awake()
    {
        _BreedGen = 1;
        if (_CurrentFactory.BreedPref.Equals(default(BreedPref)))
        {
            _CurrentFactory.SetBreedPref(
                new BreedPref(
                    _Types[0].value, // Parent selection type
                    _Types[1].value, // Crossover type
                    (int)_Adjustors[0].GetComponentInChildren<Slider>().value, // Elitism rate
                    _Types[2].value, // Mutation type
                    (int)_Adjustors[1].GetComponentInChildren<Slider>().value, // Mutation rate
                    _BreedGen// Breeding generation count
                    )
                );
        }
    }

    void Start()
    {
        AddBreedGen(0);
        RenderPanelFromPref();
    }

    void Update()
    {
        foreach (GameObject adjustor in _Adjustors)
        {
            adjustor.GetComponentInChildren<TextMeshProUGUI>().text = adjustor.GetComponentInChildren<Slider>().value.ToString();
        }
    }

    public void AddBreedGen(int amount)
    {
        _BreedGen += amount;
        if (_BreedGen <= 1)
        {
            _BreedGen = 1;
        }
        else if (_BreedGen >= 10)
        {
            _BreedGen = 10;
        }
        _RenderBreedButtons();
        _BreedGenText.text = _BreedGen.ToString();
        _CostText.text = PlayerManager.Money.ToString() + "/" + _CalculateBreedCost().ToString();
    }

    private void _RenderBreedButtons()
    {
        _AddBreedGenButtons[0].interactable = true;
        _AddBreedGenButtons[1].interactable = true;
        if (_BreedGen <= 1)
        {
            _AddBreedGenButtons[0].interactable = false;
        }
        else if (_BreedGen >= 10)
        {
            _AddBreedGenButtons[1].interactable = false;
        }
        _BreedButton.interactable = (_CalculateBreedCost() <= PlayerManager.Money);
    }

    public void RenderPanelFromPref()
    {
        CurrentPopDisplay.text = _CurrentFactory.PopulationCount.ToString();
        CurrentGenDisplay.text = _CurrentFactory.Generation.ToString();
        _Types[0].value = _CurrentFactory.BreedPref.TypeParentSelect;
        _Types[1].value = _CurrentFactory.BreedPref.TypeCrossover;
        _Types[2].value = _CurrentFactory.BreedPref.TypeMutation;
        _Adjustors[0].GetComponentInChildren<Slider>().value = _CurrentFactory.BreedPref.ElitismRate;
        _Adjustors[1].GetComponentInChildren<Slider>().value = _CurrentFactory.BreedPref.MutationRate;
        _BreedGen = _CurrentFactory.BreedPref.BreedGen;
        _BreedGenText.text = _BreedGen.ToString();
        _CostText.text = PlayerManager.Money.ToString() + "/" + _CalculateBreedCost().ToString();
        // All component are interactable only if it's in idle status
        bool isIdle = (_CurrentFactory.Status == Status.IDLE) ? true : false;
        foreach (var dropdownSelector in _Types)
        {
            dropdownSelector.interactable = isIdle;
        }
        foreach (var sliderAdjustor in _Adjustors)
        {
            sliderAdjustor.GetComponentInChildren<Slider>().interactable = isIdle;
        }
        if (!isIdle)
        {
            foreach (Button button in _AddBreedGenButtons)
            {
                button.interactable = false;
            }
        }
        else
        {
            _RenderBreedButtons();
        }
        _BreedButton.interactable = isIdle;
    }

    private int _CalculateBreedCost()
    {
        if (_BreedGen < 1)
        {
            return 0;
        }
        int breedCostPerGen = _CurrentFactory.PopulationCount * _CurrentFactory.BreedCostPerUnit;
        int breedCost = breedCostPerGen;    // Cost for first generation (no discount)
        // Sum cost for all gen (the more gen, the more discount)
        for (int i = 1; i < _BreedGen; i++)
        {
            // Calculate discount
            float discountRate = _CurrentFactory.DiscountRatePerGen * i;
            discountRate = (discountRate > 0.9) ? 0.9f : discountRate;       // Hard-code maximum discount to 90%=
            // Adding breed cost of this generation
            breedCost += Mathf.RoundToInt(((float)breedCostPerGen * (1f - discountRate)));
        }
        return breedCost;
    }

    // Set BreedPref
    public void SetBreedRequest()
    {
        bool isTransactionSuccess = PlayerManager.SpendMoneyIfEnought(_CalculateBreedCost());
        if (!isTransactionSuccess)
        {
            return;
        }
        // Set breed preferences
        BreedPref newBreedPref = new BreedPref(
            _Types[0].value, // Parent selection type
            _Types[1].value, // Crossover type
            (int)_Adjustors[0].GetComponentInChildren<Slider>().value, // Elitism rate
            _Types[2].value, // Mutation type
            (int)_Adjustors[1].GetComponentInChildren<Slider>().value, // Mutation rate
            _BreedGen// Breeding generation count
            );
        //newBreedPref.Print();
        _CurrentFactory.SetBreedPref(newBreedPref);
        // Set breed info that can actually breed population to new gen
        BreedInfo newBreedInfo = new BreedInfo(_CurrentFactory);
        _CurrentFactory.SetBreedRequest(newBreedInfo);
        _CurrentFactory.SetStatus(Status.BREEDING);
        RenderPanelFromPref();
        FactoryManager.Instance.RenderStatusPanel();
    }

    public struct BreedPref
    {
        private int _TypeParentSelect;
        public int TypeParentSelect => _TypeParentSelect;
        private int _TypeCrossover;
        public int TypeCrossover => _TypeCrossover;

        private int _ElitismRate;
        public int ElitismRate => _ElitismRate;
        private int _TypeMutation;
        public int TypeMutation => _TypeMutation;
        private int _MutationRate;
        public int MutationRate => _MutationRate;
        private int _BreedGen;
        public int BreedGen => _BreedGen;

        public BreedPref(int  typeParentSelection, int typeCrossover, int elitismRate, int typeMutation, int mutationRate, int breedGen)
        {
            _TypeParentSelect = typeParentSelection;
            _TypeCrossover = typeCrossover;
            _ElitismRate = elitismRate;
            _TypeMutation = typeMutation;
            _MutationRate = mutationRate;
            _BreedGen = breedGen;
        }

        public BreedPref Copy()
        {
            return new BreedPref(_TypeParentSelect, _TypeCrossover, _ElitismRate, _TypeMutation, _MutationRate, _BreedGen);
        }

        public void Print()
        {
            Debug.Log("--------------- My breed pref ---------------");
            Debug.Log("Selection = " + _TypeParentSelect);
            Debug.Log("Crossover = " + _TypeCrossover);
            Debug.Log("ElitismRate = " + _ElitismRate);
            Debug.Log("Mutation = " + _TypeMutation);
            Debug.Log("MutationRate = " + _MutationRate);
            Debug.Log("BreedGen = " + _BreedGen);
        }
    }

    public struct BreedInfo {
        private FactorySO _MyFactory;
        public FactorySO MyFactory => _MyFactory;

        public BreedInfo(FactorySO newFactory)
        {
            _MyFactory = newFactory;
        }

        public void Produce()
        {
            WeaponChromosome[] currentPopulation = _MyFactory.GetAllWeapon();
            int populationCount = currentPopulation.Length;
            // Calculate the number of elite and parent
            float elitismRate = _MyFactory.BreedPref.ElitismRate;
            int eliteCount = (int)(populationCount * elitismRate / 100);
            int parentCount = populationCount - eliteCount;
            // If the number of parent is odd number, make it even
            if (parentCount % 2 == 1)
            {
                parentCount--;
                eliteCount++;
            }

            // Elitism
            List<WeaponChromosome> currentPopulationList = new();
            currentPopulationList.AddRange(currentPopulation);
            currentPopulationList.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));
            int[][][] elites = new int[eliteCount][][];
            for (int eliteIndex = 0; eliteIndex < eliteCount; eliteIndex++)
            {
                elites[eliteIndex] = currentPopulationList[eliteIndex].Bitstring;
            }

            // Parent Selection
            Dictionary<dynamic, float> fv = new Dictionary<dynamic, float>();
            foreach (WeaponChromosome c in currentPopulation)
            {
                fv.Add(c, c.Fitness);
            }
            List<dynamic> parents = new List<dynamic>
                (GeneticFunc.Instance.SelectParent(fv, eliteCount, _MyFactory.BreedPref.TypeParentSelect, 3));
            
            // Crossvoer, code in this section is duplicated and modified from BreedMenu.cs
            List<List<List<int>>> parentsEncoded = new List<List<List<int>>>();
            // encode dem parents and add to list
            foreach (WeaponChromosome c in parents)
            {
                parentsEncoded.Add(c.GetChromosomeAsList());
            }
            // crossover each pair ex: 0-1, 2-3, ...
            for (int i = 0; i < parentsEncoded.Count - (parentsEncoded.Count % 2); i += 2)
            {
                GeneticFunc.Instance.Crossover(parentsEncoded[i], parentsEncoded[i + 1], _MyFactory.BreedPref.TypeCrossover);
            }

            // Mutation
            foreach (List<List<int>> offspring in parentsEncoded)
            {
                if (Random.Range(0, 100) < _MyFactory.BreedPref.MutationRate)
                {
                    _Mutate(offspring, _MyFactory.BreedPref.TypeMutation);
                }
            }

            // Add and convert population from list to array
            int[][][] nextPopulationArray = new int[elites.Length + parentsEncoded.Count][][];
            int assignedEiteIndex;
            // Add elites
            for (assignedEiteIndex = 0; assignedEiteIndex < elites.Length; assignedEiteIndex++)
            {
                nextPopulationArray[assignedEiteIndex] = elites[assignedEiteIndex];
            }
            // Add offsprings
            for (int j = 0; j < parentsEncoded.Count; j++)
            {
                List<List<int>> bitstringList = parentsEncoded[j];
                // Convert a 2D list to 2D array
                int[][] bitstring = new int[2][];
                for (int k = 0; k < bitstringList.Count; k++)
                {
                    bitstring[k] = bitstringList[k].ToArray();
                }
                nextPopulationArray[assignedEiteIndex + j] = bitstring;
            }
            // Set database to new population
            _MyFactory.SetBitChromoDatabase(nextPopulationArray);
        }

        // Mutate binary chromosome
        // Type: 0 = None, 1 = Bit flip, 2 = Flip bit
        private void _Mutate(List<List<int>> bitstring, int mutateType)
        {
            // Bit flip
            if (mutateType == 1)
            {
                // Simplify the method by flip exactly 1 bit
                // Random the position to flip
                int flipIndex = Random.Range(0, bitstring[0].Count);
                // Hard-code for 1D chromosome
                if (bitstring.Count == 1)
                {
                    bitstring[0][flipIndex] = 1 - bitstring[0][flipIndex];
                }
                // Hard-code for 2D chromosome
                else if (bitstring.Count == 2)
                {
                    // If the random position is not picked, pick in random knapsack
                    if (bitstring[1][flipIndex] + bitstring[0][flipIndex] == 0)
                    {
                        // Pick to first knapsack
                        if (Random.Range(0, 2) == 0)
                        {
                            bitstring[0][flipIndex] = 1;
                        }
                        // Pick to second knapsack
                        else
                        {
                            bitstring[1][flipIndex] = 1;
                        }
                    }
                    // If the position is already picked, move it to other knapsack or take it out from all
                    else if (bitstring[1][flipIndex] + bitstring[0][flipIndex] == 1)
                    {
                        // Move to other knapsack
                        if (Random.Range(0, 2) == 0)
                        {
                            bitstring[0][flipIndex] = 1 - bitstring[0][flipIndex];
                            bitstring[1][flipIndex] = 1 - bitstring[1][flipIndex];
                        }
                        // Take it out from all knapsack
                        else
                        {
                            bitstring[0][flipIndex] = 0;
                            bitstring[1][flipIndex] = 0;
                        }
                    }
                }
            }
            // Flip bit
            else if (mutateType == 2)
            {
                // Hard-code for 1D chromosome
                if (bitstring.Count == 1)
                {
                    // Flip all bit
                    for (int i = 0; i < bitstring[0].Count; i++)
                    {
                        bitstring[0][i] = 1 - bitstring[0][i];
                    }
                }
                // Hard-code for 2D chromosome
                else if (bitstring.Count == 2)
                {
                    for (int flipIndex = 0; flipIndex < bitstring[0].Count; flipIndex++)
                    {
                        // If the position is not picked, pick in random knapsack
                        if (bitstring[1][flipIndex] + bitstring[0][flipIndex] == 0)
                        {
                            // Pick to first knapsack
                            if (Random.Range(0, 2) == 0)
                            {
                                bitstring[0][flipIndex] = 1;
                            }
                            // Pick to second knapsack
                            else
                            {
                                bitstring[1][flipIndex] = 1;
                            }
                        }
                        // If the position is already picked, move it to other knapsack or take it out from all
                        else if (bitstring[1][flipIndex] + bitstring[0][flipIndex] == 1)
                        {
                            // Move to other knapsack
                            if (Random.Range(0, 2) == 0)
                            {
                                bitstring[0][flipIndex] = 1 - bitstring[0][flipIndex];
                                bitstring[1][flipIndex] = 1 - bitstring[1][flipIndex];
                            }
                            // Take it out from all knapsack
                            else
                            {
                                bitstring[0][flipIndex] = 0;
                                bitstring[1][flipIndex] = 0;
                            }
                        }
                    }
                }

            }
        }
    }
}
