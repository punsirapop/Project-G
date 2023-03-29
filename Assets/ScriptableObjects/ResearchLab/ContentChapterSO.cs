using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/ContentChapter")]
public class ContentChapterSO : ScriptableObject
{
    [SerializeField] private string _Header;
    public string Header => _Header;
    [SerializeField] private ContentPageSO[] _Contents;
    public ContentPageSO[] Contents => _Contents;
}
