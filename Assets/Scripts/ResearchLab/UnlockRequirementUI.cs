using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockRequirementUI : MonoBehaviour
{
    [SerializeField] private Sprite _CheckMark;
    [SerializeField] private Sprite _CrossMark;

    // Actual UI element
    [SerializeField] private Image _SatisfactionImage;
    [SerializeField] private TextMeshProUGUI _RequirementText;

    public void SetUnlockRequirement(UnlockRequirementData unlockRequirementData, Color textColor)
    {
        _SatisfactionImage.sprite = unlockRequirementData.IsSatisfy ? _CheckMark : _CrossMark;
        _RequirementText.text = unlockRequirementData.Header + ": " + unlockRequirementData.Description;
        _RequirementText.color = textColor;
    }
}
