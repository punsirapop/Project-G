using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/JigsawTray")]
public class JigsawTraySO : ScriptableObject
{
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] Sprite _FrameSprite;
    public Sprite FrameSprite => _FrameSprite;
    [SerializeField] Sprite _OverlaySprite;
    public Sprite OverlaySprite => _OverlaySprite;
    [SerializeField] private JigsawPieceGroupSO[] _AllJigsawPieceGroup;
    public JigsawPieceGroupSO[] AllJigsawPieceGroup => _AllJigsawPieceGroup;
}
