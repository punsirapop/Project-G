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

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField] private ContentChapterSO[] Chapters;
    private int _CurrentChapter;
    private int _CurrentPage;

    private void Awake()
    {
        _CurrentChapter = 0;
        _CurrentPage = 0;
    }

    private void Update()
    {
        _ContentPageHeader.text = Chapters[_CurrentChapter].Contents[_CurrentPage].Header;
        _ContentPageImage.sprite = Chapters[_CurrentChapter].Contents[_CurrentPage].Image;
        _ContentPageDescription.text = Chapters[_CurrentChapter].Contents[_CurrentPage].Description;
    }

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
    }

    public void ChangeChapter(int newChapter)
    {
        _CurrentChapter = newChapter;
        _CurrentPage = 0;
    }
}
