using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class Popup : MonoBehaviour
{
    [SerializeField] GameObject _Popup, _Overwrite, _NewGame;
    [SerializeField] TMP_InputField _InputField;
    [SerializeField] Button _StartNewGame;

    string path = "";
    string persistentPath = "";
    protected string currentPath;

    private void Start()
    {
        path = Path.Combine(Application.dataPath, "Save", "Save.json");
        persistentPath = Path.Combine(Application.persistentDataPath, "Save", "Save.json");

        currentPath = persistentPath;
    }

    private void Update()
    {
        _StartNewGame.interactable = !string.IsNullOrEmpty(_InputField.text);
    }

    public void Open()
    {
        _Popup.SetActive(true);
        _Overwrite.SetActive(File.Exists(currentPath));
        _NewGame.SetActive(!File.Exists(currentPath));
    }

    public void SaveName()
    {
        PlayerManager.Name = _InputField.text;
    }
}
