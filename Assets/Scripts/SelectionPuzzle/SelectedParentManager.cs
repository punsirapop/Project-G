using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedParentManager : MonoBehaviour
{
    public static SelectedParentManager Instance;

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

    public void AddSelectedChromosome(ChromosomeRod selectedChromosome)
    {
        Instantiate(selectedChromosome, _ChromosomeHolder);
    }
}
