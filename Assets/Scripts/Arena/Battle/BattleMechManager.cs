using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static BattleManager;

public class BattleMechManager : MonoBehaviour
{
    public enum State
    {
        Standby,
        Charging,
        Firing,
        Dead
    }

    bool _IsAlly;
    ArenaMechDisplay _MyMech;

    [SerializeField] DelayedBar _HPBar;
    [SerializeField] Bar _SPBar;

    float _HpMax, _HpCurrent;
    public float HpCurrent => _HpCurrent;
    float _SpMax, _SpCurrent, _SpFallBehind, _SpPause, _SpMarker;

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
        ChangeState(State.Standby);
    }

    private void OnDestroy()
    {
        PhaseChange -= OnChangePhase;
    }

    /*
    private void Update()
    {
        switch (_CurrentState)
        {
            case State.Standby:
                break;
            case State.Charging:
                if (_Sp[1] < _Sp[0])
                {
                    _Sp[1] += _SpRefill;
                }
                else
                {
                    _Sp[1] = 0;
                    Debug.Log("Filled");
                    // _BasicStatus = BasicStatus.Firing;
                }
                _SPBar.ChangeCurrent(_Sp[1]);
                break;
            case State.Firing:
                break;
            case State.Dead:
                break;
            default:
                break;
        }
    }
    */

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
                }
            }

            _SPBar.ChangeCurrent((_SpCurrent % _SpMax) / _SpMax);

            yield return null;
        }
    }

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

    private void OnChangePhase(Phase p)
    {
        Debug.Log("CHANGING PHASE to " + p);
        switch (p)
        {
            case Phase.Countdown:
                break;
            case Phase.Battle:
                ChangeState(State.Charging);
                break;
            case Phase.SuddenDeath:
                _SpMax /= 1.5f;
                ChangeState(State.Charging);
                break;
            case Phase.End:
                StopAllCoroutines();
                break;
            case Phase.Transition:
                if (_Action != null) StopCoroutine(_Action);
                break;
            default:
                break;
        }
    }

    public void ChangeState(State s)
    {
        if (CurrentState != State.Dead)
        {
            CurrentState = s;
            Debug.Log("CHANGING STATE to " + CurrentState);
            switch (CurrentState)
            {
                case State.Standby:
                    break;
                case State.Charging:
                    _SpFallBehind += Announcer.Timer - _SpMarker;
                    _Action = StartCoroutine(ChargeAttack());
                    break;
                case State.Firing:
                    _MyMech.StartAttacking(BattleManager.Instance.RequestAttack(_IsAlly));
                    break;
                case State.Dead:
                    _MyMech.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    BattleManager.Instance.Dead(_IsAlly);
                    break;
                default:
                    break;
            }

        }
    }

    public void SetChromo(MechChromoSO c, ArenaMechDisplay m, bool b)
    {
        _IsAlly = b;
        _MySO = c;

        _HpMax = _MySO.Hp.Sum();
        _HpCurrent = _MySO.Hp.Sum();
        _HPBar.InitVal(_HpMax);

        _SpMax = 7f - 4f * (_MySO.Spd.Sum() / (MechChromoSO.Cap * 3f));
        _SPBar.InitVal(1f);
        _SPBar.ChangeCurrent(0f);

        _MyMech = m;
    }
    public void Attacked(MechChromoSO Attacker)
    {
        float atk = Attacker.Atk.Sum() * (CurrentPhase == Phase.SuddenDeath ? 1.5f : 1f);
        float dmg = Mathf.Max(atk - _MySO.Def.Sum(),
            atk * (MechChromoSO.Cap * 3 - _MySO.Def.Sum()) / (MechChromoSO.Cap * 3));
        _HpCurrent -= Mathf.Min(_HpCurrent, dmg);
        _HPBar.ChangeCurrent(_HpCurrent);

        if (_HpCurrent == 0) ChangeState(State.Dead);
    }

    // ------------ Pauser -------------
    public void AskPauseCharging(float a)
    {
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
