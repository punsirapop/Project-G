using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _ChoiceText;

    public void SetChoiceButton(DialogueElement.Choice choiceData)
    {
        _ChoiceText.text = choiceData.SentenceContent;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Choose choice: " + choiceData.SentenceContent+" "+choiceData.number);
            CutsceneManager.Instance.SelectChoice(choiceData);
        });
    }
}
