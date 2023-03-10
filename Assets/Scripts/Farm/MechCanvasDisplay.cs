using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/*
 * Control a single chromosome on the farm
 * Carry attributes of a mech
 */
public class MechCanvasDisplay : MonoBehaviour
{
    // Chromosome of this mech
    public MechChromoSO MySO;

    // head, body-line, body-color, acc
    [SerializeField] public Image[] myRenderer;

    public void SetChromo(MechChromoSO c)
    {
        MySO = c;
        myRenderer[0].sprite = Resources.Load<Sprite>
            (Path.Combine("Sprites", "Mech", "Heads", "Head" + (c.Head + 1)));
        myRenderer[2].color = new Color32
            ((byte)MySO.Body[0], (byte)MySO.Body[1], (byte)MySO.Body[2], 255);
        myRenderer[3].sprite = Resources.Load<Sprite>
            (Path.Combine("Sprites", "Mech", "Accs", "Plus" + (c.Acc + 1)));
    }
}
