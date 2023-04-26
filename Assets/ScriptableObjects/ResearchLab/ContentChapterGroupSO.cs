using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/ContentChapterGroup")]
public class ContentChapterGroupSO : ScriptableObject
{
    [SerializeField] private string _Header = "";
    public string Header => _Header;
    [SerializeField] private ContentChapterSO[] _ContentChapters;
    public ContentChapterSO[] ContentChapters => _ContentChapters;
}
