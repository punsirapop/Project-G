
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Information")]
public class InformationSO : ScriptableObject
{
    [SerializeField] private Sprite[] _Images;
    public Sprite[] Images => _Images;
    public bool IsNeverShow { get; private set; }   // Boolean for auto-show tutorial for the first time

    private void OnEnable()
    {
        SaveManager.OnReset += Reset;
    }

    private void OnDestroy()
    {
        SaveManager.OnReset -= Reset;
    }

    public void Reset()
    {
        IsNeverShow = true;
    }

    public void Shown()
    {
        IsNeverShow = false;
    }
    
    // Save - Load
    public void Load(bool b)
    {
        IsNeverShow = b;
    }
}
