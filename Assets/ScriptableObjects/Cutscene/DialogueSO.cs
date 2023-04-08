using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Dialogue")]
public class DialogueSO : ScriptableObject
{
    // Variable for specifying the sentence speaker
    public enum Speaker
    {
        Player,
        NPCNormalFace,
        NPCExpressionless,
    }

    // Class for individual sentene
    [System.Serializable]
    public class Sentence
    {
        public Speaker Speaker;
        [TextArea(1, 5)] public string SentenceContent;
        public Sprite Illustration;
    }

    [SerializeField] private Sentence[] _Sentences;
    public Sentence[] Sentences => _Sentences;
}
