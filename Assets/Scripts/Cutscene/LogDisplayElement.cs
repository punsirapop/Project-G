using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogDisplayElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _SpeakerNameText;
    [SerializeField] private GameObject _SpeakerNameSep;
    [SerializeField] private TextMeshProUGUI _Sentence;
    [SerializeField] private Color _SelectedChoiceColor;

    public void SetLog(CutsceneManager.Log log)
    {
        // Display speaker name
        if (log.SpeakerName == "")
        {
            // No name, set them disable
            _SpeakerNameText.gameObject.SetActive(false);
            _SpeakerNameSep.SetActive(false);
        }
        else
        {
            // Has name, show them
            _SpeakerNameText.gameObject.SetActive(true);
            _SpeakerNameText.text = log.SpeakerName;
            _SpeakerNameSep.SetActive(true);
        }
        // Display sentence
        _Sentence.text = log.SentenceContent;
        // If it a selected choice, highlight it
        if (log.IsSelected)
        {
            _Sentence.color = _SelectedChoiceColor;
        }
        // Resizing height of sentence element to fit the content
        RectTransform sentenceRectTransform = _Sentence.GetComponent<RectTransform>();
        Vector2 sentenceSizeDelta = sentenceRectTransform.sizeDelta;
        sentenceSizeDelta.y = _Sentence.preferredHeight;
        sentenceRectTransform.sizeDelta = sentenceSizeDelta;
        // Resizing this object size
        RectTransform thisRectTransform = GetComponent<RectTransform>();
        Vector2 thisSizeDelta = thisRectTransform.sizeDelta;
        thisSizeDelta.y = _Sentence.preferredHeight;
        thisRectTransform.sizeDelta = thisSizeDelta;
    }
}
