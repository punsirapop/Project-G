using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BattleManager;
using static MechChromoSO;

public class BattleMechManager : MonoBehaviour
{
    public enum State
    {
        Standby,
        Charging,
        Firing,
        Dead
    }

    public bool IsAlly { get; private set; }
    public int Index { get; private set; }
    public List<EffectDisplay> Effects { get; private set; }

    ArenaMechDisplay _MyMech;
    WeaponChromosome _MyWeapon;

    [SerializeField] SkillButton _WeaponHolder;
    [SerializeField] DelayedBar _HPBar;
    [SerializeField] Bar _SPBar;
    [SerializeField] Transform _EffectHolder;
    [SerializeField] GameObject _Effect, _DamageDisplay;

    float _HpMax;
    float _HpCurrent;
    public float HpCurrent => _HpCurrent;
    float _SpMax;
    float _SpCurrent, _SpFallBehind, _SpPause, _SpMarker;
    float _SkillCooldown;

    Coroutine _ChangingStateAction;

    /*
    float[] _Sp;
    float _SpRefill;
    */

    MechChromoSO _MySO;
    Coroutine _Action;

    bool _Pause;

    public State CurrentState {get; private set;}

    private void Awake()
    {
        PhaseChange += OnChangePhase;
        // ChangeState(State.Standby);
    }

    private void OnDestroy()
    {
        PhaseChange -= OnChangePhase;
    }

    private IEnumerator ChargeAttack()
    {
        // _SpCurrent = _SPBar.CurrentFIll;
        // float _SpMax = 5f;

        while (CurrentState == State.Charging)
        {
            if (!_Pause)
            {
                _SpCurrent = Announcer.Timer - _SpPause - _SpFallBehind;

                if (_SpCurrent >= _SpMax)
                {
                    _SpFallBehind += _SpMax;
                    _SpMarker = Announcer.Timer;
                    ChangeState(State.Firing);

                    if (Effects.Any(x => x.Type == SelfEffects.Poison))
                    {
                        foreach (var item in Effects.Where(x => x.Type == SelfEffects.Poison))
                        {
                            WeaponChromosome giverW = BattleManager.Instance.Identify(item.Giver, 1);
                            MechChromoSO giver = BattleManager.Instance.Identify(item.Giver, 2);
                            float dmg = giver.Atk.Sum() / 5f + giverW.Efficiency * giver.Atk.Sum() / 2f;
                            ReduceHP(dmg, DamageMode.Poison);
                        }
                    }
                }
            }

            _SPBar.ChangeCurrent((_SpCurrent % _SpMax) / _SpMax);

            yield return null;
        }
    }
 
    private IEnumerator Cooldown()
    {
        _WeaponHolder.ChangeCurrent(_WeaponHolder.Max / 3f);
        while (CurrentState != State.Dead)
        {
            if (_WeaponHolder.isActiveAndEnabled && _WeaponHolder.CurrentFill > 0 &&
                !Effects.Any(x => x.Type == SelfEffects.Sleep))
            {
                _WeaponHolder.ChangeCurrent(_WeaponHolder.CurrentFill - Time.deltaTime);
            }
            yield return null;
        }
    }

    /*
    private IEnumerator Countdown()
    {
        _SkillCooldown = -_WeaponHolder.Max * 2 / 3;
        while (CurrentState != State.Dead)
        {
            if (_WeaponHolder.isActiveAndEnabled && _WeaponHolder.CurrentFill > 0)
            {
                _WeaponHolder.ChangeCurrent(_WeaponHolder.Max - (Announcer.Timer - _SkillCooldown));
            }
            yield return null;
        }
    }
    */

