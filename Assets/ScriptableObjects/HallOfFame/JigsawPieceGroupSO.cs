using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/JigsawPieceGroup")]
public class JigsawPieceGroupSO : ScriptableObject
{
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] private JigsawPieceSO[] _JigsawPieces;
    public JigsawPieceSO[] JigsawPieces => _JigsawPieces;
    
    // Return the highest unlocked level of jigsaw piece
    public JigsawLevel GetHighestUnlockedLevel()
    {
        JigsawLevel highestLevel = JigsawLevel.None;
        if (_JigsawPieces == null)
        {
            return highestLevel;
        }
        foreach (JigsawPieceSO piece in _JigsawPieces)
        {
            if ((piece.LockStatus == LockableStatus.Unlock) &&
                (piece.Level > highestLevel))
            {
                highestLevel = piece.Level;
            }
        }
        return highestLevel;
    }
}
