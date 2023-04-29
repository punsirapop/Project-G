using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JigsawPieceDetail : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _SuccessCountText;
    [SerializeField] private TextMeshProUGUI _FailCountText;
    [SerializeField] private TextMeshProUGUI _PieceNameText;

    public void SetJigsawPiece(JigsawPieceSO piece)
    {
        _SuccessCountText.text = piece.SuccessCount.ToString();
        _FailCountText.text = piece.FailCount.ToString();
        _PieceNameText.text = piece.GetLockableObjectName();
    }
}
