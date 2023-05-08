using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : Bar
{
    [SerializeField] Button _SkillButton;
    [SerializeField] Image _Icon;

    WeaponChromosome _MyChromo;
    bool _IsAlly;
    bool _IsDead;

    Coroutine _Activated;

    private void OnEnable()
    {
        _IsDead = false;
    }

    protected override void Update()
    {
        base.Update();

        _SkillButton.interactable = CurrentFill <= 0 && _IsAlly &&
            !_MyMech.Effects.Any(x => x.Type == SelfEffects.Sleep) && !_IsDead;

        if (CurrentFill <= 0 && !_IsAlly && _Activated == null &&
            !_MyMech.Effects.Any(x => x.Type == SelfEffects.Sleep) && !_IsDead)
        {
            _Activated = StartCoroutine(AutoActivation());
        }
    }

    private IEnumerator AutoActivation()
    {
        yield return new WaitForSeconds(Random.Range(.5f, 3f));
        Used();
        _Activated = null;
    }

    public void SetWeapon(WeaponChromosome w, BattleMechManager b)
    {
        if (w == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);

            InitVal(w.Cooldown, b);

            _Icon.sprite = ArenaManager.GetWeaponImage(w.FromFactory, w.IsMode1Active ? 0 : 1);
            _IsAlly = b.IsAlly;

            _MyChromo = w;
        }
    }

    public void Used()
    {
        CurrentFill = Max;
        transform.parent.SendMessage("UseSkill", _MyChromo.CurrentMode);
    }

    public override void Dead()
    {
        base.Dead();
        _IsDead = true;
    }
}
