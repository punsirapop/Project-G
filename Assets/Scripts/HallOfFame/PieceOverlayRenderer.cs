using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PieceOverlayRenderer : MonoBehaviour
{
    // One piece detail overlay
    [SerializeField] private GameObject _OnePieceOverlay;
    [SerializeField] private TextMeshProUGUI _PieceNameText;

    private void Start()
    {
        _OnePieceOverlay.SetActive(false);
    }

    public void OnClickJigsawPiece(int pieceIndex)
    {
        _OnePieceOverlay.SetActive(true);
        _PieceNameText.text = HallOfFameManager.Instance.CurrentJigsawTray.AllJigsawPieceGroup[pieceIndex].Name;
    }
}
