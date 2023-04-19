using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArenaMechDisplay : MechDisplay
{
    [SerializeField] GameObject _Me;
    public override void SetChromo(MechChromoSO c)
    {
        if (MySO != c)
        {
            base.SetChromo(c);

            _Me.SetActive(true);
        }
        else
        {
            MySO = null;

            _Me.SetActive(false);
        }
    }
}
