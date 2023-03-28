using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using static FitnessMenu;
using System;

public class FitnessSelector : MonoBehaviour
{
    // Select part of mech to prioritize
    [SerializeField] TMP_Dropdown Selector;

    // Settings holders
    [SerializeField] GameObject[] Holders;

    // For customization
    [SerializeField] Image HeadDisplay;
    [SerializeField] Slider HeadAdjustor;
    [SerializeField] TMP_Dropdown BodyAdjustor;
    [SerializeField] Image AccDisplay;
    [SerializeField] Slider AccAdjustor;
    [SerializeField] TMP_Dropdown CombatAdjustor;

    // For deactivation
    [SerializeField] Button DeleteButton;

    // For easier data extraction
    [HideInInspector] public FitnessMenu.Properties Type;
    [HideInInspector] public int Value;

    FarmSO myFarm => PlayerManager.CurrentFarmDatabase;

    private void OnEnable()
    {
        FarmSO.OnFarmChangeStatus += OnChangeStatus;
        OnChangeStatus(myFarm, myFarm.Status);
    }
    private void OnDisable()
    {
        FarmSO.OnFarmChangeStatus -= OnChangeStatus;
    }

    private void Update()
    {
        // set type and val to be used
        Type = (FitnessMenu.Properties)Selector.value - 1;
        switch (Type)
        {
            case FitnessMenu.Properties.Head:
                Value = (int)HeadAdjustor.value;
                break;
            case FitnessMenu.Properties.Body:
                Value = BodyAdjustor.value;
                break;
            case FitnessMenu.Properties.Acc:
                Value = (int)AccAdjustor.value;
                break;
            case FitnessMenu.Properties.Com:
                Value = CombatAdjustor.value;
                break;
            default:
                Value = 0;
                break;
        }

        /*
        var dropDownList = Selector.transform.Find("Dropdown List");
        if (dropDownList != null)
        {
            Toggle[] options = dropDownList.GetComponentsInChildren<Toggle>();
            options[0].interactable = false;
        }
        */
    }

    private void OnChangeStatus(FarmSO f, Status s)
    {
        if (f == myFarm)
        {
            // Change behavior depending on status
            switch (s)
            {
                case Status.IDLE:
                    // Activate interactables
                    Selector.interactable = true;
                    HeadAdjustor.interactable = true;
                    BodyAdjustor.interactable = true;
                    AccAdjustor.interactable = true;
                    CombatAdjustor.interactable = true;
                    DeleteButton.interactable = true;
                    break;
                case Status.BREEDING:
                    // Deactivate interactables
                    Selector.interactable = false;
                    HeadAdjustor.interactable = false;
                    BodyAdjustor.interactable = false;
                    AccAdjustor.interactable = false;
                    CombatAdjustor.interactable = false;
                    DeleteButton.interactable = false;
                    break;
                default:
                    break;
            }
        }
    }

    // Close this selector
    public void Deactivate()
    {
        // Reset stuffs
        Selector.value = 0;
        HeadAdjustor.value = 0;
        BodyAdjustor.value = 0;
        AccAdjustor.value = 0;
        CombatAdjustor.value = 0;
        OnTypeChange();
        OnSlider(true);
        OnSlider(false);

        gameObject.SetActive(false);
        transform.SetAsLastSibling();
    }

    /*
     * Load corresponding sprite of head/acc
     * 
     * Input
     *      mode: T - head / F - Tail
     */
    public void OnSlider(bool mode)
    {
        switch (mode)
        {
            case true:
                HeadDisplay.sprite = Resources.Load<Sprite>
                    (Path.Combine("Sprites", "Mech", "Heads", "Head" + (HeadAdjustor.value + 1)));
                break;
            case false:
                AccDisplay.sprite = Resources.Load<Sprite>
                    (Path.Combine("Sprites", "Mech", "Accs", "Plus" + (AccAdjustor.value + 1)));
                break;
        }
        Resources.UnloadUnusedAssets();
    }

    // Change adjustors to match selected part of mech
    public void OnTypeChange()
    {
        foreach (var item in Holders)
        {
            item.SetActive(false);
        }
        if (Selector.value > 0) Holders[Selector.value - 1].SetActive(true);
    }

    public void SetValue(Tuple<Properties, int> pair)
    {
        Type = pair.Item1;
        Value = pair.Item2;

        Selector.value = (int)Type + 1;
        switch (Type)
        {
            case FitnessMenu.Properties.Head:
                HeadAdjustor.value = Value;
                break;
            case FitnessMenu.Properties.Body:
                BodyAdjustor.value = Value;
                break;
            case FitnessMenu.Properties.Acc:
                AccAdjustor.value = Value;
                break;
            case FitnessMenu.Properties.Com:
                CombatAdjustor.value = Value;
                break;
            default:
                break;
        }
    }
}
