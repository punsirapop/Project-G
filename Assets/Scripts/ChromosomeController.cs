using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * Control a single chromosome on the farm
 * Carry attributes of a mech
 */
public class ChromosomeController : MonoBehaviour, IPointerDownHandler
{
    // Chromosome of this mech
    public ChromosomeSC MySC;
    public static event Action<ChromosomeSC> OnSelectChromo;

    SpriteRenderer myRenderer;

    private void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
    }

    public void FixedUpdate()
    {
        // Change color to match the chromosome
        myRenderer.color = new Color32((byte)MySC.Body[0], (byte)MySC.Body[1], (byte)MySC.Body[2], 255);
    }

    // Trigger when clicked
    public void OnPointerDown(PointerEventData data)
    {
        OnSelectChromo?.Invoke(MySC);
    }
}
