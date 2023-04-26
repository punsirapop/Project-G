using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGameOverlay : MonoBehaviour
{
    [SerializeField] Button _Continue;
    [SerializeField] TextMeshProUGUI _EndText;

    public void Set(bool isWon)
    {
        _Continue.interactable = isWon;
        _EndText.text = isWon ? "You Win" : "You Lose";
    }
}
