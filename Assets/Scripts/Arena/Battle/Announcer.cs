using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static BattleManager;
using UnityEngine.UI;

public class Announcer : MonoBehaviour
{
    public static float Timer { get; private set; }
    // 0 - Announcer, 1 - Timer
    [SerializeField] TextMeshProUGUI[] _Texts;
    [SerializeField] GameObject[] _Lights;
    [SerializeField] Sprite[] _Sprites;
    [SerializeField] Image _Flag;
    [SerializeField] GameObject _BackButton;

    Coroutine _Countdown;
    float _MaxTime;

    private void Awake()
    {
        PhaseChange += OnPhaseChange;
    }

    private void OnDisable()
    {
        Reset();
    }

    private void OnDestroy()
    {
        PhaseChange -= OnPhaseChange;
    }

    private void Reset()
    {
        _Texts[0].text = "";
        _Texts[0].color = Color.green;
        _Texts[1].text = "";
        _Texts[1].color = Color.green;
        _Flag.sprite = _Sprites[0];
        Timer = 0f;
        _MaxTime = 0f;
    }

    private void OnPhaseChange(Phases p)
    {
        if (p != Phases.End && p != Phases.Transition)
        {
            if(p != Phases.Battle)
            {
                SoundEffectManager.Instance.PlaySoundEffect("TimeOut");
            }
            StartCoroutine(StartCountdown(p));
        }
        else if (p == Phases.End)
        { 
            SoundEffectManager.Instance.PlaySoundEffect("TimeOut");
            StopAllCoroutines();
            StartCoroutine(Ending());
        }
    }

    private IEnumerator Ending()
    {
        yield return new WaitForSeconds(4f);
        switch (WinningStatus)
        {
            case 0:
                _Texts[0].color = Color.green;
                _Texts[0].text = "Win";
                _Flag.sprite = _Sprites[1];
                SoundEffectManager.Instance.PlaySoundEffect("Win");
                break;
            case 1:
                _Texts[0].color = Color.red;
                _Texts[0].text = "Lose";
                _Flag.sprite = _Sprites[2];
                SoundEffectManager.Instance.PlaySoundEffect("Lose");
                break;
            case 2:
                _Texts[0].color = Color.white;
                _Texts[0].text = "Tie";
                SoundEffectManager.Instance.PlaySoundEffect("Tie");
                break;
        }
        _Texts[1].text = "";
        _BackButton.SetActive(true);
    }

    /*
     * 0 - start
     * 1 - battle
     * 2 - sudden death
     */

    public IEnumerator StartCountdown(Phases p)
    {
        switch (p)
        {
            case Phases.Countdown:
                _MaxTime += 3f;
                break;
            case Phases.Battle:
                _MaxTime += 20f;
                break;
            case Phases.SuddenDeath:
                _MaxTime += 10f;
                break;
        }

        Debug.Log("Start Timer");
        
        while (Timer <= _MaxTime)
        {
            _Texts[1].text = string.Concat("00:", string.Format("{0:00}", Mathf.Ceil(_MaxTime - Timer)));
            Timer += Time.deltaTime;
            // Display Light
            if (p == Phases.Countdown && Timer % 1 == 0)
            {
                foreach (var item in _Lights)
                {
                    if (!item.activeSelf)
                    {
                        item.SetActive(true);
                        break;
                    }
                }
            }
            yield return null;
        }
        

        /*
        for (float i = maxTime; i > 0; i--)
        {
            _Texts[1].text = string.Concat("00:", string.Format("{0:00}", i));
            if (p == Phase.Countdown)
            {
                foreach (var item in _Lights)
                {
                    if (!item.activeSelf)
                    {
                        item.SetActive(true);
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(1f);
        }
        */

        ChangePhase(Phases.Transition);

        foreach (var item in _Lights)
        {
            item.SetActive(false);
        }
        _Texts[1].text = string.Concat("00:00");

        switch (p)
        {
            case Phases.Countdown:
                _Texts[0].text = "Fight";
                break;
            case Phases.Battle:
                _Texts[0].color = Color.red;
                _Texts[1].color = Color.red;
                _Texts[0].text = "Sudden Death";
                break;
            case Phases.SuddenDeath:
                _Texts[0].text = "End";
                break;
        }
        
        // yield return new WaitForSeconds(2f);

        switch (p)
        {
            case Phases.Countdown:
                SoundEffectManager.Instance.PlaySoundEffect("StartTimer");
                ChangePhase(Phases.Battle);
                break;
            case Phases.Battle:
                ChangePhase(Phases.SuddenDeath);
                break;
            case Phases.SuddenDeath:
                ChangePhase(Phases.End);
                break;
        }
    }
}