    /*
    private IEnumerator ChargeAttack()
    {
        Debug.Log("Start charging");
        
        while (_CurrentState == State.Charging)
        {
            _Sp[1] = Announcer.Timer % _Sp[0];
            _SPBar.ChangeCurrent(_Sp[1] / _Sp[0]);
            yield return null;
        }
    }
    */
    /*
    private IEnumerator ChargeAttack()
    {
        Debug.Log("Start charging");
        while (_CurrentState == State.Charging)
        {
            if (_Sp[1] < _Sp[0])
            {
                _Sp[1] += _SpRefill;
            }
            else
            {
                _Sp[1] = 0;
                Debug.Log("Filled");
                ChangeState(State.Firing);
                // _BasicStatus = BasicStatus.Firing;
            }
            _SPBar.ChangeCurrent(_Sp[1]);
            yield return null;
        }
    }
    */

    private void OnChangePhase(Phases p)
    {
        Debug.Log("CHANGING PHASE to " + p);
        switch (p)
        {
            case Phases.Countdown:
                ChangeState(State.Standby);
                break;
            case Phases.Battle:
                if (CurrentState != State.Dead)
                {
                    ChangeState(State.Charging);
                    StartCoroutine(Cooldown());
                }
                break;
            case Phases.SuddenDeath:
                _SpMax /= 1.5f;
                // if (CurrentState != State.Dead) ChangeState(State.Charging);
                break;
            case Phases.End:
                StopAllCoroutines();
                _MyMech.ForceStopAttack(true);
                _MyMech.ForceStopAttack(false);
                break;
            case Phases.Transition:
                // if (_Action != null) StopCoroutine(_Action);
                break;
            default:
                break;
        }
    }

    private void InitStuffs()
    {
        _HpCurrent = _HpMax;
        _HPBar.InitVal(_HpMax, this);

        _SPBar.InitVal(1f, this);
        _SPBar.ChangeCurrent(0f);
        _SpCurrent = 0f;
        _SpPause = 0f;
        _SpFallBehind = 0f;
        _SpMarker = 0f;

        _MyMech.gameObject.SetActive(true);
        gameObject.SetActive(true);
        Effects = new List<EffectDisplay>();

        foreach (Transform item in _EffectHolder)
        {
            Destroy(item.gameObject);
        }
    }

    private IEnumerator ChangeStateActions(State s)
    {
        switch (s)
        {
            case State.Standby:
                InitStuffs();
                break;
            case State.Charging:
                _SpFallBehind += Announcer.Timer - _SpMarker;
                _Action = StartCoroutine(ChargeAttack());
                break;
            case State.Firing:
                ArenaMechDisplay target = null;
                target = BattleManager.Instance.RequestAttack(new int[] { IsAlly ? 0 : 1, Index }, false);
                if (target == null)
                {
                    Debug.Log("BLANK!");
                    break;
                }

                if (Effects.Any(x => x.Type == SelfEffects.Snipe))
                {
                    _MyMech.StartAttacking(target, BulletType.Snipe, true);
                }
                else if (Effects.Any(x => x.Type == SelfEffects.Pierce))
                {
                    _MyMech.StartAttacking(target, BulletType.Pierce, true);
                }
                else
                {
                    _MyMech.StartAttacking(target, BulletType.Default, true);
                }
                break;
            case State.Dead:
                // _MyMech.gameObject.SetActive(false);
                // gameObject.SetActive(false);
                _HPBar.Dead();
                _SPBar.Dead();
                _WeaponHolder.Dead();
                _MyMech.Dead();
                Effects = new List<EffectDisplay>();
                foreach (Transform item in _EffectHolder)
                {
                    Destroy(item.gameObject);
                }
                BattleManager.Instance.Dead(new int[] { IsAlly ? 0 : 1, Index });
                break;
            default:
                break;
        }
        yield return null;
    }

    public void ChangeState(State s)
    {
        CurrentState = s;
        Debug.Log("CHANGING STATE to " + CurrentState);
        if (_ChangingStateAction != null) StopCoroutine(_ChangingStateAction);
        _ChangingStateAction = StartCoroutine(ChangeStateActions(s));
    }

