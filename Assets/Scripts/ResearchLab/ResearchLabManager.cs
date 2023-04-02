using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchLabManager : MonoBehaviour
{
    public static ResearchLabManager Instance;

    [SerializeField] private GameObject _ChapterButtonPrefab;
    [SerializeField] private Transform _ChapterButtonHolder;
    // UI element for displaying one page of content
    [SerializeField] private TextMeshProUGUI _ContentPageHeader;
    [SerializeField] private Image _ContentPageImage;
    [SerializeField] private TextMeshProUGUI _ContentPageDescription;
    // Button object for colorize purpose
    [SerializeField] private GameObject _PreviousPageButton;
    [SerializeField] private GameObject _NextPageButton;
    [SerializeField] private GameObject[] _TabButtons;
    [SerializeField] private ChapterButton[] _ChapterButtons;

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField] private ContentChapterSO[] _BasicBioChapters;
    [SerializeField] private ContentChapterSO[] _GeneticAlgoChapters;
    [SerializeField] private ContentChapterSO[] _KnapsackChapters;
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
        _ContentPageHeader.text = _CurrentChapterSO.Contents[_CurrentPage].Header;
        _ContentPageImage.sprite = _CurrentChapterSO.Contents[_CurrentPage].Image;
        _ContentPageDescription.text = _CurrentChapterSO.Contents[_CurrentPage].Description;
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
    public void SetCurrentTab(int newTab)
    {
        _CurrentTab = newTab;
        _SetTabButtons();
        // Destory all previous child object
        foreach (Transform child in _ChapterButtonHolder)
        {
            Destroy(child.gameObject);
        }
        // Change current ChapterButtons to the corresponding chapters in the tab
        ContentChapterSO[] currentChapters = _BasicBioChapters;
        if (_CurrentTab == 1)
        {
            currentChapters = _GeneticAlgoChapters;
        }
        else if (_CurrentTab == 2)
        {
            currentChapters = _KnapsackChapters;
        }
        // Spawn all chapter button
        bool firstButtonInvoked = false;
        foreach (ContentChapterSO contentChapter in currentChapters)
        {
            GameObject newChapterButton = Instantiate(_ChapterButtonPrefab, _ChapterButtonHolder);
            newChapterButton.GetComponent<ChapterButton>().SetChapter(contentChapter);
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
        _ChapterButtons = _ChapterButtonHolder.GetComponentsInChildren<ChapterButton>();
        foreach (ChapterButton chapterButton in _ChapterButtons)
        {
            chapterButton.SetIsSelected(false);
        }
    }
}
