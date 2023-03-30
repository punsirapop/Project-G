using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchLabManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _ContentPageHeader;
    [SerializeField] private Image _ContentPageImage;
    [SerializeField] private TextMeshProUGUI _ContentPageDescription;
    // Button object for colorize purpose
    [SerializeField] private GameObject _PreviousPageButton;
    [SerializeField] private GameObject _NextPageButton;
    [SerializeField] private GameObject[] _TabButtons;
    [SerializeField] private GameObject[] _ChapterButtons;

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField] private ContentChapterSO[] Chapters;
    private int _CurrentChapter;
    private int _CurrentPage;

    private void Start()
    {
        ChangeChapter(0);
    }

    private void Update()
    {
        // Continually display the content of the current page
        _ContentPageHeader.text = Chapters[_CurrentChapter].Contents[_CurrentPage].Header;
        _ContentPageImage.sprite = Chapters[_CurrentChapter].Contents[_CurrentPage].Image;
        _ContentPageDescription.text = Chapters[_CurrentChapter].Contents[_CurrentPage].Description;
    }

    // Add the page of current chapter
    public void AddPage(int amount)
    {
        _CurrentPage += amount;
        if (_CurrentPage < 0)
        {
            _CurrentPage = 0;
        }
        else if (_CurrentPage >= Chapters[_CurrentChapter].Contents.Length)
        {
            _CurrentPage = Chapters[_CurrentChapter].Contents.Length - 1;
        }
        _SetPageButtons();
    }

    // Change the content chapter
    public void ChangeChapter(int newChapter)
    {
        Debug.Log("Change to chapter " + newChapter.ToString());
        _CurrentChapter = newChapter;
        _CurrentPage = 0;
        _SetChapterButtons();
        _SetPageButtons();
    }

    // Set change page button to proper color
    private void _SetPageButtons()
    {
        int maxPage = Chapters[_CurrentChapter].Contents.Length - 1;
        _PreviousPageButton.GetComponent<Image>().color = (_CurrentPage > 0) ? Color.green : Color.white;
        _PreviousPageButton.GetComponent<Button>().interactable = (_CurrentPage > 0) ? true : false;
        _NextPageButton.GetComponent<Image>().color = (_CurrentPage < maxPage) ? Color.green : Color.white;
        _NextPageButton.GetComponent<Button>().interactable = (_CurrentPage < maxPage) ? true : false;
    }
    // Set chapter button to proper color
    private void _SetChapterButtons()
    {
        Debug.Log("ChapterButtons length = " + _ChapterButtons.Length.ToString());
        Debug.Log("Current chapter = " + _CurrentChapter.ToString());
        for (int i = 0; i < _ChapterButtons.Length; i++)
        {
            _ChapterButtons[i].GetComponent<Button>().interactable = (i == _CurrentChapter) ? false : true;
            if (i == _CurrentChapter)
            {
                Debug.Log("Button of Chapter " + i.ToString() + " not interactable");
                Debug.Log(_ChapterButtons[i].GetComponent<Button>().interactable);
            }
        }
        Debug.Log("Set ChapterButtons");
    }
}
