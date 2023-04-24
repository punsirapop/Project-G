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
        //Choice number
        public int number;
    }

    // Class for the checker respond
    [System.Serializable]
    public class Checker
    {
        public Sentence Pass;
        public Sentence Fail;
    }

    public bool IsChoices;
    //For check the score
    public bool IsChecker;
    public Sentence SentenceData;
    public Choice[] Choices;
    public Checker CheckerAnswer;
}
