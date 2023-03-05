using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentManager : MonoBehaviour
{
    public static ParentManager Instance;

    [SerializeField] private GameObject _ChromosomeRodTogglePrefab;
    [SerializeField] private Transform _ChromoButtonHolder;
    [SerializeField] private Color32[] _Colors;
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
    // puzzleType: 0 = demonstrate, 1 = solve
    public void InstaniateChromosomeRodToggles(int puzzleType = 0)
    {
        // Destroy all previous object in the holder
        foreach (Transform child in _ChromoButtonHolder)
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
                _InstantiateMechChromo();
                break;
        }
    }

    private void _InstantiateRandomChromo()
    {
        for (int i = 0; i < 6; i++)
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
            GameObject newChromosomeRodToggle = Instantiate(_ChromosomeRodTogglePrefab, _ChromoButtonHolder);
            newChromosomeRodToggle.GetComponentInChildren<ChromosomeRod>().SetChromosome(content, colors);
            newChromosomeRodToggle.GetComponentInChildren<ChromosomeRod>().RenderRod();
        }
    }

    private void _InstantiateMechChromo()
    {
        for (int i = 0; i < 4; i++)
        {
            ChromosomeSO newMech = ScriptableObject.CreateInstance<ChromosomeSO>();
            int[] content = newMech.GetChromosome().ToArray()[..5];
            Color32[] colors = new Color32[5];
            // Assign base color
            for (int j = 0; j < content.Length; j++)
            {
                colors[j] = Color.white;
            }
            // Create the ChromosomeRodToggle
            GameObject newChromosomeRodToggle = Instantiate(_ChromosomeRodTogglePrefab, _ChromoButtonHolder);
            newChromosomeRodToggle.GetComponentInChildren<ChromosomeRod>().SetChromosome(content, colors, true);
            newChromosomeRodToggle.GetComponentInChildren<ChromosomeRod>().RenderRod();
        }
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
