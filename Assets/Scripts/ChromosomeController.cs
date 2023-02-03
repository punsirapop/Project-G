using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * Control a single chromosome on the farm
 */
public class ChromosomeController : MonoBehaviour, IPointerDownHandler
{
    public ChromosomeSC mySC;
    public static event Action<ChromosomeSC> OnSelectChromo;

    SpriteRenderer myRenderer;

    private void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
    }

    public void FixedUpdate()
    {
        myRenderer.color = new Color32((byte)mySC.Body[0], (byte)mySC.Body[1], (byte)mySC.Body[2], 255);
    }

    public void OnPointerDown(PointerEventData data)
    {
        OnSelectChromo?.Invoke(mySC);
    }
}
