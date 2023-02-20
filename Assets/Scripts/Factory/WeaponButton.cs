using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _Name;
    private WeaponChromosome _Chromosome;

    public void SetChromosome(WeaponChromosome chromosome)
    {
        _Chromosome = chromosome;
        _Name.text = _Chromosome.Name;
    }

    public void Print()
    {
        string info = "";
        info += "Name: " + _Chromosome.Name;
        info += " , Bitstring: " + _Chromosome.Bitstring + "\n";
        info += "F = " + _Chromosome.Fitness.ToString();
        info += " , W1 = " + _Chromosome.Weight1.ToString();
        info += " , W2 = " + _Chromosome.Weight2.ToString();
        Debug.Log(info);
    }
}
