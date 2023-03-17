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
    enum Place { Farm, Arena }
    // Chromosome of this mech
    public MechChromoSO MySO;

    // head, body-line, body-color, acc
    [SerializeField] public SpriteRenderer[] myRenderer;
    [SerializeField] Rigidbody2D myRigidbody;
    [SerializeField] Animator myAnimator;

    // Debug
    Vector2 move;
    float timeToMove;
    bool moving;
    Place whereAmI;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        timeToMove = UnityEngine.Random.Range(.5f, 3f);
        moving = false;
    }

    private void Update()
    {
        switch (whereAmI)
        {
            case Place.Farm:
                timeToMove -= Time.fixedDeltaTime;

                if (timeToMove < 0 && !moving)
                {
                    moving = true;
                    StartCoroutine(MoveMe());
                }
                break;
            case Place.Arena:
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (whereAmI)
        {
            case Place.Farm:
                myAnimator.SetFloat("Speed", move.sqrMagnitude);
                if (move.sqrMagnitude > .01 && move.x != 0) myAnimator.SetFloat("Horizontal", Mathf.CeilToInt(move.x));
                if (moving) myRigidbody.MovePosition(myRigidbody.position + move * Time.fixedDeltaTime);
                break;
            case Place.Arena:
                break;
            default:
                break;
        }
    }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(MySO.ID + ": " + string.Join("-", MySO.GetChromosome()[0].Take(5)) + "\n"
            + string.Join("-", MySO.GetChromosome()[0].Skip(5)));
    }
}
