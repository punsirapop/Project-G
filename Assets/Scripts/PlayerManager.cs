using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Store various miscellanous functions
 * Still don't know how to categorize them
 * - AddChromo
 * - DelChromo
 */
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    // Store every chr. the player possesses
    // 0: Habitat, 1-3: Farms
    public static List<List<ChromosomeSC>> Chromosomes;
    public static int CurrentFarm = 1;

    public static event Action<ChromosomeSC> OnAddChromosome;
    public static event Action<ChromosomeSC> OnRemoveChromosome;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        
        Chromosomes = new List<List<ChromosomeSC>>();
        for (int i = 0; i < 4; i++)
        {
            Chromosomes.Add(new List<ChromosomeSC>());
        }
    }

    // Add new random chromosome to the current space
    public void AddChromo()
    {
        ChromosomeSC chromosome = ScriptableObject.CreateInstance<ChromosomeSC>();
        Chromosomes[CurrentFarm].Add(chromosome);
        OnAddChromosome?.Invoke(chromosome);
    }

    // Delete a chromosome from the current space
    public void DelChromo(ChromosomeSC c)
    {
        Chromosomes[CurrentFarm].Remove(c);
        OnRemoveChromosome?.Invoke(c);
    }
}
