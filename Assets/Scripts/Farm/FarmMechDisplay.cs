using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmMechDisplay : MechDisplay
{
    [SerializeField] Rigidbody2D myRigidbody;
    [SerializeField] Animator myAnimator;

    Vector2 move;
    float timeToMove;
    bool moving;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        timeToMove = Random.Range(500f, 5000f) * Time.fixedDeltaTime;
        moving = false;
    }

    private void Update()
    {
        // Random movement
        timeToMove -= Time.fixedDeltaTime;

        if (timeToMove < 0 && !moving)
        {
            moving = true;
            StartCoroutine(MoveMe());
        }
    }

    private void FixedUpdate()
    {
        // Random movement
        myAnimator.SetFloat("Speed", move.sqrMagnitude);
        if (move.sqrMagnitude > .01 && move.x != 0)
            myAnimator.SetFloat("Horizontal", Mathf.CeilToInt(move.x));
        if (moving) myRigidbody.MovePosition(myRigidbody.position + move * Time.fixedDeltaTime);
    }

    // Set vector and duration for mach movement
    private IEnumerator MoveMe()
    {
        move.x = Random.Range(-1f, 1f);
        move.y = Random.Range(-1f, 1f);
        yield return new WaitForSeconds(Random.Range(10f, 100f) * Time.fixedDeltaTime);
        timeToMove = Random.Range(1000f, 5000f) * Time.fixedDeltaTime;
        moving = false;
        move.x = 0f;
        move.y = 0f;
        yield break;
    }
}
