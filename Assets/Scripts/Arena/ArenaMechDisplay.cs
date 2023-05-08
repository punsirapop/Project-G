using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BattleManager;

public class ArenaMechDisplay : MechDisplay
{
    public WeaponChromosome MyWeaponSO;
    BattleMechManager _MyMech;

    // 0: Idle, 1: Battle
    [SerializeField] GameObject[] _BodyHolders;
    [SerializeField] SpriteRenderer _BodyColor;
    [SerializeField] SpriteRenderer[] _WeaponSprites;
    [SerializeField] GameObject _WeaponHolder;
    [SerializeField] Transform _BulletSpawn;
    [SerializeField] Animator _MyAnimator;

    [SerializeField] GameObject _Me, _BulletPrefab;

    Coroutine _Attacking, _Skill;
    ArenaMechDisplay _DmgReceiver, _DmgReceiverS;
    GameObject _Bullet, _BulletS;

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
            _BodyColor.color = MyRenderer[2].color;
            _Me.SetActive(true);
        }
        else
        {
            MySO = null;
            
            _Me.SetActive(false);
        }
    }

    public void SetWeapon(WeaponChromosome w)
    {
        if (MyWeaponSO != w)
        {
            MyWeaponSO = w;
            _WeaponSprites[w.FromFactory].gameObject.SetActive(true);
            _WeaponSprites[w.FromFactory].sprite = ArenaManager.GetWeaponRankSprite(w);
        }
        else
        {
            MyWeaponSO = null;
            foreach (var item in _WeaponSprites)
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    private void OnChangePhase(Phases p)
    {
        Debug.Log("CHANGING PHASE to " + p);
        switch (p)
        {
            case Phases.Countdown:
                _BodyHolders[0].SetActive(false);
                _BodyHolders[1].SetActive(true);
                _WeaponHolder.SetActive(true);
                break;
            case Phases.Battle:
                break;
            case Phases.SuddenDeath:
                break;
            case Phases.End:
                if (_Attacking != null) ForceStopAttack(true);
                if (_Skill != null) ForceStopAttack(false);
                
                break;
            case Phases.Transition:
                // if (_Attacking != null) ForceStopAttack();
                break;
            default:
                break;
        }
    }

    private IEnumerator Attacking(ArenaMechDisplay receiver, BulletType type, bool isAttack)
    {
        Coroutine c = isAttack ? _Attacking : _Skill;
        GameObject g = isAttack ? _Bullet : _BulletS;
        ArenaMechDisplay a = isAttack ? _DmgReceiver : _DmgReceiverS;
        a = receiver;
        g = Instantiate(_BulletPrefab, _BulletSpawn.position, Quaternion.identity);
        SoundEffectManager.Instance.PlaySoundEffect("ArenaShoot");
        // Physics2D.IgnoreCollision(g.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        g.GetComponent<Bullet>().Set(receiver, new Color32
            ((byte)MySO.Body[0], (byte)MySO.Body[1], (byte)MySO.Body[2], 255),
            _MyMech.IsAlly, _MyMech.Index, type);
        yield return new WaitUntil(() => g == null);
        a = null;
        c = null;
        _MyMech.ChangeState(BattleMechManager.State.Charging);
    }

    public void StartAttacking(ArenaMechDisplay receiver, BulletType type, bool isAttack)
    {
        if (isAttack)
            _Attacking = StartCoroutine(Attacking(receiver, type, true));
        else
            _Skill = StartCoroutine(Attacking(receiver, type, false));
    }

    public void ForceStopAttack(bool isAttack)
    {
        if (isAttack)
        {
            if (_Attacking != null) StopCoroutine(_Attacking);
            if (_Bullet != null) Destroy(_Bullet);
        }
        else
        {
            if (_Skill != null) StopCoroutine(_Skill);
            if (_BulletS != null) Destroy(_BulletS);
        }
    }

    public bool NotDeadYet()
    {
        return _MyMech.CurrentState != BattleMechManager.State.Dead;
    }

    // Team - Index - BulletType
    public void Attacked(int[] attacker)
    {
        SoundEffectManager.Instance.PlaySoundEffect("ArenaHurt");
        BattleManager.Instance.AttackReport(attacker,
            new int[] {_MyMech.IsAlly ? 0 : 1, _MyMech.Index});
    }

    public void SetMech(BattleMechManager b)
    {
        _MyMech = b;
    }

    public void StopTBag(ArenaMechDisplay dead)
    {
        if (_DmgReceiver == dead) ForceStopAttack(true);
        if (_DmgReceiverS == dead) ForceStopAttack(false);
    }

    public void Dead()
    {
        SoundEffectManager.Instance.PlaySoundEffect("ArenaExplosion");
        StopAllCoroutines();
        _MyAnimator.SetBool("IsDead",true);
    }

    public void SwapAppearance(bool isToIdle)
    {
        _BodyHolders[0].SetActive(isToIdle);
        _BodyHolders[1].SetActive(!isToIdle);
        _WeaponHolder.SetActive(!isToIdle);
        if (isToIdle)
        {
            _BodyColor.color = MyRenderer[2].color;
            _MyAnimator.SetBool("IsDead", false);
        }

    }

    public void TurningGray()
    {
        Color c = _BodyColor.color;
        c = Color.Lerp(c, Color.grey, .5f);
        _BodyColor.color = c;
    }
}
