using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterGroupButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _ButtonText;

    public void SetChapterGroup(ContentChapterGroupSO newContentChapterGroup)
    {
        _ButtonText.text = newContentChapterGroup.Header;
        GetComponent<Button>().onClick.AddListener(() => ResearchLabManager.Instance.ClickChapterGroup(newContentChapterGroup));
    }
}
