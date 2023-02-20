using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Item")]
public class ItemSO : ScriptableObject
{
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] private int _Value;
    public int Value => _Value;
    [SerializeField] private int _Weight1;
    public int Weight1 => _Weight1;
    [SerializeField] private int _Weight2;
    public int Weight2 => _Weight2;
}
