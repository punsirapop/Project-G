using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/OneDimChromoDatabase")]
public class OneDimChromoDatabase : ScriptableObject
{
    // Class for individual chromosome
    [System.Serializable]
    public class OneDimChromosome
    {
        public int[] BitString = new int[10];
    }

    // Population array
    [SerializeField] private OneDimChromosome[] _Population = new OneDimChromosome[100];

    public void ResetPopulation()
    {
        // Make 1 bit at pickIndex to be 1, representing picking only one item
        int pickIndex = 0;
        // Loop over number of fixed population
        foreach (OneDimChromosome chromosome in _Population)
        {
            chromosome.BitString[pickIndex] = 1;
            pickIndex++;
            if (pickIndex >= 10)
            {
                pickIndex = 0;
            }
        }
    }
}