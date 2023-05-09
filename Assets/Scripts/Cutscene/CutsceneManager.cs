using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;

    // UI element for display data
    [Header("Basic Element")]
    [SerializeField] private TextMeshProUGUI _SpeakerName;
    [SerializeField] private TextMeshProUGUI _SentenceText;
    [SerializeField] private HorizontalLayoutGroup _CharacterLayout;
    [SerializeField] private Sprite[] _NPCFaceSprites;
    [SerializeField] private Image _CharacterImage;
    [SerializeField] private Image _IllustrationImage;
    [SerializeField] private Transform _ChoicesHolder;
    [SerializeField] private GameObject _ChoiceButtonPrefab;
    [SerializeField] private GameObject _OverlayPrefab;
    [Header("Skip Auto and Log")]
    [SerializeField] private GameObject _SkipConfirmOverlay;
    [SerializeField] private GameObject _SkipCancelButton;
    [SerializeField] private TextMeshProUGUI _SkipConfirmNotifyText;
    [SerializeField] private Toggle _AutoToggle;
    [SerializeField] private int _AutoWaitBetweenSentence;
    [SerializeField] private Toggle _SpeedToggle;
    [SerializeField] private GameObject _LogPanel;
    [SerializeField] private Transform _LogHolder;
    [SerializeField] private Scrollbar _LogScrollbar;
    [SerializeField] private GameObject _LogDisplayPrefab;
    private List<Log> _DialogueLogs;
    private int _WaitBetweenSentence => _SpeedToggle.isOn ? (int)(_AutoWaitBetweenSentence / 2) : _AutoWaitBetweenSentence;
    // Variable related to dialogue data
    private DialogueElement _CurrentDialogueElement => PlayerManager.CurrentDialogueDatabase.Elements[_CurrentSentenceIndex];
    private DialogueSO _CurrentDialogueSO => PlayerManager.CurrentDialogueDatabase;
    private int _CurrentSentenceIndex;
    [SerializeField] private float _DefaultTypeDelaySeconds;
    private float _TypeDelaySeconds => _SpeedToggle.isOn ? (_DefaultTypeDelaySeconds / 2) : _DefaultTypeDelaySeconds;
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
    public GameObject Holder;
    private SoundEffectHolder _SoundEffctManager;
    private int _CharacterIndex;

    public struct Log
    {
        public string SpeakerName { get; private set; }
        public string SentenceContent { get; private set; }
        public bool IsSelected { get; private set; }

        // Spawn normal sentence
        public Log(DialogueElement.Sentence sentence)
        {
            if (sentence.Speaker == DialogueElement.Speaker.Player)
            {
                SpeakerName = "Player";
            }
            else
            {
                // NPC name
                SpeakerName = CutsceneManager.Instance._CurrentDialogueSO.SpeakerName;
            }
            SentenceContent = sentence.SentenceContent;
            IsSelected = false;     // This's not a choice
        }

        // Spawn choice
        public Log(DialogueElement.Choice choice, bool isSelected)
        {
            SpeakerName = "";
            SentenceContent = "[" + choice.SentenceContent + "]";
            IsSelected = isSelected;
        }
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _SkipConfirmOverlay.SetActive(false);
        _AutoToggle.isOn = false;
        _SpeedToggle.isOn = false;
        _SpeedToggle.gameObject.SetActive(false);
        _LogPanel.SetActive(false);
        _CurrentSentenceIndex = 0;
        _IsChoiceResponseDisplaying = false;
        _IsWaitingChoiceSelect = false;
        // Initiate log
        _DialogueLogs = new List<Log>();
        // Start displaying
        DisplaySentence(_CurrentDialogueElement.SentenceData);
        _Score= 0;
        _CurrentAnswerIndex = 0;
        _SoundEffctManager = Holder.GetComponent<SoundEffectHolder>();
    }

    public void DisplaySentence(DialogueElement.Sentence currentSentence)
    {
        // Add sentence to the log
        _DialogueLogs.Add(new Log(currentSentence));
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
            case DialogueElement.Speaker.NPCHandDown_EyeClose_MouthClose:
                _CharacterImage.sprite = _NPCFaceSprites[0];
                break;
            case DialogueElement.Speaker.NPCHandDown_EyeClose_MouthOpen:
                _CharacterImage.sprite = _NPCFaceSprites[1];
                break;
            case DialogueElement.Speaker.NPCHandDown_EyeOpen_MouthClose:
                _CharacterImage.sprite = _NPCFaceSprites[2];
                break;
            case DialogueElement.Speaker.NPCHandDown_EyeOpen_MouthOpen:
                _CharacterImage.sprite = _NPCFaceSprites[3];
                break;
            case DialogueElement.Speaker.NPCHandDown_Happy:
                _CharacterImage.sprite = _NPCFaceSprites[4];
                break;
            case DialogueElement.Speaker.NPCHandDown_Sad_MouthClose:
                _CharacterImage.sprite = _NPCFaceSprites[5];
                break;
            case DialogueElement.Speaker.NPCHandDown_Sad_MouthOpen:
                _CharacterImage.sprite = _NPCFaceSprites[6];
                break;
            case DialogueElement.Speaker.NPC1Hand_EyeClose_MouthClose:
                _CharacterImage.sprite = _NPCFaceSprites[7];
                break;
            case DialogueElement.Speaker.NPC1Hand_EyeClose_MouthOpen:
                _CharacterImage.sprite = _NPCFaceSprites[8];
                break;
            case DialogueElement.Speaker.NPC1Hand_EyeOpen_MouthClose:
                _CharacterImage.sprite = _NPCFaceSprites[9];
                break;
            case DialogueElement.Speaker.NPC1Hand_EyeOpen_MouthOpen:
                _CharacterImage.sprite = _NPCFaceSprites[10];
                break;
            case DialogueElement.Speaker.NPC1Hand_Happy:
                _CharacterImage.sprite = _NPCFaceSprites[11];
                break;
            case DialogueElement.Speaker.NPC1Hand_Sad_MouthClose:
                _CharacterImage.sprite = _NPCFaceSprites[12];
                break;
            case DialogueElement.Speaker.NPC1Hand_Sad_MouthOpen:
                _CharacterImage.sprite = _NPCFaceSprites[13];
                break;
            case DialogueElement.Speaker.NPC2Hand_EyeClose_MouthClose:
                _CharacterImage.sprite = _NPCFaceSprites[14];
                break;
            case DialogueElement.Speaker.NPC2Hand_EyeClose_MouthOpen:
                _CharacterImage.sprite = _NPCFaceSprites[15];
                break;
            case DialogueElement.Speaker.NPC2Hand_EyeOpen_MouthClose:
                _CharacterImage.sprite = _NPCFaceSprites[16];
                break;
            case DialogueElement.Speaker.NPC2Hand_EyeOpen_MouthOpen:
                _CharacterImage.sprite = _NPCFaceSprites[17];
                break;
            case DialogueElement.Speaker.NPC2Hand_Happy:
                _CharacterImage.sprite = _NPCFaceSprites[18];
                break;
            case DialogueElement.Speaker.NPC2Hand_Sad_MouthClose:
                _CharacterImage.sprite = _NPCFaceSprites[19];
                break;
            case DialogueElement.Speaker.NPC2Hand_Sad_MouthOpen:
                _CharacterImage.sprite = _NPCFaceSprites[20];
                break;
        }
        _SpeakerName.text = speaker.ToString();
        if(_SpeakerName.text != "Player"){
            _SpeakerName.text = _CurrentDialogueSO.SpeakerName;
        }
        // Start sentence text displaying
        StartCoroutine(TypeSentence(currentSentence));
    }

    // Function for gradually display the sentence text
    IEnumerator TypeSentence(DialogueElement.Sentence currentSentence)
    {
        bool _InTag = false;
        bool _InBoldTag = false;
        _SentenceText.text = "";
        _CharacterIndex = 0;
        // Type each character with some delay
        foreach (char c in currentSentence.SentenceContent.ToCharArray())
        {
            _CharacterIndex++;
            if (c == '<')
            {
                if(!_InBoldTag)
                {
                    _InBoldTag = true;
                }
                else
                {
                    _InBoldTag = false;
                }
                _InTag = true;
            }
            else if (c == '>')
            {
                _InTag = false;
            }
            
            if (!_InTag && c != '>')
            {
                _SentenceText.text += c;
                 if (_InBoldTag)
                {
                    int _lastCharacterIndex = _SentenceText.text.Length - 1;
                    _SentenceText.text = _SentenceText.text.Substring(0, _lastCharacterIndex) + "<b>" + _SentenceText.text[_lastCharacterIndex] + "</b>";
                }
            }
            yield return new WaitForSeconds(_TypeDelaySeconds);
        }
        if (_AutoToggle.isOn)
        {
            Debug.Log("Auto continue");
            yield return new WaitForSeconds(_WaitBetweenSentence);
            DisplayNextElement();
        }
    }

    IEnumerator DisplayNextElementDelayed(int secondDelay)
    {
        yield return new WaitForSeconds(secondDelay);
        DisplayNextElement();
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
    public void SelectChoice(DialogueElement.Choice ChoiceResponse)
    {
        // Record in the log
        foreach (DialogueElement.Choice choice in _CurrentDialogueElement.Choices)
        {
            bool isSelected = (choice == ChoiceResponse);
            _DialogueLogs.Add(new Log(choice, isSelected));
        }
        // Calculate score
        if(_CurrentAnswerIndex < _ChoiceAnswers.Length && ChoiceResponse.number != 0){
            if (ChoiceResponse.number == _ChoiceAnswers[_CurrentAnswerIndex]){
                _Score++;
            }
            Debug.Log("Correct " + _ChoiceAnswers[_CurrentAnswerIndex].ToString() + " | Select " + ChoiceResponse.number);
            Debug.Log("_Score: " + _Score.ToString());
            _CurrentAnswerIndex++;
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
        
        if (_CurrentChoiceResponseIndex != -1)
        {
            // If sthe current sentence isn't complete, complete it
            if (_CharacterIndex != _ChoiceResponses[_CurrentChoiceResponseIndex].SentenceContent.Length)
            {
                _CharacterIndex = _ChoiceResponses[_CurrentChoiceResponseIndex].SentenceContent.Length;
                _SentenceText.text = _ChoiceResponses[_CurrentChoiceResponseIndex].SentenceContent;
                StopAllCoroutines();
                return;
            }
        }
        // Show the next response
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
            (_CharacterIndex != _CurrentDialogueElement.SentenceData.SentenceContent.Length))
        {
            _CharacterIndex = _CurrentDialogueElement.SentenceData.SentenceContent.Length;
            _SentenceText.text = _CurrentDialogueElement.SentenceData.SentenceContent;
            StopAllCoroutines();
            if (_AutoToggle.isOn)
            {
                StartCoroutine(DisplayNextElementDelayed(_AutoWaitBetweenSentence));
            }
            return;
        }
        // Move on next element
        _CurrentSentenceIndex++;
        if (_CurrentSentenceIndex >= _CurrentDialogueSO.Elements.Length)
        {
            _OnDialogueEnd();
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
            DialogueElement.Sentence sentenceToDisplay;
            if (_Score >= _PassScore){
                sentenceToDisplay = _CurrentDialogueElement.CheckerAnswer.Pass.Copy();
                _SoundEffctManager.PlaySoundEffect("Pass");
            } else {
                sentenceToDisplay = _CurrentDialogueElement.CheckerAnswer.Fail.Copy();
                _SoundEffctManager.PlaySoundEffect("Fail");
            }
            sentenceToDisplay.SentenceContent = sentenceToDisplay.SentenceContent.Replace("[score]", _Score.ToString());
            DisplaySentence(sentenceToDisplay);
        }
        // If it's a sentence, display sentence
        else
        {
            // Display sentence
            DisplaySentence(_CurrentDialogueElement.SentenceData);
        }
    }

    // Change to previous scene and show notificaion if it's a test
    private void _OnDialogueEnd()
    {
        // Stop auto
        _AutoToggle.isOn = false;
        OnToggleAuto();
        Debug.Log("End of dialogue, trigger OnDialogueEnd of DialogueSO.");
        // Keep index within the array to prevent ArrayIndexOuTOfBound in unexpected situation
        _CurrentSentenceIndex = _CurrentDialogueSO.Elements.Length - 1;
        // Change scene after end the dialogue
        //SceneManager.ChangeScene(_CurrentDialogueSO.ChangeScene);

        // Result conclusion for testing (gathering JigsawPieceSO)
        if (_CurrentDialogueSO.ChoiceAnswers.Length > 0)
        {
            // Score feedback
            string feedbackText = "- Your score is " + _Score.ToString() + "/" + _ChoiceAnswers.Length.ToString();
            if (_Score < _PassScore)
            {
                feedbackText += " (Required at least " + _PassScore.ToString() + " score to pass)";
            }
            // Jigsaw feedback
            foreach (string jigsawFeedback in PlayerManager.RecordPuzzleResult(_Score >= _PassScore))
            {
                feedbackText += "\n" + jigsawFeedback;
            }
            GameObject overlay = Instantiate(_OverlayPrefab, this.transform);
            overlay.GetComponent<PuzzleFeedbackOverlay>().SetFeedBack(
                isPass: _Score >= _PassScore,
                headerText: _Score >= _PassScore ? "Success" : "Fail",
                feedbackText: feedbackText
                );
            return;
        }
        SceneMng.ReturnToPreviousScene();
    }

    // Trigger when player tap the dialogue box
    public void OnClickDialogueBox()
    {
        // If it's in auto, stop auto
        if (_AutoToggle.isOn)
        {
            _AutoToggle.isOn = false;
            OnToggleAuto();
        }
        else
        {
            DisplayNextElement();
        }
    }

    // Immediately end the skippable dialogue
    public void OnSkipConfirmClick()
    {
        if (_CurrentDialogueSO.IsSkippable)
        {
            _OnDialogueEnd();
        }
    }

    // Open skip confirmation overlay
    public void OnSkipButtonClick()
    {
        if (_AutoToggle.isOn)
        {
            _AutoToggle.isOn = false;
            OnToggleAuto();
        }
        _SkipConfirmOverlay.SetActive(true);
        _SkipConfirmNotifyText.text = _CurrentDialogueSO.IsSkippable ? "Do you sure you want to skip?" : "This dialogue can not be skipped.";
        _SkipConfirmNotifyText.text += "\n\n" + _CurrentDialogueSO.SkipNotifyText;
        _SkipCancelButton.SetActive(_CurrentDialogueSO.IsSkippable);
    }

    // Set the appearance of auto and speed toggle
    public void OnToggleAuto()
    {
        // Toggle auto on, immidiately display next element and show speed toggle
        if (_AutoToggle.isOn)
        {
            _AutoToggle.GetComponent<Animator>().enabled = true;
            _SpeedToggle.gameObject.SetActive(true);
            _SpeedToggle.isOn = true;
            OnToggleSpeed();
            DisplayNextElement();
        }
        // Toggle auto off, do nothing
        else
        {
            _AutoToggle.GetComponent<Animator>().enabled = false;
            _AutoToggle.GetComponentsInChildren<TextMeshProUGUI>(true)[1].gameObject.SetActive(false);
            _AutoToggle.GetComponentsInChildren<TextMeshProUGUI>(true)[2].gameObject.SetActive(false);
            _SpeedToggle.gameObject.SetActive(false);
            _SpeedToggle.isOn = false;
            OnToggleSpeed();
        }
    }

    // Change speed toggle appearance
    public void OnToggleSpeed()
    {
        if (_SpeedToggle.isOn)
        {
            _SpeedToggle.GetComponentInChildren<TextMeshProUGUI>().text = "2X";
        }
        else
        {
            _SpeedToggle.GetComponentInChildren<TextMeshProUGUI>().text = "1X";
        }
    }

    // Open log panel and show all log
    public void OnOpenLog()
    {
        // Stop the auto and enable log panel
        _AutoToggle.isOn = false;
        OnToggleAuto();
        _LogPanel.SetActive(true);
        // Refresh logs
        foreach (Transform child in _LogHolder)
        {
            Destroy(child.gameObject);
        }
        foreach (Log log in _DialogueLogs)
        {
            GameObject newLogDisplay = Instantiate(_LogDisplayPrefab, _LogHolder);
            newLogDisplay.GetComponent<LogDisplayElement>().SetLog(log);
        }
        // Set scrollbar to the bottom
        _LogScrollbar.value = 0;
    }
}
