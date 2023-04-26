using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static BattleManager;

public class Announcer : MonoBehaviour
{
    public static float Timer { get; private set; }
    // 0 - Announcer, 1 - Timer
    [SerializeField] TextMeshProUGUI[] _Texts;
    [SerializeField] GameObject[] _Lights;
    [SerializeField] Sprite[] _Sprites;

    Coroutine _Countdown;

    private void Awake()
    {
        PhaseChange += OnPhaseChange;
    }

    private void OnDestroy()
    {
        PhaseChange -= OnPhaseChange;
    }

    private void OnPhaseChange(Phase p)
    {
        if (p != Phase.End && p != Phase.Transition)
            StartCoroutine(StartCountdown(p));
        else if (p == Phase.End) StopAllCoroutines();
    }

    /*
     * 0 - start
     * 1 - battle
     * 2 - sudden death
     */

    public IEnumerator StartCountdown(Phase p)
    {
        Timer = 0f;
        float maxTime = 0f;

        switch (p)
        {
            case Phase.Countdown:
                maxTime = 3f;
                break;
            case Phase.Battle:
                maxTime = 20f;
                break;
            case Phase.SuddenDeath:
                maxTime = 10f;
                break;
        }

        Debug.Log("Start Timer");
        
        while (Timer <= maxTime)
        {
            _Texts[1].text = string.Concat("00:", string.Format("{0:00}", Mathf.Ceil(maxTime - Timer)));
            Timer += Time.deltaTime;
            // Display Light
            if (p == Phase.Countdown && Timer % 1 == 0)
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

        ChangePhase(Phase.Transition);

        foreach (var item in _Lights)
        {
            item.SetActive(false);
        }
        _Texts[1].text = string.Concat("00:00");

        switch (p)
        {
            case Phase.Countdown:
                _Texts[0].text = "Fight";
                break;
            case Phase.Battle:
                _Texts[0].color = Color.red;
                _Texts[1].color = Color.red;
                _Texts[0].text = "Sudden Death";
                break;
            case Phase.SuddenDeath:
                _Texts[0].text = "End";
                break;
        }
        
        // yield return new WaitForSeconds(2f);

        switch (p)
        {
            case Phase.Countdown:
                ChangePhase(Phase.Battle);
                break;
            case Phase.Battle:
                ChangePhase(Phase.SuddenDeath);
                break;
            case Phase.SuddenDeath:
                ChangePhase(Phase.End);
                break;
        }
    }
}
