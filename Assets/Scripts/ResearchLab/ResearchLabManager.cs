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
    [SerializeField] private ContentChapterSO[] _Chapters;
    private ContentChapterSO _CurrentChapterSO;
    private int _CurrentPage;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Destory all previous child object
        foreach (Transform child in _ChapterButtonHolder)
        {
            Destroy(child.gameObject);
        }
        // Spawn all chapter button
        foreach (ContentChapterSO contentChapter in _Chapters)
        {
            GameObject newChapterButton = Instantiate(_ChapterButtonPrefab, _ChapterButtonHolder);
            newChapterButton.GetComponent<ChapterButton>().SetChapter(contentChapter);
        }
        DeselectChapterButtons();
        GetComponentInChildren<ChapterButton>().GetComponent<Button>().onClick.Invoke();
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
        Debug.Log("Set current chapter to " + newContentChapter.Header);
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
