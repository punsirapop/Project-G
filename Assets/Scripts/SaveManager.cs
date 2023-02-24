using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveHolder
{
    public struct SaveData
    {
        // public List<int> CurrentGen;
    }

    string defaultPath = Path.Combine("Assets", "ScriptableObjects", "Mechs");
    string newPath = "";
    public SaveData Save()
    {
        // ---------- Save normal data ----------
        SaveData saveData = new SaveData();
        // saveData.CurrentGen = new List<int>(PlayerManager.CurrentGen);

        // ---------- Save objects ----------
        // Mechs
        for (int i = 0; i < 4; i++)
        {
            newPath = Path.Combine(defaultPath, i.ToString());
            // delete olf obj
            AssetDatabase.DeleteAsset(newPath);
            // create folder
            AssetDatabase.CreateFolder(defaultPath, i.ToString());
            // save new obj
            foreach (var item in FarmManager.Instance.FarmsData[i].MechChromos)
            {
                string savePath = Path.Combine(newPath, item.ID.ToString() + ".asset");
                AssetDatabase.CreateAsset(item, savePath);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return saveData;
    }

    public void LoadData(SaveData s)
    {

    }
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public static event Action OnReset;

    SaveHolder saver = new SaveHolder();
    string path = "";
    string persistentPath = "";

    string currentPath;

    protected bool SaveReady()
    {
        return File.Exists(currentPath);
    }

    protected void SetPaths()
    {
        path = Application.dataPath + Path.DirectorySeparatorChar + "Save.json";
        persistentPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Save.json";

        currentPath = path;
    }

    /*
    public void SaveData()
    {
        playerDataSC.ChromoSave = new List<List<ChromosomeSC>>(PlayerManager.Chromosomes);
    }

    public void LoadData()
    {
        PlayerManager.Chromosomes = new List<List<ChromosomeSC>>(playerDataSC.ChromoSave);
    }
    */

    public void SaveToFile()
    {
        SetPaths();
        string savePath = currentPath;

        Debug.Log("Saving at " + savePath);
        SaveHolder.SaveData saveData =  saver.Save();
        string json = JsonUtility.ToJson(saveData);

        using StreamWriter streamWriter = new StreamWriter(savePath);
        streamWriter.Write(json);
        streamWriter.Close();

        Debug.Log(json);
    }

    public void LoadFromFile()
    {
        SetPaths();
        string loadPath = currentPath;

        using StreamReader streamReader = new StreamReader(loadPath);
        string json = streamReader.ReadToEnd();
        streamReader.Close();

        SaveHolder.SaveData loadData = JsonUtility.FromJson<SaveHolder.SaveData>(json);
        //data.Update();
        Debug.Log("Data Loaded");
    }

    public void ResetGame()
    {
        Debug.Log("PLS RESET");
        OnReset?.Invoke();
    }
}
