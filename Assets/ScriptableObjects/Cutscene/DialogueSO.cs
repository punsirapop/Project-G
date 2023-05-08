using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Dialogue")]
public class DialogueSO : ScriptableObject
{
    [SerializeField] public int[] ChoiceAnswers;
    [SerializeField] public int PassScore;
    [SerializeField] public string SpeakerName;
    [SerializeField] public bool IsSkippable;
    [SerializeField] [TextArea] public string SkipNotifyText;
    [SerializeField] private DialogueElement[] _Elements;
    public DialogueElement[] Elements => _Elements;
}
