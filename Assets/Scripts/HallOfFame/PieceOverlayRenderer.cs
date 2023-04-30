using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PieceOverlayRenderer : MonoBehaviour
{
    public static PieceOverlayRenderer Instance;


    // One piece group detail overlay
    [Header("Piece Group Overlay")]
    [SerializeField] private GameObject _OnePieceGroupOverlay;
    [SerializeField] private Transform _PieceDatailHolder;
    [SerializeField] private GameObject _PieceDetailPrefab;
    [SerializeField] private TextMeshProUGUI _PieceGroupNameText;

    // One locked piece detail overlay
    [Header("Locked Piece Overlay")]
    [SerializeField] private GameObject _OnePieceLockedOverlay;
    [SerializeField] private Transform _UnlockRequirementHolder;
    [SerializeField] private GameObject _UnlockRequirementPrefab;
    [SerializeField] private TextMeshProUGUI _LockedPieceNameText;

    // One unlockable/unlocked piece detail overlay
    [Header("Unlocked Piece Overaly")]
    [SerializeField] private GameObject _OnePieceUnlockedOverlay;
    [SerializeField] private TextMeshProUGUI _UnlockedPieceNameText;
    [SerializeField] private TextMeshProUGUI _HowToObtainText;
    private JigsawPieceSO _CurrentJigsawPiece;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        _OnePieceGroupOverlay.SetActive(false);
        _OnePieceLockedOverlay.SetActive(false);
        _OnePieceUnlockedOverlay.SetActive(false);
    }

    private void OnEnable()
    {
        _OnePieceGroupOverlay.SetActive(false);
        _OnePieceLockedOverlay.SetActive(false);
        _OnePieceUnlockedOverlay.SetActive(false);
    }

    // Method to invoke when click on the piece in jigsaw tray. Show detail of clicked piece group
    public void OnClickJigsawPiece(int pieceIndex)
    {
        _OnePieceGroupOverlay.SetActive(true);
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

    // Method to invoke when click on HowToObtain button on piece group detail
    public void OnClickHowToObtain(JigsawPieceSO piece)
    {
        // If it's locked, show requirements
        if (piece.LockStatus == LockableStatus.Lock)
        {
            _OnePieceLockedOverlay.SetActive(true);
            _LockedPieceNameText.text = piece.GetLockableObjectName();
            // Refresh unlock requirement
            foreach (Transform child in _UnlockRequirementHolder)
            {
                Destroy(child.gameObject);
            }
            foreach (UnlockRequirementData unlockRequirementData in piece.GetUnlockRequirements(includeMoney:false))
            {
                GameObject newUnlockRequirement = Instantiate(_UnlockRequirementPrefab, _UnlockRequirementHolder);
                newUnlockRequirement.GetComponent<UnlockRequirementUI>().SetUnlockRequirement(unlockRequirementData, Color.black);
            }
        }
        // Else, show how to obtain
        else
        {
            _CurrentJigsawPiece = piece;
            _OnePieceUnlockedOverlay.SetActive(true);
            _UnlockedPieceNameText.text = piece.GetLockableObjectName();
            _HowToObtainText.text = PlayerManager.DescribePuzzleType(piece.HowToObtain);
        }
    }

    // Method to invoke when click comfirm on HowToObtain overlay
    public void OnClickConfirmObtain()
    {
        _CurrentJigsawPiece.GoToObtain();
    }
}
