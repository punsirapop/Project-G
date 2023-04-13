using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    // UI element for display data
    [SerializeField] private TextMeshProUGUI _SpeakerName;
    [SerializeField] private TextMeshProUGUI _SentenceText;
    [SerializeField] private HorizontalLayoutGroup _CharacterLayout;
    [SerializeField] private Sprite[] _NPCFaceSprites;
    [SerializeField] private Image _CharacterImage;
    [SerializeField] private Image _IllustrationImage;
    [SerializeField] private Transform _ChoicesHolder;
    [SerializeField] private GameObject _ChoiceButtonPrefab;
    // Variable related to dialogue data
    [SerializeField] private DialogueSO _CurrentDialogueSO;
    private DialogueElement CurrentDialogue => _CurrentDialogueSO.Elements[_CurrentSentenceIndex];
    private int _CurrentSentenceIndex;
    [SerializeField] private float _TypeDelaySeconds;
    // Variable related to choice' response
    private bool _IsChoiceResponseDisplaying;
    private bool _IsWaitingChoiceSelect;
    private DialogueElement.Sentence[] _ChoiceResponses;
    private int _CurrentChoiceResponseIndex;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _CurrentSentenceIndex = 0;
        _IsChoiceResponseDisplaying = false;
        _IsWaitingChoiceSelect = false;
        DisplaySentence(CurrentDialogue.SentenceData);
    }

    public void DisplaySentence(DialogueElement.Sentence currentSentence)
    {
        // Set illustration and character position
        Sprite illustration = currentSentence.Illustration;
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
        DialogueElement.Speaker speaker = currentSentence.Speaker;
        _CharacterImage.color = (speaker == DialogueElement.Speaker.Player) ? Color.gray : Color.white;
        switch (speaker)
        {
            case DialogueElement.Speaker.NPCNormalFace:
                _CharacterImage.sprite = _NPCFaceSprites[0];
                break;
            case DialogueElement.Speaker.NPCExpressionless:
                _CharacterImage.sprite = _NPCFaceSprites[1];
                break;
            case DialogueElement.Speaker.NPCHappy:
                _CharacterImage.sprite = _NPCFaceSprites[2];
                break;
            case DialogueElement.Speaker.NPCNervous:
                _CharacterImage.sprite = _NPCFaceSprites[3];
                break;
        }
        _SpeakerName.text = speaker.ToString();
        // Start sentence text displaying
        StartCoroutine(TypeSentence(currentSentence));
    }

    // Function for gradually display the sentence text
    IEnumerator TypeSentence(DialogueElement.Sentence currentSentence)
    {
        _SentenceText.text = "";
        // Type each character with some delay
        foreach (char c in currentSentence.SentenceContent.ToCharArray())
        {
            _SentenceText.text += c;
            yield return new WaitForSeconds(_TypeDelaySeconds);
        }
    }

    // Spawn overlay choices
    public void SpawnChoices(DialogueElement.Choice[] choices)
    {
        // Active holder
        _ChoicesHolder.gameObject.SetActive(true);
        // Destroy all previous choices
        foreach (Transform child in _ChoicesHolder)
        {
            Destroy(child.gameObject);
        }
        // Spawn all new choices
        foreach(DialogueElement.Choice choiceData in choices)
        {
            GameObject newChoiceButton = Instantiate(_ChoiceButtonPrefab, _ChoicesHolder);
            newChoiceButton.GetComponent<ChoiceButton>().SetChoiceButton(choiceData);
        }
        _IsWaitingChoiceSelect = true;
    }

    // Set the response of the choice
    public void SelectChoice(DialogueElement.Sentence[] newChoiceResponse)
    {
        _ChoiceResponses = newChoiceResponse;
        _CurrentChoiceResponseIndex = -1;
        _IsChoiceResponseDisplaying = true;
        _IsWaitingChoiceSelect = false;
        _ChoicesHolder.gameObject.SetActive(false);
        DisplayNextChoiceResponse();
    }

    public void DisplayNextChoiceResponse()
    {
        _CurrentChoiceResponseIndex++;
        if (_CurrentChoiceResponseIndex > _ChoiceResponses.Length - 1)
        {
            EndChoices();
            DisplayNextElement();
            return;
        }
        DisplaySentence(_ChoiceResponses[_CurrentChoiceResponseIndex]);
    }

    // Close overlay choice
    public void EndChoices()
    {
        _ChoicesHolder.gameObject.SetActive(false);
        _IsChoiceResponseDisplaying = false;
    }

    public void DisplayNextElement()
    {
        // If the choice is now displayed and the player doesn't select yet, do nothing
        if (_IsWaitingChoiceSelect)
        {
            Debug.Log("Select choice pls");
            return;
        }
        // If the reponse to the choice is still displaying, go on
        if (_IsChoiceResponseDisplaying)
        {
            Debug.Log("Display next response");
            DisplayNextChoiceResponse();
            return;
        }
        // If current dialogue is sentence and not complete yet, complete the sentence
        if (!CurrentDialogue.IsChoices &&
            _SentenceText.text != CurrentDialogue.SentenceData.SentenceContent)
        {
            _SentenceText.text = CurrentDialogue.SentenceData.SentenceContent;
            StopAllCoroutines();
            return;
        }
        // Move on next element
        _CurrentSentenceIndex++;
        if (_CurrentSentenceIndex >= _CurrentDialogueSO.Elements.Length)
        {
            Debug.Log("End of dialogue, should able to invoke some custom function");
            // Keep index within the array to prevent ArrayIndexOuTOfBound in unexpected situation
            _CurrentSentenceIndex = _CurrentDialogueSO.Elements.Length - 1;
            return;
        }
        // Display element
        // If it's a choice, spawn a choice overlay
        if (CurrentDialogue.IsChoices)
        {
            SpawnChoices(CurrentDialogue.Choices);
        }
        // If it's a sentence, display sentence
        else
        {
            DisplaySentence(CurrentDialogue.SentenceData);
        }
    }

    
}
