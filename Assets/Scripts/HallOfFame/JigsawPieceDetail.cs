using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JigsawPieceDetail : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _SuccessCountText;
    [SerializeField] private TextMeshProUGUI _FailCountText;
    [SerializeField] private TextMeshProUGUI _PieceNameText;
    [SerializeField] private GameObject _HowToObtainButton;

    public void SetJigsawPiece(JigsawPieceSO piece)
    {
        _SuccessCountText.text = piece.SuccessCount.ToString();
        _FailCountText.text = piece.FailCount.ToString();
        _PieceNameText.text = piece.GetLockableObjectName();
        _HowToObtainButton.GetComponent<Button>().onClick.AddListener(() => PieceOverlayRenderer.Instance.OnClickHowToObtain(piece));
        bool isLock = (piece.LockStatus == LockableStatus.Lock);
        _HowToObtainButton.GetComponent<Image>().color = isLock ? Color.white : Color.green;
        _HowToObtainButton.GetComponentInChildren<TextMeshProUGUI>().color = isLock ? Color.black : Color.white;
    }
}
