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

    public void SetDatabase(int[][][] newBitstringArray, bool isShuffle=true)
    {
        if (_Population.Length != newBitstringArray.Length)
        {
            Debug.Log("Cannot set new database, length not equal");
            return;
        }

        // Use Fisher-Yates shuffle algorithm to shuffle the element
        if (isShuffle)
        {
            int[][] arrayToShuffle = newBitstringArray[0];
            // Shuffle the last element to any random element
            System.Random rng = new System.Random();
            int n = newBitstringArray.Length;
            // Loop from the last element to first element
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int[][] temp = newBitstringArray[k];
                newBitstringArray[k] = newBitstringArray[n];
                newBitstringArray[n] = temp;
            }
        }

        for (int i = 0; i < newBitstringArray.Length; i++)
        {
            BitChromosome newChromo = new BitChromosome();
            newChromo.Bitstring1 = newBitstringArray[i][0];
            newChromo.Bitstring2 = newBitstringArray[i][1];
            _Population[i] = newChromo;
        }
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

    // Return a random bitstring corresponding to the given bit1 count
    public int[][] GenerateRandomBitstring(int bit1Count)
    {
        int[] bitstring1 = new int[_ChromoLength];
        int[] bitstring2 = new int[_ChromoLength];
        // Random the position to be bit1
        int[] randomIndex = _RandomChoices(_GenerateRandomIndexPool(), bit1Count);
        foreach (int index in randomIndex)
        {
            // Hard-code calculate section only for chromosome with no more thant 2 dimension
            int bitstringSection = (index > _ChromoLength - 1) ? 1 : 0;
            int bitstringIndex = index % _ChromoLength;
            if (bitstringSection == 0)
            {
                if (bitstring2[bitstringIndex] != 1)
                {
                    bitstring1[bitstringIndex] = 1;
                }
            }
            else
            {
                if (bitstring1[bitstringIndex] != 1)
                {
                    bitstring2[bitstringIndex] = 1;
                }
            }
        }
        int[][] bitstring = { bitstring1, bitstring2 };
        return bitstring;
    }

    // Return a random pool for the index int bitstring[] for _RandomChoices purpose
    private int[] _GenerateRandomIndexPool()
    {
        int[] randomPool = new int[_ChromoDimension * _ChromoLength];
        for (int i = 0; i < randomPool.Length; i++)
        {
            randomPool[i] = i;
        }
        return randomPool;
    }

    // Return a number of random distinct value from the randomPool equal to the number of randomCount
    private int[] _RandomChoices(int[] randomPool, int randomCount)
    {
        if (randomPool.Length < randomCount)
        {
            return null;
        }
        else if (randomPool.Length == randomCount)
        {
            return randomPool;
        }
        int[] currentRandomPool = randomPool;
        int[] resultPool = new int[randomCount];
        for (int i = 0; i < randomCount; i++)
        {
            // Get new random value in pool
            int newRandomIndex = Random.Range(0, currentRandomPool.Length);
            resultPool[i] = currentRandomPool[newRandomIndex];
            // Remove such value from the pool
            int[] newRandomPool = new int[currentRandomPool.Length - 1];
            for (int j = 0; j < currentRandomPool.Length - 1; j++)
            {
                newRandomPool[j] = (j >= newRandomIndex) ? currentRandomPool[j + 1] : currentRandomPool[j];
            }
            currentRandomPool = newRandomPool;
        }
        return resultPool;
    }
}
