using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/*
// Data saved
public class SaveInspector
{
    public List<List<ChromosomeSO>> Chromosomes => PlayerManager.Chromosomes;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
}

public class SaveFormat
{
    public List<List<ChromosomeSO>> Chromosomes = new List<List<ChromosomeSO>>();

    public void SetUp(SaveInspector si)
    {
        Chromosomes = new List<List<ChromosomeSO>>(si.Chromosomes);
    }

    public void Update()
    {
        PlayerManager.Chromosomes = new List<List<ChromosomeSO>>(Chromosomes);
    }
}
*/

/*
public class SaveHolder
{
    public List<string> Habitat;
    public List<string> Farm1;
    public List<string> Farm2;
    public List<string> Farm3;

    public void Save()
    {
        List<string> list = new List<string>();
        for (int i = 0; i < 4; i++)
        {
            list.Clear();
            foreach (var c in PlayerManager.Chromosomes[i])
            {
                list.Add(string.Join("-", c.GetChromosome()));
            }

            switch (i)
            {
                case 0:
                    Habitat = list;
                    break;
                case 1:
                    Farm1 = list;
                    break;
                case 2:
                    Farm2 = list;
                    break;
                case 3:
                    Farm3 = list;
                    break;
            }
        }
    }
}
*/

public class SaveHolder
{
    public struct SaveData
    {
        public List<int> CurrentGen;
    }

    string defaultPath = Path.Combine("Assets", "ScriptableObjects", "Mechs");
    string newPath = "";
    public SaveData Save()
    {
        // ---------- Save normal data ----------
        SaveData saveData = new SaveData();
        saveData.CurrentGen = new List<int>(PlayerManager.CurrentGen);

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
            foreach (var item in PlayerManager.Chromosomes[i])
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
}
