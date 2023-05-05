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
    [SerializeField] public string ChangeScene;
    [SerializeField] public string SpeakerName;
    [SerializeField] private DialogueElement[] _Elements;
    public DialogueElement[] Elements => _Elements;
}
