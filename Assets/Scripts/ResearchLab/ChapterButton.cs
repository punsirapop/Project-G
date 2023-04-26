using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _ButtonText;
    private bool _IsSelected = false;

    public void SetChapter(ContentChapterSO newContentChapter)
    {
        _ButtonText.text = newContentChapter.Header;
        GetComponent<Button>().onClick.AddListener(() => ResearchLabManager.Instance.SetCurrentChapter(newContentChapter));
        GetComponent<Button>().onClick.AddListener(() => ResearchLabManager.Instance.DeselectChapterButtons());
        GetComponent<Button>().onClick.AddListener(() => SetIsSelected(true));
    }

    public void SetIsSelected(bool newIsSelected)
    {
        _IsSelected = newIsSelected;
        GetComponent<Button>().interactable = !newIsSelected;
    }
}
