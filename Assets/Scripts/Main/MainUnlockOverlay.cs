using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainUnlockOverlay : MonoBehaviour
{
    [SerializeField] private GameObject _CloseOverlayButton;
    [SerializeField] private GameObject _ConfirmationButtons;
    [SerializeField] private TextMeshProUGUI FacilityName;
    [SerializeField] private Transform _RequirementHolder;
    [SerializeField] private GameObject _UnlockRequirementPrefab;

    public void SetOverlay(FactorySO clickedFactory)
    {
        _CloseOverlayButton.SetActive(clickedFactory.LockStatus == LockableStatus.Lock);
        _ConfirmationButtons.SetActive(clickedFactory.LockStatus == LockableStatus.Unlockable);
        FacilityName.text = clickedFactory.Name;
        foreach (Transform child in _RequirementHolder)
        {
            Destroy(child.gameObject);
        }
        foreach (UnlockRequirementData unlockRequirementData in clickedFactory.GetUnlockRequirements())
        {
            GameObject newUnlockRequirement = Instantiate(_UnlockRequirementPrefab, _RequirementHolder);
            newUnlockRequirement.GetComponent<UnlockRequirementUI>().SetUnlockRequirement(unlockRequirementData, Color.black);
        }
    }
}