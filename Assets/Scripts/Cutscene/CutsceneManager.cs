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
    private DialogueElement _CurrentDialogueElement => PlayerManager.CurrentDialogueDatabase.Elements[_CurrentSentenceIndex];
    private DialogueSO _CurrentDialogueSO => PlayerManager.CurrentDialogueDatabase;
    private int _CurrentSentenceIndex;
    [SerializeField] private float _TypeDelaySeconds;
    // Variable related to choice' response
    private bool _IsChoiceResponseDisplaying;
    private bool _IsWaitingChoiceSelect;
    private DialogueElement.Sentence[] _ChoiceResponses;
    private int _CurrentChoiceResponseIndex;
    //the choices answer
    private int[] _ChoiceAnswers => PlayerManager.CurrentDialogueDatabase.ChoiceAnswers;
    private int _PassScore => PlayerManager.CurrentDialogueDatabase.PassScore;
    private int _CurrentAnswerIndex;
    //Collect the choices input
    private int _Score;
    private DialogueElement.Sentence _Temp;
    public GameObject Holder;
    private SceneMng SceneManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _CurrentSentenceIndex = 0;
        _IsChoiceResponseDisplaying = false;
        _IsWaitingChoiceSelect = false;
        DisplaySentence(_CurrentDialogueElement.SentenceData);
        _Score= 0;
        _CurrentAnswerIndex = 0;
        SceneManager = Holder.GetComponent<SceneMng>();
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

    // Set the response of the choice to display
    //Old ** public void SelectChoice(DialogueElement.Sentence[] newChoiceResponse)
    public void SelectChoice(DialogueElement.Choice ChoiceResponse)
    {
        if(_CurrentAnswerIndex < _ChoiceAnswers.Length && ChoiceResponse.number != 0){
            if(ChoiceResponse.number == _ChoiceAnswers[_CurrentAnswerIndex]){
                _Score++;
                _CurrentAnswerIndex++;
                Debug.Log("_Score: " + _Score.ToString());
            }
        }
        _ChoiceResponses = ChoiceResponse.ReponseData;
        _CurrentChoiceResponseIndex = -1;
        _IsChoiceResponseDisplaying = true;
        _IsWaitingChoiceSelect = false;
        _ChoicesHolder.gameObject.SetActive(false);
        DisplayNextChoiceResponse();
    }

    // Display the next sentence in choice' response
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

    // The function to show next dialogue element, triggered when click on dialougue box UI
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
        // If current dialogue element is sentence and not complete yet, complete the sentence
        if (!_CurrentDialogueElement.IsChoices && !_CurrentDialogueElement.IsChecker &&
            _SentenceText.text != _CurrentDialogueElement.SentenceData.SentenceContent)
        {
            _SentenceText.text = _CurrentDialogueElement.SentenceData.SentenceContent;
            StopAllCoroutines();
            return;
        }
        // Move on next element
        _CurrentSentenceIndex++;
        if (_CurrentSentenceIndex >= _CurrentDialogueSO.Elements.Length)
        {
            Debug.Log("End of dialogue, trigger OnDialogueEnd of DialogueSO.");
            // Keep index within the array to prevent ArrayIndexOuTOfBound in unexpected situation
            _CurrentSentenceIndex = _CurrentDialogueSO.Elements.Length - 1;
            // Change scene after end the dialogue
            SceneManager.ChangeScene(_CurrentDialogueSO.ChangeScene);
        }
        // Display element
        // If it's a choice, spawn a choice overlay
        if (_CurrentDialogueElement.IsChoices)
        {
            SpawnChoices(_CurrentDialogueElement.Choices);
        }
        // If it's a checker and not a choice, display sentence + _Score        
        else if (_CurrentDialogueElement.IsChecker)
        {
            if(_Score >= _PassScore){
                _Temp = _CurrentDialogueElement.CheckerAnswer.Pass;
                _Temp.SentenceContent = string.Copy(_CurrentDialogueElement.CheckerAnswer.Pass.SentenceContent.Replace(("[score]"),_Score.ToString()));
                DisplaySentence(_Temp);
                _Temp.SentenceContent = string.Copy(_CurrentDialogueElement.CheckerAnswer.Pass.SentenceContent.Replace(_Score.ToString(),("[score]")));
            } else {
                _Temp = _CurrentDialogueElement.CheckerAnswer.Fail;
                _Temp.SentenceContent = string.Copy(_CurrentDialogueElement.CheckerAnswer.Fail.SentenceContent.Replace(("[score]"),_Score.ToString()));
                DisplaySentence(_Temp);
                _Temp.SentenceContent = string.Copy(_CurrentDialogueElement.CheckerAnswer.Pass.SentenceContent.Replace(_Score.ToString(),("[score]")));
            }
        }
        // If it's a sentence, display sentence
        else
        {
            DisplaySentence(_CurrentDialogueElement.SentenceData);
        }
    }
}
