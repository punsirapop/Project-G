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
        NPCHandDown_EyeClose_MouthClose,
        NPCHandDown_EyeClose_MouthOpen,
        NPCHandDown_EyeOpen_MouthClose,
        NPCHandDown_EyeOpen_MouthOpen,
        NPCHandDown_Happy,
        NPCHandDown_Sad_MouthClose,
        NPCHandDown_Sad_MouthOpen,
        NPC1Hand_EyeClose_MouthClose,
        NPC1Hand_EyeClose_MouthOpen,
        NPC1Hand_EyeOpen_MouthClose,
        NPC1Hand_EyeOpen_MouthOpen,
        NPC1Hand_Happy,
        NPC1Hand_Sad_MouthClose,
        NPC1Hand_Sad_MouthOpen,
        NPC2Hand_EyeClose_MouthClose,
        NPC2Hand_EyeClose_MouthOpen,
        NPC2Hand_EyeOpen_MouthClose,
        NPC2Hand_EyeOpen_MouthOpen,
        NPC2Hand_Happy,
        NPC2Hand_Sad_MouthClose,
        NPC2Hand_Sad_MouthOpen
    }

    // Class for the individual sentence
    [System.Serializable]
    public class Sentence
    {
        public Speaker Speaker;
        [TextArea(1, 5)] public string SentenceContent;
        public Sprite Illustration;

        public Sentence Copy()
        {
            Sentence newSentence = new Sentence
            {
                Speaker = Speaker,
                SentenceContent = SentenceContent,
                Illustration = Illustration
            };
            return newSentence;
        }
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
