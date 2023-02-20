using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Factory")]
public class FactorySO : ScriptableObject
{
    // Informations
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] [TextArea] private string _Problem;
    public string Problem => _Problem;
    [SerializeField] private int _Generation;
    public int Generation => _Generation;
    [SerializeField] private string _Status;
    public string Status => _Status;
    [SerializeField] private string _WeaponPrefix;
    [SerializeField] private string _WeaponIdFormat;

    // Sprites
    [SerializeField] private Sprite _Floor;
    public Sprite Floor => _Floor;
    [SerializeField] private Sprite _Conveyor;
    public Sprite Conveyor => _Conveyor;
    [SerializeField] private Sprite _Border;
    public Sprite Border => _Border;

    // Knapsack and items preset
    [SerializeField] private KnapsackSO[] _Knapsacks;
    public KnapsackSO[] Knapsacks => _Knapsacks;
    [SerializeField] private ItemSO[] _Items;
    public ItemSO[] Items => _Items;

    // Chromosome population database
    [SerializeField] private BitChromoDatabase _ChromoDatabase;
    [SerializeField] private int _PopulationCount;
    public int PopulationCount => _PopulationCount;

    // Populate database in case if it's not populated yet
    private void _PopulateDatabaseIfNot()
    {
        if (!_ChromoDatabase.IsValid(_Knapsacks.Length, _Items.Length, _PopulationCount))
        {
            _ChromoDatabase.SetChromoDimension(_Knapsacks.Length);
            _ChromoDatabase.SetChromoLength(_Items.Length);
            _ChromoDatabase.Populate(_PopulationCount);
        }
    }

    public int[][] GetRandomBitstring()
    {
        _PopulateDatabaseIfNot();
        int index = Random.Range(0, _PopulationCount);
        return _ChromoDatabase.GetBitstringAtIndex(index);
    }

    // Return all weapon in database and its evaluated values
    public WeaponChromosome[] GetAllWeapon()
    {
        _PopulateDatabaseIfNot();
        // Create empty array of type WeaponChromosome
        WeaponChromosome[] allWeapon = new WeaponChromosome[_PopulationCount];
        int idLength = _PopulationCount.ToString().Length;
        for (int i = 0; i < _PopulationCount; i++)
        {
            // Create actual instance of WeaponChromosome
            allWeapon[i] = new WeaponChromosome();
            WeaponChromosome weaponChromosome = allWeapon[i];
            // Evaluate the bitstring
            int[][] bitstring = _ChromoDatabase.GetBitstringAtIndex(i);
            int[] values = EvaluateChromosome(bitstring);
            // Assign the WeaponChromosome according to the values
            weaponChromosome.Name = _WeaponPrefix + (i + 1).ToString(_WeaponIdFormat);
            string chromoString = "";
            foreach (int[] section in bitstring)
            {
                if (section == null)
                {
                    continue;
                }
                foreach (int bit in section)
                {
                    chromoString = chromoString + bit.ToString();
                }
                chromoString += " | ";
            }
            char[] trimChar = { ' ', '|', ' ' };
            weaponChromosome.Bitstring = chromoString.Trim(trimChar);
            weaponChromosome.Fitness = values[0];
            weaponChromosome.Weight1 = values[1];
            weaponChromosome.Weight2 = values[2];
        }
        return allWeapon;
    }

    // Since FactorySO hold both Knapsacks and Items, it should perform the evaluation task that use these information
    private int[] EvaluateChromosome(int[][] bitString)
    {
        int fitness = 0;
        int weight1 = 0;
        int weight2 = -1;
        // If the item preset have 2 weight, init weight2
        if (_Items[0].Weight2 != 0)
        {
            weight2 = 0;
        }
        // For each knapsack
        for (int kIndex = 0; kIndex < _Knapsacks.Length; kIndex++)
        {
            int[] thisKnapsackBitstring = bitString[kIndex];
            int thisKnapsackFitness = 0;
            int thisKnapsackWeight1 = 0;
            int thisKnapsackWeight2 = 0;
            // For eack item, sum all value of it
            for (int iIndex = 0; iIndex < _Items.Length; iIndex++)
            {
                // Skip to next item if it's not picked
                if (thisKnapsackBitstring[iIndex] == 0)
                {
                    continue;
                }
                thisKnapsackFitness += _Items[iIndex].Value;
                thisKnapsackWeight1 += _Items[iIndex].Weight1;
                thisKnapsackWeight2 += _Items[iIndex].Weight2;
            }
            // Set the fitness to 0 if the weight exceed the limit
            if (thisKnapsackWeight1 > _Knapsacks[kIndex].Weight1Limit)
            {
                thisKnapsackFitness = 0;
            }
            else if (thisKnapsackWeight2 > _Knapsacks[kIndex].Weight2Limit)
            {
                thisKnapsackFitness = 0;
            }
            // Sum up overall value
            fitness += thisKnapsackFitness;
            weight1 += thisKnapsackWeight1;
            if (weight2 != -1)
            {
                weight2 += thisKnapsackWeight2;
            }
        }
        int[] returnValue = { fitness, weight1, weight2 };
        return returnValue;
    }
}
