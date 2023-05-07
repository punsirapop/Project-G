using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchLabManager : MonoBehaviour
{
    public static ResearchLabManager Instance;

    // Prefab and holder for chapter button
    [Header("Prefab and Holder")]
    [SerializeField] private GameObject _ChapterButtonPrefab;
    [SerializeField] private GameObject _ChapterButtonGroupPrefab;
    [SerializeField] private GameObject _ChapterButtonHolder;
    [SerializeField] private GameObject _OverlayForChapterGroup;
    // UI element for displaying page when it's not unlocked yet
    [Header("Lock and Unlockable Panel")]
    [SerializeField] private GameObject _LockedPage;
    [SerializeField] private Transform _UnlockRequirementsHolder;
    [SerializeField] private GameObject _UnlockRequirementPrefab;
    [SerializeField] private GameObject _UnlockablePage;
    [SerializeField] private Transform _SatisfiedRequirementHolder;
    // UI element for displaying one page of content
    [Header("Unlocked Panel")]
    [SerializeField] private GameObject _UnlockedPage;      // Parent of all elements in page
    [SerializeField] private TextMeshProUGUI _ContentPageHeader;
    [SerializeField] private Image _ContentPageImage;
    [SerializeField] private TextMeshProUGUI _ContentPageDescription;
    [SerializeField] private ProgressIndicator _ProgressIndicator;
    // Button object for colorize purpose
    [SerializeField] private GameObject _PreviousPageButton;
    [SerializeField] private GameObject _NextPageButton;
    [SerializeField] private GameObject[] _TabButtons;
    [SerializeField] private ChapterButton[] _ChapterButtons;

    // Hard-code 3 major category of content
    [Header("Contents")]
    [SerializeField] private ContentChapterGroupSO[] _BasicBioChapters;
    [SerializeField] private ContentChapterGroupSO[] _GeneticAlgoChapters;
    [SerializeField] private ContentChapterGroupSO[] _KnapsackChapters;
    private ContentChapterSO _CurrentChapterSO;
    private int _CurrentPage;
    private int _CurrentTab;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        SetCurrentTab(0);
        _RefreshPage();
    }

    // Refresh the content on the current page
    private void _RefreshPage()
    {
        _LockedPage.SetActive(_CurrentChapterSO.LockStatus == LockableStatus.Lock);
        _UnlockablePage.SetActive(_CurrentChapterSO.LockStatus == LockableStatus.Unlockable);
        _UnlockedPage.SetActive(_CurrentChapterSO.LockStatus == LockableStatus.Unlock);
        // if it's Unlocked, refresh content
        if (_CurrentChapterSO.LockStatus == LockableStatus.Unlock)
        {
            _ContentPageHeader.text = _CurrentChapterSO.Contents[_CurrentPage].Header;
            _ContentPageImage.sprite = _CurrentChapterSO.Contents[_CurrentPage].Image;
            _ContentPageDescription.text = _CurrentChapterSO.Contents[_CurrentPage].Description;
            _ProgressIndicator.SetIndicator(_CurrentPage, _CurrentChapterSO.Contents.Length - 1);
        }
        // If it's Unlockable, show unlock requirements and unlock button or something else
        else if (_CurrentChapterSO.LockStatus == LockableStatus.Unlockable)
        {
            // Refresh all unlock requirements
            foreach (Transform child in _SatisfiedRequirementHolder)
            {
                Destroy(child.gameObject);
            }
            foreach (UnlockRequirementData unlockRequirementData in _CurrentChapterSO.GetUnlockRequirements())
            {
                GameObject newUnlockRequirement = Instantiate(_UnlockRequirementPrefab, _SatisfiedRequirementHolder);
                newUnlockRequirement.GetComponent<UnlockRequirementUI>().SetUnlockRequirement(unlockRequirementData, Color.black);
            }
        }
        // If it's Locked (and by default), show unlock requirements
        else
        {
            // Refresh all unlock requirements
            foreach (Transform child in _UnlockRequirementsHolder)
            {
                Destroy(child.gameObject);
            }
            foreach (UnlockRequirementData unlockRequirementData in _CurrentChapterSO.GetUnlockRequirements())
            {
                GameObject newUnlockRequirement = Instantiate(_UnlockRequirementPrefab, _UnlockRequirementsHolder);
                newUnlockRequirement.GetComponent<UnlockRequirementUI>().SetUnlockRequirement(unlockRequirementData, Color.white);
            }
        }
    }

    // Add the page of current chapter
    public void AddPage(int amount)
    {
        _CurrentPage += amount;
        if (_CurrentPage < 0)
        {
            _CurrentPage = 0;
        }
        else if (_CurrentPage >= _CurrentChapterSO.Contents.Length)
        {
            _CurrentPage = _CurrentChapterSO.Contents.Length - 1;
        }
        _SetPageButtons();
        _RefreshPage();
    }

    // Set current chapter to display
    public void SetCurrentChapter(ContentChapterSO newContentChapter)
    {
        _CurrentChapterSO = newContentChapter;
        _CurrentPage = 0;
        _SetPageButtons();
        _RefreshPage();
    }

    // Set change page button to proper color
    private void _SetPageButtons()
    {
        int maxPage = _CurrentChapterSO.Contents.Length - 1;
        _PreviousPageButton.GetComponent<Image>().color = (_CurrentPage > 0) ? Color.green : Color.white;
        _PreviousPageButton.GetComponent<Button>().interactable = (_CurrentPage > 0) ? true : false;
        _NextPageButton.GetComponent<Image>().color = (_CurrentPage < maxPage) ? Color.green : Color.white;
        _NextPageButton.GetComponent<Button>().interactable = (_CurrentPage < maxPage) ? true : false;
    }

    // Set current tab
    public void SetCurrentTab(int newTab=-1)
    {
        _ChapterButtonHolder.SetActive(true);
        _OverlayForChapterGroup.SetActive(false);
        if (newTab >= 0)
        {
            _CurrentTab = newTab;
        }        
        _SetTabButtons();
        // Destory all previous child object
        foreach (Transform child in _ChapterButtonHolder.transform)
        {
            Destroy(child.gameObject);
        }
        // Change current ChapterButtons to the corresponding chapters in the tab
        ContentChapterGroupSO[] currentChapterGroup = _BasicBioChapters;
        if (_CurrentTab == 1)
        {
            currentChapterGroup = _GeneticAlgoChapters;
        }
        else if (_CurrentTab == 2)
        {
            currentChapterGroup = _KnapsackChapters;
        }
        // Spawn all chapter button
        bool firstButtonInvoked = false;
        foreach (ContentChapterGroupSO contentChapterGroup in currentChapterGroup)
        {
            // If there is only one chapter in the group, spawn the content chapter button
            if (contentChapterGroup.ContentChapters.Length == 1)
            {
                GameObject newChapterButton = Instantiate(_ChapterButtonPrefab, _ChapterButtonHolder.transform);
                newChapterButton.GetComponent<ChapterButton>().SetChapter(contentChapterGroup.ContentChapters[0]);
                // Trigger the onClick of first new created button
                // Don't use GetComponentInChildren<ChapterButton>() because the destroyed objects are not immediately destroyed
                // They just be marked to be destroyed and Unity actually destroy them after this SetCurrentTab() end
                // So if we use said method it will trigger the first button in the previous tab instead
                if (!firstButtonInvoked)
                {
                    newChapterButton.GetComponent<ChapterButton>().GetComponent<Button>().onClick.Invoke();
                    firstButtonInvoked = true;
                }
            }
            // If there is more than one chapter in the group, spawn the content chapter group button instead
            else
            {
                GameObject newChapterGroupButton = Instantiate(_ChapterButtonGroupPrefab, _ChapterButtonHolder.transform);
                newChapterGroupButton.GetComponent<ChapterGroupButton>().SetChapterGroup(contentChapterGroup);
            }
        }
    }

    public void ClickChapterGroup(ContentChapterGroupSO clickedGroup)
    {
        _ChapterButtonHolder.SetActive(false);
        _OverlayForChapterGroup.SetActive(true);
        // Destory all previous child object
        foreach (Transform child in _OverlayForChapterGroup.GetComponentsInChildren<Transform>()[1].transform)
        {
            Destroy(child.gameObject);
        }
        // Spawn all chapter button
        bool firstButtonInvoked = false;
        foreach (ContentChapterSO contentChapter in clickedGroup.ContentChapters)
        {
            GameObject newChapterButton = Instantiate(_ChapterButtonPrefab, _OverlayForChapterGroup.GetComponentsInChildren<Transform>()[1].transform);
            newChapterButton.GetComponent<ChapterButton>().SetChapter(contentChapter);
            if (!firstButtonInvoked)
            {
                newChapterButton.GetComponent<ChapterButton>().GetComponent<Button>().onClick.Invoke();
                firstButtonInvoked = true;
            }
        }
    }

    // Set tab button to proper color
    private void _SetTabButtons()
    {
        for (int i = 0; i < _TabButtons.Length; i++)
        {
            if (i == _CurrentTab)
            {
                _TabButtons[i].GetComponent<Button>().interactable = false;
            }
            else
            {
                _TabButtons[i].GetComponent<Button>().interactable = true;
            }
        }
    }

    // Deselect all chapter buttons
    public void DeselectChapterButtons()
    {
        _ChapterButtons = GetComponentsInChildren<ChapterButton>();
        foreach (ChapterButton chapterButton in _ChapterButtons)
        {
            chapterButton.SetIsSelected(false);
        }
    }

    public void UnlockCurrentChapter()
    {
        _CurrentChapterSO.Unlock();
        PlayerManager.ValidateUnlocking();
        if (_CurrentChapterSO.TeachingDialogue != null)
        {
            PlayerManager.SetCurrentDialogue(_CurrentChapterSO.TeachingDialogue);
            SceneMng.StaticChangeScene("Cutscene");
        }
        else
        {
            _RefreshPage();
        }
    }
}
