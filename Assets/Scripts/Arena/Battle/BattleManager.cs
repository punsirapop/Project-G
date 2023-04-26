using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public enum Phase
    {
        Countdown,      // prepare for battle
        Battle,         // battle
        SuddenDeath,    // battle intensely
        End,            // end
        Transition      // inbetween state
    }

    public static event Action<Phase> PhaseChange;
    public static BattleManager Instance;
    public static Phase CurrentPhase;

    [SerializeField] BattleMechManager[] _AllyBattleStats, _EnemyBattleStats;
    [SerializeField] ArenaMechDisplay[] _AllyBattleLineUp, _EnemyBattleLineUp;
    [SerializeField] Button _FightButton;
    [SerializeField] EndGameOverlay _EndGameOverlay;

    MechChromoSO[] _AllyTeam => AllySelectionManager.Instance.AllyTeam;
    MechChromoSO[] _EnemyTeam => EnemySelectionManager.Instance.EnemyTeam;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        PhaseChange += EndGame;
    }

    private void OnDestroy()
    {
        PhaseChange -= EndGame;
    }

    public static void ChangePhase(Phase p)
    {
        CurrentPhase = p;
        PhaseChange?.Invoke(CurrentPhase);
    }

    public void InitiateSequence()
    {
        for (int i = 0; i < 3; i++)
        {
            _AllyBattleStats[i].SetChromo(_AllyTeam[i], _AllyBattleLineUp[i], true);
            _AllyBattleLineUp[i].SetMech(_AllyBattleStats[i]);

            _EnemyBattleStats[i].SetChromo(_EnemyTeam[i], _EnemyBattleLineUp[i], false);
            _EnemyBattleLineUp[i].SetMech(_EnemyBattleStats[i]);
        }

        ChangePhase(Phase.Countdown);
    }

    public void CheckStartFight()
    {
        _FightButton.interactable = !_AllyTeam.Any(x => x == null) && !_EnemyTeam.Any(x => x == null);
    }

    public ArenaMechDisplay RequestAttack(bool isSentFromAlly)
    {
        float[] chances = new float[] { 50, 30, 20 };

        for (int i = 0; i < chances.Length; i++)
        {
            if ((isSentFromAlly ? _EnemyBattleStats : _AllyBattleStats)
                [i].CurrentState == BattleMechManager.State.Dead)
            {
                chances[i] = 0;
            }
        }

        int target = 0;
        float random = UnityEngine.Random.Range(0, chances.Sum());
        float sum = 0;

        for (; target < 3; target++)
        {
            sum += chances[target];
            if (random < sum) break;
        }

        return (isSentFromAlly ? _EnemyBattleLineUp : _AllyBattleLineUp)[target];
    }

    public void Dead(bool isAlly)
    {
        if ((isAlly ? _AllyBattleStats : _EnemyBattleStats).
            All(x => x.CurrentState == BattleMechManager.State.Dead))
        {
            ChangePhase(Phase.End);
        }
    }

    private void EndGame(Phase p)
    {
        if (p == Phase.End)
        {
            foreach (var item in _AllyBattleLineUp) item.gameObject.SetActive(false);
            foreach (var item in _EnemyBattleLineUp) item.gameObject.SetActive(false);
            float allyHp = _AllyBattleStats.Select(x => x.HpCurrent).Sum();
            float enemyHp = _EnemyBattleStats.Select(x => x.HpCurrent).Sum();
            _EndGameOverlay.gameObject.SetActive(true);
            _EndGameOverlay.Set(allyHp > enemyHp);
        }
    }

    // -------------- Debug ----------------
    public void AskForPause(float a)
    {
        _AllyBattleStats[0].SendMessage("AskPauseCharging", a);
    }
}
