using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

/*
 * Control Chromosome detail tab
 */
public class ChromoDetail : MonoBehaviour
{
    public MechChromoSO currentDisplay;
    // List of textboxes
    [SerializeField] TextMeshProUGUI[] Displays;
    [SerializeField] MechCanvasDisplay Icon;
    [SerializeField] Button SwitchButton;
    // Delete button
    // ***** For debug purposes *****
    // [SerializeField] Button DeleteButton;

    public void SetDisplay(MechChromoSO c)
    {
        currentDisplay = c;
        Displays[0].text = "ID: " + c.ID.ToString();
        Displays[1].text = c.Head.ToString();
        Displays[2].text = string.Join("-", c.Body);
        Displays[3].text = c.Acc.ToString();
        Displays[4].text = string.Join("\t", "Atk: " + c.Atk.Sum().ToString(),
            "Def: " + c.Def.Sum().ToString(), "Hp: " + c.Hp.Sum().ToString(),
            "Spd: " + c.Spd.Sum().ToString(), "Rank: " + c.Rank, "Element: " + c.Element);
        Displays[5].text = string.Join("-", c.GetChromosome()[0].Take(5)) + "\n"
            + string.Join("-", c.GetChromosome()[0].Skip(5));
        Icon.SetChromo(c);

        /*
        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(() => FarmManager.Instance.OpenPanel(2));
        DeleteButton.onClick.AddListener(() => FarmManager.Instance.DelChromo(c));
        */
    }
}
