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

    public void SetOverlay(LockableObject clickedFacility)
    {
        _CloseOverlayButton.SetActive(clickedFacility.LockStatus == LockableStatus.Lock);
        _ConfirmationButtons.SetActive(clickedFacility.LockStatus == LockableStatus.Unlockable);
        FacilityName.text = clickedFacility.GetLockableObjectName();
        foreach (Transform child in _RequirementHolder)
        {
            Destroy(child.gameObject);
        }
        foreach (UnlockRequirementData unlockRequirementData in clickedFacility.GetUnlockRequirements())
        {
            GameObject newUnlockRequirement = Instantiate(_UnlockRequirementPrefab, _RequirementHolder);
            newUnlockRequirement.GetComponent<UnlockRequirementUI>().SetUnlockRequirement(unlockRequirementData, Color.black);
        }
    }
}