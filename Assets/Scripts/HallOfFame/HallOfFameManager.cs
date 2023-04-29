using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HallOfFameManager : MonoBehaviour
{
    public static HallOfFameManager Instance;

    // Jigsaw level color
    [Header("Jigsaw Level Color")]
    [SerializeField] private Color _EmptyPieceColor;
    public Color EmptyPieceColor => _EmptyPieceColor;
    [SerializeField] private Color _CopperPieceColor;
    public Color CopperPieceColor => _CopperPieceColor;
    [SerializeField] private Color _SilverPieceColor;
    public Color SilverPieceColor => _SilverPieceColor;
    [SerializeField] private Color _GoldPieceColor;
    public Color GoldPieceColor => _GoldPieceColor;

    // JigsawTray
    [SerializeField] private GameObject _AllTrayHolder;
    [SerializeField] private GameObject _OneTrayHolder;
    [SerializeField] private JigsawTraySO[] _AllJigsawTray;
    private int _CurrentJigsawTrayIndex;
    public JigsawTraySO CurrentJigsawTray => _AllJigsawTray[_CurrentJigsawTrayIndex];


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        //_AllTrayHolder.SetActive(true);
        //_OneTrayHolder.SetActive(false);
        _AllTrayHolder.SetActive(false);
        _OneTrayHolder.SetActive(true);
    }

    // Enter one tray mode
    public void EnterJigsawTrayByIndex(int index)
    {
        if ((index < 0) || (index > _AllJigsawTray.Length - 1))
        {
            return;
        }
        _AllTrayHolder.SetActive(false);
        _OneTrayHolder.SetActive(true);
        _CurrentJigsawTrayIndex = index;
        GetComponentInChildren<JigsawTrayRenderer>().SetJigsawTray(CurrentJigsawTray);
    }

    // Display the detail of one piece
    public void DisplayOnePieceOverlay(int pieceIndex)
    {
        GetComponentInChildren<PieceOverlayRenderer>().OnClickJigsawPiece(pieceIndex);
    }

    // Add current jigsaw tray index to change current tray in one tray mode
    public void AddJigsawTrayIndex(int amount)
    {
        _CurrentJigsawTrayIndex += amount;
        if (_CurrentJigsawTrayIndex > _AllJigsawTray.Length - 1)
        {
            _CurrentJigsawTrayIndex = 0;
        }
        else if (_CurrentJigsawTrayIndex < 0)
        {
            _CurrentJigsawTrayIndex = _AllJigsawTray.Length - 1;
        }
        GetComponentInChildren<JigsawTrayRenderer>().SetJigsawTray(CurrentJigsawTray);
    }

    // Customize back button to have an ability of exiting the one tray mode
    public void OnClickBackButton(string previousScene)
    {
        // If in one tray mode, go back to all tray
        if (_OneTrayHolder.activeInHierarchy)
        {
            _AllTrayHolder.SetActive(true);
            _OneTrayHolder.SetActive(false);
        }
        // Else, go back to research lab
        else
        {
            SceneManager.LoadScene(previousScene);
        }
    }
}
