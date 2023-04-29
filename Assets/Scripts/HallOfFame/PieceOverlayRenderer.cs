using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PieceOverlayRenderer : MonoBehaviour
{
    // One piece detail overlay
    [SerializeField] private GameObject _OnePieceOverlay;
    [SerializeField] private Transform _PieceDatailHolder;
    [SerializeField] private GameObject _PieceDetailPrefab;
    [SerializeField] private TextMeshProUGUI _PieceGroupNameText;

    private void Start()
    {
        _OnePieceOverlay.SetActive(false);
    }

    public void OnClickJigsawPiece(int pieceIndex)
    {
        _OnePieceOverlay.SetActive(true);
        _PieceGroupNameText.text = HallOfFameManager.Instance.CurrentJigsawTray.AllJigsawPieceGroup[pieceIndex].Name;
        // Refresh all piece detail
        foreach (Transform child in _PieceDatailHolder)
        {
            Destroy(child.gameObject);
        }
        foreach (JigsawPieceSO piece in HallOfFameManager.Instance.CurrentJigsawTray.AllJigsawPieceGroup[pieceIndex].JigsawPieces)
        {
            GameObject newPieceDetail = Instantiate(_PieceDetailPrefab, _PieceDatailHolder);
            newPieceDetail.GetComponent<JigsawPieceDetail>().SetJigsawPiece(piece);
        }
    }
}
