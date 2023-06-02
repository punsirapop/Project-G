using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponButton : MonoBehaviour
{
    private WeaponChromosome _Chromosome;
    [SerializeField] private Image _Image;
    [SerializeField] private TextMeshProUGUI _Name;
    [SerializeField] private TextMeshProUGUI _Fitness;

    public void SetChromosome(WeaponChromosome chromosome)
    {
        _Chromosome = chromosome;
        _Image.sprite = _Chromosome.Image;
        _Name.text = _Chromosome.Name;
        _Fitness.text = _Chromosome.Fitness.ToString();
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
