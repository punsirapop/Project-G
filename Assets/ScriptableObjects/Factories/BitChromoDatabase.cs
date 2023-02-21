using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/BitChromoDatabase")]
public class BitChromoDatabase : ScriptableObject
{
    // Class for individual chromosome
    [System.Serializable]
    public class BitChromosome
    {
        public int[] Bitstring1;
        public int[] Bitstring2;
    }

    // Number of chromosome dimension
    [SerializeField] private int _ChromoDimension = -1;
    [SerializeField] private int _ChromoLength = 0;
    // Population array
    [SerializeField] private BitChromosome[] _Population;

    // Return false if database not match the given setting value
    public bool IsValid(int chromoDimension, int chromoLength, int populationCount)
    {
        if (_ChromoDimension != chromoDimension)
        {
            return false;
        }
        if (_ChromoLength != chromoLength)
        {
            return false;
        }
        if (_Population.Length != populationCount)
        {
            return false;
        }
        return true;
    }

    public void SetChromoDimension(int chromoDimension)
    {
        _ChromoDimension = chromoDimension;
    }

    public void SetChromoLength(int chromoLength)
    {
        _ChromoLength = chromoLength;
    }

    public void Populate(int populationCount)
    {
        // Create empty array of type BitChromosome with size of populactionCount
        _Population = new BitChromosome[populationCount];
        // Make 1 bit at pickIndex to be 1, representing picking only one item
        int pickIndex = 0;
        int pickBitstring = 1;
        // Loop over number of fixed population
        for (int popIndex = 0; popIndex < _Population.Length; popIndex++)
        {
            // Create actual BitChromosome instance
            _Population[popIndex] = new BitChromosome();
            BitChromosome chromosome = _Population[popIndex];
            chromosome.Bitstring1 = new int[_ChromoLength];
            if (_ChromoDimension == 2)
            {
                chromosome.Bitstring2 = new int[_ChromoLength];
            }
            // Assign bit to corresponding section
            if (pickBitstring == 1)
            {
                chromosome.Bitstring1[pickIndex] = 1;
                pickIndex++;
                if (pickIndex >= _ChromoLength)
                {
                    pickIndex = 0;
                    if (_ChromoDimension == 2)
                    {
                        pickBitstring = 2;
                    }
                }
            }
            else if (pickBitstring == 2)
            {
                chromosome.Bitstring2[pickIndex] = 1;
                pickIndex++;
                if (pickIndex >= _ChromoLength)
                {
                    pickIndex = 0;
                    if (_ChromoDimension == 2)
                    {
                        pickBitstring = 1;
                    }
                }
            }
        }
    }

    // Return a random bitstring from the population
    public int[][] GetBitstringAtIndex(int index)
    {
        int[] bitstring1 = _Population[index].Bitstring1;
        int[] bitstring2 = _Population[index].Bitstring2;
        int[][] bitstring = { bitstring1, bitstring2 };
        return bitstring;
    }
}
