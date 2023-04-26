using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BattleManager;

public class ArenaMechDisplay : MechDisplay
{
    BattleMechManager _MyMech;

    [SerializeField] GameObject _Me, _BulletPrefab;

    Coroutine _Attacking;
    ArenaMechDisplay _DmgReceiver;
    GameObject _Bullet;

    private void Awake()
    {
        PhaseChange += OnChangePhase;
    }

    private void OnDestroy()
    {
        PhaseChange -= OnChangePhase;
    }

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

    private void OnChangePhase(Phase p)
    {
        Debug.Log("CHANGING PHASE to " + p);
        switch (p)
        {
            case Phase.Countdown:
                break;
            case Phase.Battle:
                break;
            case Phase.SuddenDeath:
                break;
            case Phase.End:
                if (_Attacking != null) ForceStopAttack();
                break;
            case Phase.Transition:
                if (_Attacking != null) ForceStopAttack();
                break;
            default:
                break;
        }
    }

    private IEnumerator Attacking(ArenaMechDisplay receiver)
    {
        _DmgReceiver = receiver;
        _Bullet = Instantiate(_BulletPrefab, transform.position, Quaternion.identity);
        Physics2D.IgnoreCollision(_Bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        _Bullet.GetComponent<Bullet>().Set(receiver.transform.position, new Color32
            ((byte)MySO.Body[0], (byte)MySO.Body[1], (byte)MySO.Body[2], 255), MySO);
        yield return new WaitUntil(() => _Bullet == null);
        _DmgReceiver = null;
        _Attacking = null;
        _MyMech.ChangeState(BattleMechManager.State.Charging);
    }

    public void StartAttacking(ArenaMechDisplay receiver)
    {
        _Attacking = StartCoroutine(Attacking(receiver));
    }

    public void ForceStopAttack()
    {
        StopCoroutine(_Attacking);
        Destroy(_Bullet);
    }

    public bool NotDeadYet()
    {
        return _MyMech.CurrentState != BattleMechManager.State.Dead;
    }

    public void Attacked(MechChromoSO Attacker)
    {
        _MyMech.Attacked(Attacker);
    }

    public void SetMech(BattleMechManager b)
    {
        _MyMech = b;
    }

    public void StopTBag(ArenaMechDisplay dead)
    {
        if (_DmgReceiver == dead) StopCoroutine(_Attacking);
    }
}
