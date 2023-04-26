using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueElement
{
    // Variable for specifying the sentence speaker
    public enum Speaker
    {
        Player,
        NPCNormalFace,
        NPCExpressionless,
        NPCHappy,
        NPCNervous
    }

    // Class for the individual sentence
    [System.Serializable]
    public class Sentence
    {
        public Speaker Speaker;
        [TextArea(1, 5)] public string SentenceContent;
        public Sprite Illustration;
    }

    // Class for the individual choice
    [System.Serializable]
    public class Choice
    {
        [TextArea(1, 5)] public string SentenceContent;
        public Sentence[] ReponseData;
    }

    public bool IsChoices;
    public Sentence SentenceData;
    public Choice[] Choices;
}
