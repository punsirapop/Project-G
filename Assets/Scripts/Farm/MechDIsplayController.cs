using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * Control a single chromosome on the farm
 * Carry attributes of a mech
 */
public class MechDisplayController : MonoBehaviour, IPointerDownHandler
{
    // Chromosome of this mech
    public MechChromoSO MySO;
    public static event Action<MechChromoSO> OnSelectChromo;

    // head, body-line, body-color, acc
    [SerializeField] public SpriteRenderer[] myRenderer;

    public void SetChromo(MechChromoSO c)
    {
        MySO = c;
        myRenderer[0].sprite = Resources.Load(Path.Combine("Sprites", "Mech", "Heads", "Head" + c.Head)) as Sprite;
        myRenderer[2].color = new Color32((byte)MySO.Body[0], (byte)MySO.Body[1], (byte)MySO.Body[2], 255);
        myRenderer[3].sprite = Resources.Load(Path.Combine("Sprites", "Mech", "Accs", "Plus" + c.Head)) as Sprite;
    }

    public void FixedUpdate()
    {
        // Change color to match the chromosome
    }

    // Trigger when clicked
    public void OnPointerDown(PointerEventData data)
    {
        OnSelectChromo?.Invoke(MySO);
    }
}
