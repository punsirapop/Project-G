using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MechButton : MonoBehaviour
{
    // MechChromoSO _Chromosome;
    [SerializeField] MechDisplayController _Mech;
    [SerializeField] TextMeshProUGUI _Name;
    [SerializeField] Image[] _Images;
    [SerializeField] TextMeshProUGUI _Fitness;

    public void SetChromosome(ChromoMenu.OrderFormat c)
    {
        // _Chromosome = c.chromo;
        _Mech.SetChromo(c.chromo);
        _Name.text = c.name;
        _Fitness.text = c.fitness.ToString();

        foreach (var pair in _Images.Zip(_Mech.myRenderer, (a, b) => Tuple.Create(a, b)))
        {
            pair.Item1.sprite = pair.Item2.sprite;
        }
    }

    /*
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
    */
}