    public void ReduceHP(float dmg, DamageMode dm)
    {
        _HpCurrent -= Mathf.Min(_HpCurrent, dmg);
        _HPBar.ChangeCurrent(Mathf.Min(_HpMax, _HpCurrent));

        GameObject dmd = Instantiate(_DamageDisplay,
            new Vector2(transform.position.x + Random.Range(-.5f, .5f), transform.position.y + 4f),
            Quaternion.identity, transform);
        dmd.GetComponent<DamageDisplay>().SetDamage(dmg, dm);

        if (_HpCurrent <= 0) ChangeState(State.Dead);
    }

    public void UseSkill()
    {
        int[] giver = new int[] { IsAlly ? 0 : 1, Index };
        if (_MyWeapon.FromFactory < 2)
        {
            SelfEffects type = new SelfEffects();

            switch (_MyWeapon.FromFactory)
            {
                case 0:
                    type = _MyWeapon.IsMode1Active ? SelfEffects.Taunt : SelfEffects.Stealth;
                    break;
                case 1:
                    type = _MyWeapon.IsMode1Active ? SelfEffects.Snipe : SelfEffects.Pierce;
                    break;
                default:
                    break;
            }

            GetNewEffect(type, giver);
        }
        else
        {
            switch (_MyWeapon.FromFactory)
            {
                case 2:
                    _MyMech.StartAttacking
                        (BattleManager.Instance.RequestAttack
                        (new int[] { IsAlly ? 0 : 1, Index }, false),
                        _MyWeapon.IsMode1Active ? BulletType.Sleep : BulletType.Poison, false);
                    break;
                case 3:
                    _MyMech.StartAttacking
                        (BattleManager.Instance.RequestAttack
                        (new int[] { IsAlly ? 0 : 1, Index }, _MyWeapon.IsMode1Active),
                        _MyWeapon.IsMode1Active ? BulletType.AoEHeal : BulletType.AoEDamage, false);
                    break;
                default:
                    break;
            }
        }
    }

    public void GetNewEffect(SelfEffects type, int[] giver)
    {
        GameObject effect = Instantiate(_Effect, _EffectHolder);
        EffectDisplay effectDis = effect.GetComponent<EffectDisplay>();
        float dur = 3f;
        // adjust duration for sleep
        if (type == SelfEffects.Sleep)
        {
            dur = 3f + ((WeaponChromosome)BattleManager.Instance.Identify(giver, 1)).Efficiency * 4f;
            AskPauseCharging(dur);
        }
        effectDis.Set(type, giver, this, dur);
        Effects.Add(effectDis);
    }

    public void SetChromo(MechChromoSO c, ArenaMechDisplay m, bool b, int i, WeaponChromosome w)
    {
        IsAlly = b;
        Index = i;

        _MySO = c;
        _MyMech = m;
        _MyWeapon = w;

        // _WeaponHolder.gameObject.SetActive(_MyWeapon != null);
        _WeaponHolder.SetWeapon(_MyWeapon, this);

        _HpMax = _MySO.Hp.Sum() + (_MyWeapon != null ? _MyWeapon.BonusStat.Hp : 0);
        _SpMax = 7f - 4f * ((_MySO.Spd.Sum() + 
            (_MyWeapon != null ? _MyWeapon.BonusStat.Spd : 0)) / (MechChromoSO.Cap * 3f));

        CurrentState = State.Standby;
        InitStuffs();
    }

    /*
    public void ActivateSkill()
    {
        Debug.Log("Activated");
        _SkillCooldown = Announcer.Timer;
    }
    */

    // ------------ Pauser -------------
    public void AskPauseCharging(float a)
    {
        Debug.Log($"Pause for {a}");
        StartCoroutine(PauseCharging(a));
    }

    private IEnumerator PauseCharging(float a)
    {
        _Pause = true;
        float timer = Announcer.Timer;
        yield return new WaitUntil(() => Announcer.Timer - timer >= a);
        timer = Announcer.Timer - timer;
        _SpPause += timer;
        _Pause = false;
    }
}
