using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMng : MonoBehaviour
{
    [SerializeField] GameObject[] overlays;
    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    // Overloading ChangeScene for string parameter
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ToggleOverlay(int index)
    {
        switch (overlays[index].activeSelf)
        {
            case true:
                overlays[index].SetActive(false);
                break;
            case false:
                foreach (var item in overlays)
                {
                    item.SetActive(false);
                }
                overlays[index].SetActive(true);
                break;
        }
    }
}
