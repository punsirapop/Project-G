using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JigsawPieceRenderer : MonoBehaviour
{
    [SerializeField] int _PieceIndex;

    private void Start()
    {
        this.gameObject.AddComponent<Button>();
        GetComponent<Button>().onClick.AddListener(OnClickJigsawPiece);
    }

    public void OnClickJigsawPiece()
    {
        HallOfFameManager.Instance.DisplayOnePieceOverlay(_PieceIndex);
    }

    // Color the jigsaw piece according to the level
    public void SetColor(JigsawLevel level)
    {
        Color pieceColor = HallOfFameManager.Instance.EmptyPieceColor;
        switch(level)
        {
            case JigsawLevel.Copper:
                pieceColor = HallOfFameManager.Instance.CopperPieceColor;
                break;
            case JigsawLevel.Silver:
                pieceColor = HallOfFameManager.Instance.SilverPieceColor;
                break;
            case JigsawLevel.Gold:
                pieceColor = HallOfFameManager.Instance.GoldPieceColor;
                break;
        }
        GetComponent<Image>().color = pieceColor;
    }
}
