using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FitnessMenu : MonoBehaviour
{
    public enum Properties { Head, Body, Acc, Com }

    [SerializeField] Button AddButton;
    [SerializeField] FitnessSelector[] Selectors;
    // head, body-line, body-color, acc
    // [SerializeField] public Image[] myRenderer;

    FarmSO myFarm;
    // Color[] bodyColor = {Color.red, Color.green, Color.blue, Color.white, Color.black};
    // Color inactive = new Color(.5f, .5f, .5f, .5f);

    private void Awake()
    {
        myFarm = FarmManager.Instance.FarmsData[PlayerManager.CurrentFarm];
    }

    private void Update()
    {
        AddButton.interactable = Selectors.Where(x => x.gameObject.activeSelf).Count() < 5;
    }

    /*
     * Get current fitness preferences
     * 
     * Output
     *      list of preferred values (default = -1)
     */
    public List<Tuple<Properties, int>> CurrentPref()
    {
        List<Tuple<Properties, int>> a = new List<Tuple<Properties, int>>();
        foreach (var item in Selectors)
        {
            if(item.gameObject.activeSelf)
                a.Add(Tuple.Create(item.Type, item.Value));
        }
        return a;
    }

    public Dictionary<dynamic, float> GetFitnessDict()
    {
        Dictionary<dynamic, float> dict = new Dictionary<dynamic, float>();
        foreach (MechChromoSO c in myFarm.MechChromos)
        {
            dict.Add(c, c.GetFitness(CurrentPref()));
        }
        return dict;
    }

    public void OpenSelector()
    {
        foreach (var item in Selectors)
        {
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
                break;
            }
        }
    }

    /*
    // Head/Body/Acc/Combat
    [SerializeField] Toggle[] EnableMe;
    [SerializeField] GameObject[] Titles;
    [SerializeField] GameObject[] Holders;

    [SerializeField] Slider HeadSelect;
    [SerializeField] TextMeshProUGUI HeadDisplay;
    // R-G-B-W-B
    [SerializeField] TMP_Dropdown Body;
    [SerializeField] Slider AccSelect;
    [SerializeField] TextMeshProUGUI AccDisplay;
    // Atk-Def-Hp-Spd
    [SerializeField] TMP_Dropdown Combat;

    void Update()
    {
        for (int i = 0; i < EnableMe.Length; i++)
        {
            Titles[i].SetActive(EnableMe[i].isOn);
            Holders[i].SetActive(EnableMe[i].isOn);
        }

        HeadDisplay.text = (HeadSelect.value + 1).ToString();
        AccDisplay.text = (AccSelect.value + 1).ToString();

        myRenderer[0].sprite = Resources.Load<Sprite>
            (Path.Combine("Sprites", "Mech", "Heads", "Head" + (HeadSelect.value + 1)));
        myRenderer[3].sprite = Resources.Load<Sprite>
            (Path.Combine("Sprites", "Mech", "Accs", "Plus" + (AccSelect.value + 1)));

        myRenderer[0].color = EnableMe[0].isOn ? Color.white : Color.gray;
        myRenderer[1].color = EnableMe[1].isOn ? Color.white : Color.gray;
        myRenderer[2].color = EnableMe[1].isOn ? bodyColor[Body.value] : Color.gray;
        myRenderer[3].color = EnableMe[2].isOn ? Color.white : Color.gray;
    }
    */
}
