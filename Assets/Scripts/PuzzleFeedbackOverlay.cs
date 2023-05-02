using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleFeedbackOverlay : MonoBehaviour
{
    [SerializeField] private GameObject _Box;
    [SerializeField] private TextMeshProUGUI _HeaderText;
    [SerializeField] private TextMeshProUGUI _FeedbackText;
    [SerializeField] private Sprite _PassSprite;
    [SerializeField] private Sprite _FailSprite;

    // Set background and feedback text
    public void SetFeedBack(bool isPass, string headerText, string feedbackText)
    {
        _Box.GetComponent<Image>().sprite = isPass ? _PassSprite : _FailSprite;
        _HeaderText.text = headerText;
        _FeedbackText.text = feedbackText;
    }

    // (deprecated)
    public void SetFeedBack(bool isPass, string feedbackText)
    {
        _Box.GetComponent<Image>().sprite = isPass ? _PassSprite : _FailSprite;
        _HeaderText.text = isPass ? "Success" : "Fail";
        _FeedbackText.text = feedbackText;
    }
}
