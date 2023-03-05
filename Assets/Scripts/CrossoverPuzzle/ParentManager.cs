using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentManager : MonoBehaviour
{
    public static ParentManager Instance;

    [SerializeField] private GameObject _ChromosomeRodTogglePrefab;
    [SerializeField] private Transform _ChromosomeRodsHolder;
    [SerializeField] private Color32[] _Colors;
    [SerializeField] private GameObject _DemonstrateText;
    [SerializeField] private GameObject _WantedChildPanel;

    private ChromosomeRodToggle[] _ChromosomeRodToggles;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        InstaniateChromosomeRodToggles(1);
    }

    void Update()
    {
        // Make sure that there are at most 2 buttons selected
        _ChromosomeRodToggles = GetComponentsInChildren<ChromosomeRodToggle>();
        // Count the number of selected button
        int selectCount = 0;
        foreach (ChromosomeRodToggle rodToggle in _ChromosomeRodToggles)
        {
            selectCount += rodToggle.isOn ? 1 : 0;
            rodToggle.SetInteractable(true);
        }
        // Disable other if there are already 2 buttons selected
        if (selectCount < 2)
        {
            return;
        }
        selectCount = 0;
        foreach (ChromosomeRodToggle rodToggle in _ChromosomeRodToggles)
        {
            if (!rodToggle.isOn)
            {
                rodToggle.SetInteractable(false);
            }
            else
            {
                if (selectCount >= 2)
                {
                    rodToggle.SetIsOn(false);
                    rodToggle.SetInteractable(false);
                }
                selectCount++;
            }
        }
    }

    // Create ChromosomeRodToggle correspond to the given puzzleType
    // puzzleType: 0 = demonstrate, 1 = solve single point, 2 = solve two point
    public void InstaniateChromosomeRodToggles(int puzzleType = 0)
    {
        // Destroy all previous object in the holder
        foreach (Transform child in _ChromosomeRodsHolder)
        {
            Destroy(child.gameObject);
        }
        switch (puzzleType)
        {
            default:
                _InstantiateRandomChromo();
                break;
            case 0:
                _InstantiateRandomChromo();
                break;
            case 1:
                _InstantiateMechChromo(0);
                break;
            case 2:
                _InstantiateMechChromo(1);
                break;
        }
    }

    private void _InstantiateRandomChromo()
    {
        for (int i = 0; i < 4; i++)
        {
            // Generate content in chromosome
            int[] content = new int[5];
            Color32[] colors = new Color32[5];
            for (int j = 0; j < content.Length; j++)
            {
                content[j] = Random.Range(0, 2);
                colors[j] = _Colors[i];
            }
            // Create the ChromosomeRodToggle
            GameObject newChromosomeRodToggle = Instantiate(_ChromosomeRodTogglePrefab, _ChromosomeRodsHolder);
            newChromosomeRodToggle.GetComponentInChildren<ChromosomeRod>().SetChromosome(content, colors);
            newChromosomeRodToggle.GetComponentInChildren<ChromosomeRod>().RenderRod();
        }
        _DemonstrateText.SetActive(true);
        _WantedChildPanel.SetActive(false);
    }

    private void _InstantiateMechChromo(int crossoverType)
    {
        // Create wanted children
        int[] child1 = ScriptableObject.CreateInstance<ChromosomeSO>().GetChromosome().ToArray()[..5];
        int[] child2 = ScriptableObject.CreateInstance<ChromosomeSO>().GetChromosome().ToArray()[..5];
        // Create possible parent using crossover
        int[][] parents = new int[4][];
        for (int parentCount = 0; parentCount < 4; parentCount += 2)
        {
            List<int> parent1 = new();
            List<int> parent2 = new();
            parent1.AddRange(child1);
            parent2.AddRange(child2);
            GeneticFunc.Instance.Crossover(parent1, parent2, crossoverType);
            parents[0 + parentCount] = parent1.ToArray();
            parents[1 + parentCount] = parent2.ToArray();
        }
        // Shuffle parent's order
        for (int parentIndex = 0; parentIndex < parents.Length; parentIndex++)
        {
            // Swap this parent with another random parent
            int randomIndex = Random.Range(0, parents.Length);
            int[] thisParent = parents[parentIndex];
            parents[parentIndex] = parents[randomIndex];
            parents[randomIndex] = thisParent;
        }
        // Assign base color to all white
        Color32[] baseColor = new Color32[child1.Length];
        for (int i = 0; i < baseColor.Length; i++)
        {
            baseColor[i] = Color.white;
        }
        // Create the ChromosomeRodToggle of parent
        foreach (int[] parent in parents)
        {
            GameObject newChromosomeRodToggle = Instantiate(_ChromosomeRodTogglePrefab, _ChromosomeRodsHolder);
            newChromosomeRodToggle.GetComponentInChildren<ChromosomeRod>().SetChromosome(parent, baseColor, true);
            newChromosomeRodToggle.GetComponentInChildren<ChromosomeRod>().RenderRod();
        }
        // Show one of the wanted children
        _DemonstrateText.SetActive(false);
        _WantedChildPanel.SetActive(true);
        GameObject childChromosomeRodToggle = Instantiate(_ChromosomeRodTogglePrefab, _WantedChildPanel.transform);
        childChromosomeRodToggle.GetComponentInChildren<ChromosomeRod>().SetChromosome(child1, baseColor, true);
        childChromosomeRodToggle.GetComponentInChildren<ChromosomeRod>().RenderRod();
    }

// Return all chromosomeRods that is selected
public ChromosomeRod[] GetSelectedChromosomeRods()
    {
        // Count the number of selected chromosomeRodToggle
        _ChromosomeRodToggles = GetComponentsInChildren<ChromosomeRodToggle>();
        int selectCount = 0;
        foreach (ChromosomeRodToggle rodToggle in _ChromosomeRodToggles)
        {
            if (rodToggle.isOn)
            {
                selectCount++;
            }
        }
        // Create the array of selected chromosomeRod
        ChromosomeRod[] selectedRods = new ChromosomeRod[selectCount];
        selectCount = 0;
        foreach (ChromosomeRodToggle rodToggle in _ChromosomeRodToggles)
        {
            if (rodToggle.isOn)
            {
                selectedRods[selectCount] = rodToggle.GetComponentInChildren<ChromosomeRod>();
                selectCount++;
            }
        }
        return selectedRods;
    }    
}
