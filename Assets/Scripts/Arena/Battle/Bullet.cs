using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    MechChromoSO _Sender;
    public MechChromoSO Sender => _Sender;

    List<Vector2> pts = new List<Vector2>();
    float tParam;
    Vector2 pos;
    bool runMe;

    private void Awake()
    {
        tParam = 0;
        runMe = false;
    }

    private void Update()
    {
        if (runMe) StartCoroutine(Go());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag != "Bullet")
        {
            if (collision.gameObject.GetComponent<ArenaMechDisplay>().NotDeadYet())
                collision.gameObject.SendMessage("Attacked", _Sender);
            Destroy(gameObject);
        }
    }

    public void Set(Vector2 receiver, Color color, MechChromoSO sender)
    {
        GetComponent<SpriteRenderer>().color = color;
        pts.Add(transform.position);
        pts.Add(new Vector2(transform.position.x, 4f));
        pts.Add(new Vector2(receiver.x, 4f));
        pts.Add(receiver);
        _Sender = sender;

        runMe = true;
    }

    private IEnumerator Go()
    {
        runMe = false;

        while (tParam < 1)
        {
            tParam += Time.deltaTime * 1.5f;

            pos = Mathf.Pow(1 - tParam, 3) * pts[0] +
                3 * Mathf.Pow(1 - tParam, 2) * tParam * pts[1] +
                3 * (1 - tParam) * Mathf.Pow(tParam, 2) * pts[2] +
                Mathf.Pow(tParam, 3) * pts[3];

            transform.position = pos;
            yield return new WaitForEndOfFrame();
        }

        tParam = 0f;
        Destroy(gameObject);
    }
}
