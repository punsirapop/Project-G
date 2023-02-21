using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChromosome
{
    // Simple class that hold information of each weapon chromosome
    public Sprite Image;
    public Sprite BigImage;
    public string Name;
    public string Bitstring;
    public int Fitness;
    public int Weight1;
    public int Weight2 = -1; // Use -1 representing this has no Weight2
}
