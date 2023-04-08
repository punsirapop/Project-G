using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    // UI element for display data
    [SerializeField] private TextMeshProUGUI _SpeakerName;
    [SerializeField] private TextMeshProUGUI _SentenceText;
    [SerializeField] private float _TypeDelaySeconds;
    [SerializeField] private HorizontalLayoutGroup _CharacterLayout;
    [SerializeField] private Sprite _CharacterNormalFace;
    [SerializeField] private Sprite _CharacterExpressionless;
    [SerializeField] private Image _CharacterImage;
    [SerializeField] private Image _IllustrationImage;
    // Variable related to dialogue data
    [SerializeField] private DialogueSO _CurrentDialogueSO;
    private DialogueSO.Sentence _CurrentSentence => _CurrentDialogueSO.Sentences[_CurrentSentenceIndex];
    private int _CurrentSentenceIndex;

    void Start()
    {
        _CurrentSentenceIndex = 0;
        DisplaySentence();
    }

    public void DisplaySentence()
    {
        // Set illustration and character position
        Sprite illustration = _CurrentSentence.Illustration;
        if (illustration != null)
        {
            _CharacterLayout.childAlignment = TextAnchor.LowerRight;
            _IllustrationImage.gameObject.SetActive(true);
            _IllustrationImage.sprite = illustration;
        }
        else
        {
            _CharacterLayout.childAlignment = TextAnchor.LowerCenter;
            _IllustrationImage.gameObject.SetActive(false);
        }
        // Set sprite, name, and color of speaker
        DialogueSO.Speaker speaker = _CurrentSentence.Speaker;
        _CharacterImage.color = (speaker == DialogueSO.Speaker.Player) ? Color.gray : Color.white;
        if (speaker == DialogueSO.Speaker.NPCNormalFace)
        {
            _CharacterImage.sprite = _CharacterNormalFace;
        }
        else if (speaker == DialogueSO.Speaker.NPCExpressionless)
        {
            _CharacterImage.sprite = _CharacterExpressionless;
        }
        _SpeakerName.text = speaker.ToString();
        // Start sentence text displaying
        StartCoroutine(TypeSentence());
    }

    public void DisplayNextSentence()
    {
        // Immediately finish current sentence in case it's not
        if (_SentenceText.text != _CurrentSentence.SentenceContent)
        {
            _SentenceText.text = _CurrentSentence.SentenceContent;
            StopAllCoroutines();
            return;
        }
        // Move on next sentence
        _CurrentSentenceIndex++;
        if (_CurrentSentenceIndex < _CurrentDialogueSO.Sentences.Length)
        {
            DisplaySentence();
        }
        else
        {
            Debug.Log("End of dialogue");
        }
    }

    // Function for gradually display the sentence text
    IEnumerator TypeSentence()
    {
        _SentenceText.text = "";
        // Type each character with some delay
        foreach (char c in _CurrentSentence.SentenceContent.ToCharArray())
        {
            _SentenceText.text += c;
            yield return new WaitForSeconds(_TypeDelaySeconds);
        }
    }
}
