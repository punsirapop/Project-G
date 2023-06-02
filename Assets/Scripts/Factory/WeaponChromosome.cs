using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChromosome
{
    // Simple class that hold information of each weapon chromosome
    public Sprite Image;
    public Sprite BigImage;
    public string Name;
    public int[][] Bitstring;
    public int Fitness;
    public int Weight1;
    public int Weight2 = -1; // Use -1 representing this has no Weight2

    // Temporaly overide the ToString method to monitoring the object
    public new string ToString()
    {
        string toReturn = "";
        toReturn += "Name:" + Name;
        // Formatting bitstring
        string bitstring = "";
        foreach (int[] section in Bitstring)
        {
            if (section == null)
            {
                break;
            }
            foreach (int bit in section)
            {
                bitstring += bit.ToString();
            }
            bitstring += "-";
        }
        toReturn += ", Bitstring:" + bitstring.Trim('-');
        toReturn += ", Fitness:" + Fitness.ToString();
        toReturn += ", W1:" + Weight1.ToString();
        toReturn += ", W2:" + Weight2.ToString();
        return toReturn;
    }
}
