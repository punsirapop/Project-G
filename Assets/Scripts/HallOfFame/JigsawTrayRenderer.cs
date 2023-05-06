using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JigsawTrayRenderer : MonoBehaviour
{
    // Tray information
    [SerializeField] private JigsawTraySO _JigsawTray;

    // Actual UI components for jigsaw tray
    [SerializeField] private GameObject _TwoPiecesGroup;
    [SerializeField] private GameObject _ThreePiecesGroup;
    [SerializeField] private GameObject _FourPiecesGroup;
    [SerializeField] private Image _FrameImage;
    [SerializeField] private Image _OverlayImage;
    [SerializeField] private TextMeshProUGUI _TrayNameText;

    private void OnEnable()
    {
        RenderSprites();
    }

    public void SetJigsawTray(JigsawTraySO newJigsawTray)
    {
        _JigsawTray = newJigsawTray;
        RenderSprites();
    }

    public void RenderSprites()
    {
        _FrameImage.sprite = _JigsawTray.FrameSprite;
        _OverlayImage.sprite = _JigsawTray.OverlaySprite;
        _TrayNameText.text = _JigsawTray.Name;
        JigsawPieceGroupSO[] jigsawGroups = _JigsawTray.AllJigsawPieceGroup;
        // Active corresponding pieces layout
        _TwoPiecesGroup.SetActive(jigsawGroups.Length == 2);
        _ThreePiecesGroup.SetActive(jigsawGroups.Length == 3);
        _FourPiecesGroup.SetActive(jigsawGroups.Length == 4);
        // Render each piece with the color corresponding to the level
        JigsawPieceRenderer[] jigsawRenderers = GetComponentsInChildren<JigsawPieceRenderer>();
        for (int jigsawIndex = 0; jigsawIndex < jigsawGroups.Length; jigsawIndex++)
        {
            // Looping through all rank of this piece to calculate highest rank of this piece
            JigsawLevel jigsawLevel = jigsawGroups[jigsawIndex].GetHighestObtainedLevel();
            jigsawRenderers[jigsawIndex].SetColor(jigsawLevel);
        }
    }
}
