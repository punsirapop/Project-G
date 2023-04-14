using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AMechAllyDisplay : MechCanvasDisplay
{
    [SerializeField] TextMeshProUGUI _Description;
    [SerializeField] Button _Button;

    public override void SetChromo(MechChromoSO c)
    {
        if (MySO == c)
        {
            foreach (var item in myRenderer)
            {
                item.gameObject.SetActive(true);
            }
            _Description.gameObject.SetActive(true);
            base.SetChromo(c);
            _Description.text = string.Join("/", c.ID, c.Atk, c.Def, c.Hp, c.Spd);
        }
        else
        {
            foreach (var item in myRenderer)
            {
                item.gameObject.SetActive(false);
            }
            _Description.gameObject.SetActive(false);
        }
    }

    public void SetButton(bool b)
    {
        _Button.interactable = b;
    }
}
