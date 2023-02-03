using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Store various miscellanous functions
 */
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
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

    public void AddChromo()
    {
        List<int> c = new List<int> ();
        for (int i = 0; i < 17; i++) c.Add(0);
        ChromosomeSC chromosome = ScriptableObject.CreateInstance<ChromosomeSC>();
        // chromosome.SetChromosome(c);
        Chromosomes[CurrentFarm].Add(chromosome);
        OnAddChromosome?.Invoke(chromosome);
    }

    public void DelChromo(ChromosomeSC c)
    {
        Chromosomes[CurrentFarm].Remove(c);
        OnRemoveChromosome?.Invoke(c);
    }
}
