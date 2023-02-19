using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Factory")]
public class FactorySO : ScriptableObject
{
    // Informations
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] [TextArea] private string _Problem;
    public string Problem => _Problem;
    [SerializeField] private int _Generation;
    public int Generation => _Generation;
    [SerializeField] private string _Status;
    public string Status => _Status;

    // Sprites
    [SerializeField] private Sprite _Floor;
    public Sprite Floor => _Floor;
    [SerializeField] private Sprite _Conveyor;
    public Sprite Conveyor => _Conveyor;
    [SerializeField] private Sprite _Border;
    public Sprite Border => _Border;
}
