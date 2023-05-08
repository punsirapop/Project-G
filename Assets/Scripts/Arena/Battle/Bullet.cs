using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Team (0: ally, 1: enemy), AttackerIndex, BulletMode
    int[] _BattlePackage;

    SpriteRenderer _SpriteRenderer;
    ArenaMechDisplay _Target;
    Vector2 _StartPos;
    Vector2 _TargetPos;
    float _TimeToTarget = .75f;

    private void Awake()
    {
        _BattlePackage = new int[3];
        _SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Bullet")
        {
            ArenaMechDisplay col = collision.gameObject.GetComponent<ArenaMechDisplay>();
            if (col == _Target && col.NotDeadYet())
            {
                // Debug.Log("Attack hit!");
                collision.gameObject.SendMessage("Attacked", _BattlePackage);
                Destroy(gameObject);
            }
        }
    }

    public void Set(ArenaMechDisplay receiver, Color color, bool isAlly, int index, BulletType effect)
    {
        _Target = receiver;
        _StartPos = transform.position;
        _TargetPos = receiver.transform.position;
        _SpriteRenderer.sprite = ArenaManager.GetBulletSprite(effect);
        if ((int)effect < 3) _SpriteRenderer.color = color;

        _BattlePackage[0] = isAlly ? 0 : 1;
        _BattlePackage[1] = index;
        _BattlePackage[2] = (int)effect;

        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        float timeElapsed = 0f;

        while (timeElapsed < _TimeToTarget)
        {
            float t = timeElapsed / _TimeToTarget;
            t = Mathf.Clamp01(t);
            Vector2 nextPos = Vector2.Lerp(_StartPos, _TargetPos, t);
            float yOffset = Mathf.Sin(t * Mathf.PI) * 2f;
            nextPos.y += yOffset;
            transform.position = nextPos;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (_Target == BattleManager.Instance.Identify(_BattlePackage, 3))
        {
            _Target.SendMessage("Attacked", _BattlePackage);
        }

        Destroy(gameObject);
    }
}
