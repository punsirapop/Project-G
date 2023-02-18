using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

/*
 * Control Chromosome detail tab
 */
public class ChromoDetail : PlayerManager
{
    public static ChromoDetail Instance;

    // List of textboxes
    [SerializeField] TextMeshProUGUI[] Displays;
    // Delete button
    // ***** For debug purposes *****
    [SerializeField] Button DeleteButton;

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }

    public void SetDisplay(ChromosomeSO c)
    {
        Displays[0].text = c.ID.ToString();
        Displays[1].text = c.Head.ToString();
        Displays[2].text = string.Join("-", c.Body);
        Displays[3].text = c.Acc.ToString();
        Displays[4].text = c.Atk.Sum().ToString();
        Displays[5].text = c.Def.Sum().ToString();
        Displays[6].text = c.Hp.Sum().ToString();
        Displays[7].text = c.Spd.Sum().ToString();
        Displays[8].text = string.Join("-", c.GetChromosome());
        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(() => FarmManager.Instance.OpenPanel(2));
        DeleteButton.onClick.AddListener(() => DelChromo(c));
    }
}