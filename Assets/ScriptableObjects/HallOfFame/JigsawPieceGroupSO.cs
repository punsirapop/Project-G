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
    public JigsawLevel GetHighestObtainedLevel()
    {
        JigsawLevel highestLevel = JigsawLevel.None;
        if (_JigsawPieces == null)
        {
            return highestLevel;
        }
        foreach (JigsawPieceSO piece in _JigsawPieces)
        {
            if ((piece.LockStatus == LockableStatus.Unlock) &&
                (piece.Level > highestLevel) &&
                (piece.SuccessCount > 0))
            {
                highestLevel = piece.Level;
            }
        }
        return highestLevel;
    }
}
