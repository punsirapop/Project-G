using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple class that hold information of each weapon chromosome
[Serializable]
public class WeaponChromosome
{
    // Note: use auto-implemented property like { get; private set; } to shorten the declaration code
    // Data that fix to the factory
    public int FromFactory { get; private set; }
    public WeaponMode Mode1 { get; private set; }
    public WeaponMode Mode2 { get; private set; }
    public bool IsMode1Active { get; private set; } // true mean this weapon is on Mode1. Otherwise, it's on Mode2
    public WeaponMode CurrentMode => IsMode1Active ? Mode1 : Mode2;
    // Data that related to weapon's rank
    public WeaponRank Rank { get; private set; }
    public Sprite Image { get; private set; }
    public Sprite BigImage { get; private set; }
    public string Name { get; private set; }
    public int[][] Bitstring { get; private set; }
    public int Fitness { get; private set; }
    public int Weight1 { get; private set; }
    public int Weight2 { get; private set; } = -1;  // Use -1 representing this has no Weight2
    // Data that directly effect the battle
    public float Efficiency { get; private set; }   // Goodness of weapon vary from 0.0 to 1.0
    public float Cooldown { get; private set; }     // Cooldown in seconds
    public WeaponStat BonusStat { get; private set; }

    public struct WeaponStat
    {
        public int Atk;
        public int Def;
        public int Hp;
        public int Spd;

        public WeaponStat(int atk, int def, int hp, int spd)
        {
            Atk = atk;
            Def = def;
            Hp = hp;
            Spd = spd;
        }
    }

    public WeaponChromosome(int fromFactory, WeaponMode mode1, WeaponMode mode2, bool isMode1Active,
        WeaponRank rank, Sprite image, Sprite bigImage, string name, int[][] bitstring, int fitness, int weight1, int weight2,
        float efficiency, float cooldown, int atk, int def, int hp, int spd)
    {
        FromFactory = fromFactory;
        Mode1 = mode1;
        Mode2 = mode2;
        IsMode1Active = isMode1Active;
        Rank = rank;
        Image = image;
        BigImage = bigImage;
        Name = name;
        Bitstring = bitstring;
        Fitness = fitness;
        Weight1 = weight1;
        Weight2 = weight2;
        Efficiency = efficiency;
        Cooldown = cooldown;
        BonusStat = new WeaponStat(atk, def, hp, spd);
    }

    // Setter used for change current mode in the battle preparation scene
    public void SetIsMode1Active(bool newIsMode1IsActive)
    {
        IsMode1Active = newIsMode1IsActive;
    }

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

    // Return bitstring as 2D list for the breeding purpose
    public List<List<int>> GetChromosomeAsList()
    {
        List<int> c = new List<int>();
        List<List<int>> holder = new List<List<int>>();
        foreach (int[] arr in Bitstring)
        {
            if (arr == null)
            {
                break;
            }
            List<int> list = new List<int>(arr);
            holder.Add(list);
        }
        return holder;
    }
}

public enum WeaponRank { S, A, B, C, D }

public enum WeaponMode
{
    Taunt,
    Stealth,
    Snipe,
    Pierce,
    Sleep,
    Poison,
    AOEHeal,
    AOEDamage,
}