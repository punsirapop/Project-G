using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/ContentPage")]
public class ContentPageSO : ScriptableObject
{
    [SerializeField] private string _Header;
    public string Header => _Header;
    [SerializeField] private Sprite _Image;
    public Sprite Image => _Image;
    [SerializeField] [TextArea(10, 100)] private string _Description;
    public string Description => _Description;
}
