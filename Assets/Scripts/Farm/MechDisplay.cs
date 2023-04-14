using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * Control a single chromosome on the farm
 * Carry attributes of a mech
 */
public class MechDisplay : MonoBehaviour, IPointerClickHandler
{
    // Place for setting behavior
    enum Place { Farm, Habitat, Arena }
    // Chromosome of this mech
    public MechChromoSO MySO;

    // head, body-line, body-color, acc
    [SerializeField] public SpriteRenderer[] myRenderer;
    [SerializeField] Rigidbody2D myRigidbody;
    [SerializeField] Animator myAnimator;

    Vector2 move;
    float timeToMove;
    bool moving;
    Place whereAmI;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        timeToMove = UnityEngine.Random.Range(500f, 5000f) * Time.fixedDeltaTime;
        moving = false;
    }

    private void Update()
    {
        // Set behavior from place
        switch (whereAmI)
        {
            case Place.Farm:
                // Random movement
                timeToMove -= Time.fixedDeltaTime;

                if (timeToMove < 0 && !moving)
                {
                    moving = true;
                    StartCoroutine(MoveMe());
                }
                break;
            case Place.Habitat:
                break;
            case Place.Arena:
                break;
        }
    }

    private void FixedUpdate()
    {
        // Set behavior from place
        switch (whereAmI)
        {
            case Place.Farm:
                // Random movement
                myAnimator.SetFloat("Speed", move.sqrMagnitude);
                if (move.sqrMagnitude > .01 && move.x != 0)
                    myAnimator.SetFloat("Horizontal", Mathf.CeilToInt(move.x));
                if (moving) myRigidbody.MovePosition(myRigidbody.position + move * Time.fixedDeltaTime);
                break;
            case Place.Arena:
                break;
            case Place.Habitat:
                break;
        }
    }

    // Set vector and duration for mach movement
    private IEnumerator MoveMe()
    {
        move.x = UnityEngine.Random.Range(-1f, 1f);
        move.y = UnityEngine.Random.Range(-1f, 1f);
        yield return new WaitForSeconds(UnityEngine.Random.Range(10f, 100f) * Time.fixedDeltaTime);
        timeToMove = UnityEngine.Random.Range(1000f, 5000f) * Time.fixedDeltaTime;
        moving = false;
        move.x = 0f;
        move.y = 0f;
        yield break;
    }

    /*
     * Set mech to match a chromosome
     * 
     * Input:
     *      c: chromosome scriptable object
     */
    public void SetChromo(MechChromoSO c)
    {
        MySO = c;
        myRenderer[0].sprite = Resources.Load<Sprite>
            (Path.Combine("Sprites", "Mech", "Heads", "Head" + (c.Head+1)));
        myRenderer[2].color = new Color32
            ((byte)MySO.Body[0], (byte)MySO.Body[1], (byte)MySO.Body[2], 255);
        myRenderer[3].sprite = Resources.Load<Sprite>
            (Path.Combine("Sprites", "Mech", "Accs", "Plus" + (c.Acc+1)));
    }

    // ======= Debug ========
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(MySO.ID + ": " + string.Join("-", MySO.GetChromosome()[0].Take(5)) + "\n"
            + string.Join("-", MySO.GetChromosome()[0].Skip(5)));
    }
}