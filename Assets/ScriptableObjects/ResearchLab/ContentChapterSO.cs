using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/ContentChapter")]
public class ContentChapterSO : LockableObject
{
    // Header and contents
    [SerializeField] private string _Header;
    public string Header => _Header;
    [SerializeField] private ContentPageSO[] _Contents;
    public ContentPageSO[] Contents => _Contents;

    private void OnEnable()
    {
        SaveManager.OnReset += base.Reset;
    }

    private void OnDestroy()
    {
        SaveManager.OnReset -= base.Reset;
    }

    public override string GetRequirementPrefix()
    {
        return "Research";
    }
    public override string GetLockableObjectName()
    {
        return _Header;
    }
}
