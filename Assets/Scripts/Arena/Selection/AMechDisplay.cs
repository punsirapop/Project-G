using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AMechDisplay : MechCanvasDisplay
{
    [SerializeField] GameObject _MechIcon;
    [SerializeField] TextMeshProUGUI _Description;
    public override void SetChromo(MechChromoSO c)
    {
        if (MySO != c)
        {
            base.SetChromo(c);

            _MechIcon.SetActive(true);
            _Description.gameObject.SetActive(true);
            _Description.text = string.Join("/",
                "Atk:" + c.Atk.Sum(), "Def:" + c.Def.Sum(),
                "Hp:" + c.Hp.Sum(), "Spd:" + c.Spd.Sum());
        }
        else
        {
            MySO = null;
            _MechIcon.SetActive(false);
            _Description.gameObject.SetActive(false);
        }
    }
}
