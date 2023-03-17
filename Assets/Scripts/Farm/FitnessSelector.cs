using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class FitnessSelector : MonoBehaviour
{
    [SerializeField] TMP_Dropdown Selector;

    [SerializeField] GameObject[] Holders;

    [SerializeField] Image HeadDisplay;
    [SerializeField] Slider HeadAdjustor;
    [SerializeField] TMP_Dropdown BodyAdjustor;
    [SerializeField] Image AccDisplay;
    [SerializeField] Slider AccAdjustor;
    [SerializeField] TMP_Dropdown CombatAdjustor;

    [HideInInspector] public FitnessMenu.Properties Type;
    [HideInInspector] public int Value;

    private void OnEnable()
    {
        Selector.value = 0;
        HeadAdjustor.value = 0;
        BodyAdjustor.value = 0;
        AccAdjustor.value = 0;
        CombatAdjustor.value = 0;
        OnTypeChange();
        OnSlider(true);
        OnSlider(false);
    }

    private void Update()
    {
        // set type and val to be used
        Type = (FitnessMenu.Properties)Selector.value;
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
        }
    }

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
                break ;
        }
        Resources.UnloadUnusedAssets();
    }

    public void OnTypeChange()
    {
        foreach (var item in Holders)
        {
            item.SetActive(false);
        }
        Holders[Selector.value].SetActive(true);
    }
}
