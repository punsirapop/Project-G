using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Dialogue")]
public class DialogueSO : ScriptableObject
{
    [SerializeField] private DialogueElement[] _Elements;
    public DialogueElement[] Elements => _Elements;
}