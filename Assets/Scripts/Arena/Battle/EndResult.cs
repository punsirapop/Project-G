using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndResult : MonoBehaviour
{
    [SerializeField] Animator _MyAnimator;
    [SerializeField] TextMeshProUGUI _Result;

    // 0: win, 1: lose, 2: tie
    public void SetResult(int mode)
    {
        switch (mode)
        {
            case 0:
                _Result.color = Color.green;
                _Result.text = "Win";
                break;
            case 1:
                _Result.color = Color.red;
                _Result.text = "Lose";
                break;
            case 2:
                _Result.color = Color.white;
                _Result.text = "Tie";
                break;
        }
        StartCoroutine(DeactivateMe());
    }

    private IEnumerator DeactivateMe()
    {
        Debug.Log("DEACTIVE");
        yield return new WaitForSeconds(2f);
        _MyAnimator.SetTrigger("Deactive");
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
