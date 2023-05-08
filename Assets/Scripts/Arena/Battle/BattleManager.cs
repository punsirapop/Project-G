using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static MechChromoSO;

public class BattleManager : MonoBehaviour
{
    public enum Phases
    {
        Countdown,      // prepare for battle
        Battle,         // battle
        SuddenDeath,    // battle intensely
        End,            // end
        Transition      // inbetween state
    }

    public static event Action<Phases> PhaseChange;
    public static BattleManager Instance;
    public static Phases CurrentPhase;
    // 0: win, 1: lose, 2: tie
    public static int WinningStatus;

    [SerializeField] BattleMechManager[] _AllyBattleStats, _EnemyBattleStats;
    [SerializeField] ArenaMechDisplay[] _AllyBattleLineUp, _EnemyBattleLineUp;
    [SerializeField] Button _FightButton;
    [SerializeField] GameObject _StatPanel;
    [SerializeField] EndResult _EndResult;

    MechChromoSO[] _AllyTeam => AllySelectionManager.Instance.AllyMech;
    MechChromoSO[] _EnemyTeam => EnemySelectionManager.Instance.EnemyTeam;
    WeaponChromosome[] _AllyWeapon => AllySelectionManager.Instance.AllyWeapon;
    WeaponChromosome[] _EnemyWeapon => EnemySelectionManager.Instance.EnemyWeapon;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        PhaseChange += EndGame;
    }

    private void OnDestroy()
    {
        PhaseChange -= EndGame;
    }

    public static void ChangePhase(Phases p)
    {
        CurrentPhase = p;
        PhaseChange?.Invoke(CurrentPhase);
    }

    public void InitiateSequence()
    {
        for (int i = 0; i < 3; i++)
        {
            _AllyBattleStats[i]
                .SetChromo(_AllyTeam[i], _AllyBattleLineUp[i], true, i, _AllyWeapon[i]);
            _AllyBattleLineUp[i].SetMech(_AllyBattleStats[i]);

            _EnemyBattleStats[i]
                .SetChromo(_EnemyTeam[i], _EnemyBattleLineUp[i], false, i, _EnemyWeapon[i]);
            _EnemyBattleLineUp[i].SetMech(_EnemyBattleStats[i]);
        }

        ChangePhase(Phases.Countdown);
    }

    public void CheckStartFight()
    {
        _FightButton.interactable = !_AllyTeam.Any(x => x == null) && !_EnemyTeam.Any(x => x == null) &&
            !_AllyWeapon.Any(x => x == null) && !_EnemyWeapon.Any(x => x == null);
    }

    // Randomly choose attack target
    public ArenaMechDisplay RequestAttack(int[] sender, bool reqSameSide)
    {
        float[] chances = new float[] { 50f, 30f, 20f };
        BattleMechManager[] targetList = sender[0] == (reqSameSide ? 1 : 0) ? _EnemyBattleStats : _AllyBattleStats;
        WeaponChromosome[] weaponList = sender[0] == (reqSameSide ? 1 : 0) ? _EnemyWeapon : _AllyWeapon;
        BattleMechManager senderMech = Identify(sender, 0);
        WeaponChromosome senderWeapon = Identify(sender, 1);

        // check if dead
        for (int i = 0; i < chances.Length; i++)
        {
            if (targetList[i].CurrentState == BattleMechManager.State.Dead)
            {
                chances[i] = 0;
            }
        }
        // check for receiver ongoing effects
        for (int i = 0; i < chances.Length; i++)
        {
            if (targetList[i].Effects.Where(x => x.Type == SelfEffects.Taunt).Count() > 0)
            {
                foreach (var item in targetList[i].Effects.Where(x => x.Type == SelfEffects.Taunt))
                {
                    EditChance(chances, i, weaponList[i].Efficiency, 1);
                }
            }
            if (targetList[i].Effects.Where(x => x.Type == SelfEffects.Stealth).Count() > 0)
            {
                foreach (var item in targetList[i].Effects.Where(x => x.Type == SelfEffects.Stealth))
                {
                    EditChance(chances, i, weaponList[i].Efficiency, -1);
                }
            }
        }
        // check for sender ongoing effects
        if (senderMech.Effects.Any(x => x.Type == SelfEffects.Snipe))
        {
            foreach (var item in senderMech.Effects.Where(x => x.Type == SelfEffects.Snipe))
            {
                EditChance(chances, 2, senderWeapon.Efficiency, 1);
            }
        }
        // check if someone is alive but 0 chance of attacking
        if (targetList.Any(x => x.CurrentState != BattleMechManager.State.Dead) && chances.Sum() <= 0)
        {
            chances[Array.IndexOf(targetList, targetList
                .First(x => x.CurrentState != BattleMechManager.State.Dead))] = 1;
        }

        int target = 0;
        float random = UnityEngine.Random.Range(0, chances.Sum());
        float sum = 0;

        for (; target < 3; target++)
        {
            sum += chances[target];
            if (random < sum) break;
        }

        int[] encodedTarget = new int[] { sender[0] == (reqSameSide ? 1 : 0) ? 1 : 0, target };
        Debug.Log(string.Join("/", chances) + $" Target: {target}");

        return Identify(encodedTarget, 3);
    }

    public void AttackReport(int[] attacker, int[] receiver)
    {
        BattleMechManager Attacker = Identify(attacker, 0);
        WeaponChromosome AttackerW = Identify(attacker, 1);
        MechChromoSO AttackerSO = Identify(attacker, 2);
        BattleMechManager Receiver = Identify(receiver, 0);
        WeaponChromosome ReceiverW = Identify(receiver, 1);
        MechChromoSO ReceiverSO = Identify(receiver, 2);

        switch ((BulletType)attacker[2])
        {
            case BulletType.Sleep:
                Receiver.GetNewEffect(SelfEffects.Sleep, attacker);
                break;
            case BulletType.Poison:
                Receiver.GetNewEffect(SelfEffects.Poison, attacker);
                break;
            case BulletType.AOEHeal:
                foreach (var item in (receiver[0] == 0 ? _AllyBattleStats : _EnemyBattleStats)
                    .Where(x => x.CurrentState != BattleMechManager.State.Dead))
                {
                    float heal = AttackerSO.Atk.Sum() * .2f + AttackerW.Efficiency * AttackerSO.Atk.Sum() * .3f;
                    item.ReduceHP(-heal, DamageMode.Heal);
                }
                break;
            case BulletType.AOEDamage:
                foreach (var item in (receiver[0] == 0 ? _AllyBattleStats : _EnemyBattleStats)
                    .Where(x => x.isActiveAndEnabled))
                {
                    float atk2 = AttackerSO.Atk.Sum() * 2f / 3f 
                        + AttackerW.Efficiency * AttackerSO.Atk.Sum() / 3f;

                    float def2 = ReceiverSO.Def.Sum() + ReceiverW.BonusStat.Def;

                    // check sender ongoing effects
                    if (Attacker.Effects.Any(x => x.Type == SelfEffects.Pierce))
                    {
                        foreach (var item2 in Attacker.Effects.Where(x => x.Type == SelfEffects.Pierce))
                        {
                            def2 *= .8f - AttackerW.Efficiency * .3f;
                        }
                    }

                    int ele2 = CheckElement(AttackerSO.Element, ReceiverSO.Element);
                    DamageMode dm2;

                    switch (ele2)
                    {
                        case 0:
                            atk2 *= 1.5f;
                            dm2 = DamageMode.Weak;
                            break;
                        case 1:
                            atk2 *= 0.5f;
                            dm2 = DamageMode.Resist;
                            break;
                        default:
                            dm2 = DamageMode.Normal;
                            break;
                    }

                    float dmg2 = Mathf.Max(atk2 - def2, atk2 / 10f);

                    item.ReduceHP(dmg2, dm2);
                }
                break;
            default:
                float atk = (AttackerSO.Atk.Sum() + AttackerW.BonusStat.Atk)
                    * (CurrentPhase == Phases.SuddenDeath ? 1.5f : 1f);

                float def = ReceiverSO.Def.Sum() + ReceiverW.BonusStat.Def;

                // check sender ongoing effects
                if (Attacker.Effects.Any(x => x.Type == SelfEffects.Pierce))
                {
                    foreach (var item in Attacker.Effects.Where(x => x.Type == SelfEffects.Pierce))
                    {
                        def *= .8f - AttackerW.Efficiency * .3f;
                    }
                }

                int ele = CheckElement(AttackerSO.Element, ReceiverSO.Element);
                DamageMode dm;

                switch (ele)
                {
                    case 0:
                        atk *= 1.5f;
                        dm = DamageMode.Weak;
                        break;
                    case 1:
                        atk *= 0.5f;
                        dm = DamageMode.Resist;
                        break;
                    default:
                        dm = DamageMode.Normal;
                        break;
                }

                float dmg = Mathf.Max(atk - def, atk / 10f);

                Receiver.ReduceHP(dmg, dm);
                break;
        }

    }

    // Team - Index
    public void Dead(int[] dead)
    {
        foreach (var item in _AllyBattleLineUp)
        {
            item.StopTBag(Identify(dead, 3));
        }
        if ((dead[0] == 0 ? _AllyBattleStats : _EnemyBattleStats).
            All(x => x.CurrentState == BattleMechManager.State.Dead))
        {
            ChangePhase(Phases.End);
        }
    }

    public void MechBackToIdle()
    {
        foreach (var item in _AllyBattleLineUp)
        {
            item.SwapAppearance(true);
        }
        foreach (var item in _EnemyBattleLineUp)
        {
            item.SwapAppearance(true);
        }
    }

    // Team - Index, 0: BMM - 1: WeaponChromo - 2: MechChromo  - 3: AMD
    public dynamic Identify(int[] me, int mode)
    {
        dynamic[] arrayToCheck;
        switch (mode)
        {
            case 0:
                arrayToCheck = (me[0] == 0 ? _AllyBattleStats : _EnemyBattleStats);
                break;
            case 1:
                arrayToCheck = (me[0] == 0 ? _AllyWeapon : _EnemyWeapon);
                break;
            case 2:
                arrayToCheck = (me[0] == 0 ? _AllyTeam : _EnemyTeam);
                break;
            case 3:
                arrayToCheck = (me[0] == 0 ? _AllyBattleLineUp : _EnemyBattleLineUp);
                break;
            default:
                arrayToCheck = null;
                break;
        }
        if (arrayToCheck.Length > me[1]) return arrayToCheck[me[1]];
        else return null;
    }

    private void EndGame(Phases p)
    {
        if (p == Phases.End)
        {
            StartCoroutine(FightEnding());
        }
    }

    private IEnumerator FightEnding()
    {
        float allyHp = _AllyBattleStats.Select(x => x.HpCurrent).Sum();
        float enemyHp = _EnemyBattleStats.Select(x => x.HpCurrent).Sum();

        if (allyHp > enemyHp) WinningStatus = 0;
        else if (allyHp < enemyHp) WinningStatus = 1;
        else WinningStatus = 2;

        yield return new WaitForSeconds(1f);
        _StatPanel.SetActive(false);

        _EndResult.gameObject.SetActive(true);
        _EndResult.SetResult(WinningStatus);

        //foreach (var item in _AllyBattleLineUp) item.gameObject.SetActive(true);
        //foreach (var item in _EnemyBattleLineUp) item.gameObject.SetActive(false);
    }

    // 0: Attacker wins (weakness), 1: Defender wins (resist), 2: Neutral
    private int CheckElement(Elements dealer, Elements receiver)
    {
        // check main wheel
        if ((dealer == Elements.Fire && receiver == Elements.Plant) ||
                (dealer == Elements.Plant && receiver == Elements.Water) ||
                (dealer == Elements.Water && receiver == Elements.Fire) ||
                (dealer == Elements.Light && receiver == Elements.Dark) ||
                (dealer == Elements.Dark && receiver == Elements.Light))
        {
            return 0;
        }
        else if ((dealer == Elements.Fire && receiver == Elements.Plant) ||
                (dealer == Elements.Plant && receiver == Elements.Water) ||
                (dealer == Elements.Water && receiver == Elements.Fire))
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    private void EditChance(float[] chances, int i, float eff, int mul)
    {
        float offset = (20f + eff * 30f) * mul;
        float s = chances.Sum() - chances[i];
        chances[i] += offset;
        for (int j = 0; j < chances.Length; j++)
        {
            if (j != i)
            {
                chances[j] -= offset * chances[j] / s;
                if (chances[j] < 0) chances[j] = 0;
            }
        }
    }

    // -------------- Debug ----------------
    public void AskForPause(float a)
    {
        _AllyBattleStats[0].SendMessage("AskPauseCharging", a);
    }
}