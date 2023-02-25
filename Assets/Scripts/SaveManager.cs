using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SaveHolder
{
    public struct SaveData
    {
        // public List<int> mechCurrentGen;
        // public int mechCurrentID;
    }

    string mechAssetsPath = Path.Combine("Assets", "Resources", "Mechs");
    string mechResourcePath = "Mechs";
    public SaveData Save()
    {
        // ---------- Save normal data ----------
        SaveData saveData = new SaveData();
        // saveData.mechCurrentGen = new List<int>(FarmManager.CurrentGen);
        // saveData.mechCurrentID = FarmManager.CurrentID;

        // ---------- Save objects ----------
        // Mechs
        MechChromoSO[] loaded = FarmManager.Instance.FarmsData.SelectMany(x => x.MechChromos).ToArray();
        MechChromoSO[] saved = Resources.LoadAll<MechChromoSO>(mechResourcePath);
        Debug.Log("LOADED: " + loaded.Count() + " SAVED: " + saved.Length);
        Debug.Log("UNSAVED: " + loaded.Except(saved).Count() + " UNLOADED: " + saved.Except(loaded).Count());
        // Save new items
        foreach (var item in loaded.Except(saved))
        {
            AssetDatabase.CreateAsset(item, Path.Combine(mechAssetsPath, item.ID.ToString() + ".asset"));
        }
        // Delete unused items
        foreach (var item in saved.Except(loaded))
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        /*
        for (int i = 0; i < 4; i++)
        {
            foreach (var item in FarmManager.Instance.FarmsData[i].MechChromos)
            {
                if (saved.Contains(item))
                {
                    AssetDatabase.CreateAsset(item,
                        Path.Combine(mechResourcePath, item.ID.ToString() + ".asset"));
                }
            }
        }
        */
        /*
        mechTempPath = Path.Combine(mechDefaultPath, "TEMP");
        for (int i = 0; i < 4; i++)
        {
            mechNewPath = Path.Combine(mechDefaultPath, i.ToString());
            Debug.Log("im at " + mechNewPath);
            // move obj to temp
            Debug.Log(AssetDatabase.RenameAsset(mechNewPath, "TEMP"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            // Debug.Log("Moved " + mechNewPath + " to " + mechTempPath + " Count: " + Resources.LoadAll(mechTempPath).Length);
            // AssetDatabase.DeleteAsset(mechNewPath);
            // create folder
            Directory.CreateDirectory(mechNewPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            // AssetDatabase.CreateFolder(mechDefaultPath, i.ToString());
            // Debug.Log("Folder active status: " + Directory.Exists(mechNewPath));
            // save new obj
            foreach (var item in FarmManager.Instance.FarmsData[i].MechChromos)
            {
                string savePath = Path.Combine(mechNewPath, item.ID.ToString() + ".asset");
                Debug.Log("File name: " + savePath);
                // Debug.Log("FUCK U " + !File.Exists(savePath));
                if (!File.Exists(savePath)) AssetDatabase.CreateAsset(item, savePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

            }
            AssetDatabase.DeleteAsset(mechTempPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        */

        return saveData;
    }

    public void LoadData(SaveData s)
    {
        // ---------- Load normal data ----------
        /*
        FarmManager.CurrentGen = new List<int>(s.mechCurrentGen);
        FarmManager.CurrentID = s.mechCurrentID;
        */

        // ---------- Load objects ----------
        // Mechs
        /*
        List<List<MechChromoSO>> m = new List<List<MechChromoSO>>();
        for (int i = 0; i < 4; i++)
        {
            mechNewPath = Path.Combine("Mechs", i.ToString());
            m.Add(Resources.LoadAll<MechChromoSO>(mechNewPath).ToList());
        }
        FarmManager.MechChromo = new List<List<MechChromoSO>>(m);
        */
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
        path = Path.Combine(Application.dataPath, "Save.json");
        persistentPath = Path.Combine(Application.persistentDataPath, "Save.json");

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
        SaveHolder.SaveData saveData = saver.Save();
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
        saver.LoadData(loadData);
        Debug.Log("Data Loaded");
    }

    public void ResetGame()
    {
        Debug.Log("PLS RESET");
        MechChromoSO[] deleteMe = Resources.LoadAll<MechChromoSO>("Mechs");
        foreach (MechChromoSO mechChromoSO in deleteMe)
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(mechChromoSO));
        OnReset?.Invoke();
    }
}