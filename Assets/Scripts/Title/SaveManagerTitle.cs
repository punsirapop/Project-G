using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveManagerTitle : SaveManager
{
    // ref. to farms data as FarmManager still isn't loaded
    [SerializeField] FarmSO[] loadTargets;
    [SerializeField] Button LoadBtn;

    void Start()
    {
        SetPaths();
    }

    void Update()
    {
        LoadBtn.interactable = SaveReady();
    }

    public new void LoadFromFile()
    {
        // ---------- Load json ----------
        SetPaths();
        string loadPath = currentPath;

        using StreamReader streamReader = new StreamReader(loadPath);
        string json = streamReader.ReadToEnd();
        streamReader.Close();

        SaveData loadData = JsonUtility.FromJson<SaveData>(json);
        LoadJson(loadData);

        // ---------- Load scriptable objects ----------
        for (int i = 0; i < savedFarms.Length; i++)
        {
            loadTargets[i].SetMe(savedFarms[i]);
        }
        Debug.Log("Data Loaded");
    }
}