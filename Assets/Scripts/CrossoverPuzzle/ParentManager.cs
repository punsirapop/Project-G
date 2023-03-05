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
        // Destroy all previous object in the holder
        foreach (Transform child in _ChromoButtonHolder)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 6; i++)
        {
            int[] content = new int[5];
            Color32[] colors = new Color32[5];
            for (int j = 0; j < content.Length; j++)
            {
                content[j] = Random.Range(0, 2);
                colors[j] = _Colors[i];
            }
            GameObject newChromosomeRodToggle = Instantiate(_ChromosomeRodTogglePrefab, _ChromoButtonHolder);
            newChromosomeRodToggle.GetComponentInChildren<ChromosomeRod>().SetChromosome(content, colors);
        }
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
