using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

/*
 * Store various miscellanous functions
 * Still don't know how to categorize them
 * - AddChromo
 * - DelChromo
 */
public class PlayerManager : MonoBehaviour, ISerializationCallbackReceiver
{
    // public static PlayerManager Instance;
    // Store every chr. the player possesses
    // 0: Habitat, 1-3: Farms
    public static List<List<ChromosomeSO>> Chromosomes;
    // List storing current breeding generation for each farm
    public static List<int> CurrentGen;
    public static int CurrentPlace = 1;
    public static int CurrentFactoryIndex = 0;

    public static FactorySO[] FactoryDatabase;
    public static FactorySO CurrentFactoryDatabase => FactoryDatabase[CurrentFactoryIndex];
    [SerializeField] private FactorySO[] FactoryDatabaseHelper;

    public static event Action<ChromosomeSO> OnAddChromosome;
    public static event Action<ChromosomeSO> OnRemoveChromosome;

    private void Awake()
    {
        // if(Instance == null) Instance = this;
        if(Chromosomes == null)
        {
            Chromosomes = new List<List<ChromosomeSO>>();
            for (int i = 0; i < 4; i++)
            {
                Chromosomes.Add(new List<ChromosomeSO>());
            }
        }
        if(CurrentGen == null)
        {
            CurrentGen = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                CurrentGen.Add(0);
            }
        }
    }

    // Assign factories data from serialized field on editor to the static variable
    public void OnAfterDeserialize()
    {
        FactoryDatabase = FactoryDatabaseHelper;
    }

    // Reflect the value back into editor
    public void OnBeforeSerialize()
    {
        FactoryDatabaseHelper = FactoryDatabase;
    }



    // Change current factory
    public void SetCurrentFactoryIndex(int newFactoryIndex)
    {
        CurrentFactoryIndex = newFactoryIndex;
    }

    // Add new random chromosome to the current space
    public void AddChromo()
    {
        ChromosomeSO chromosome = ScriptableObject.CreateInstance<ChromosomeSO>();
        Chromosomes[CurrentPlace].Add(chromosome);
        OnAddChromosome?.Invoke(chromosome);
    }

    // Delete a chromosome from the current space
    public void DelChromo(ChromosomeSO c)
    {
        Chromosomes[CurrentPlace].Remove(c);
        OnRemoveChromosome?.Invoke(c);
    }
}
