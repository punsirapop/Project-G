using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    SaveInspector saver = new SaveInspector();

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
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Save.json";

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
        string savePath = currentPath;

        Debug.Log("Saving at " + savePath);
        SaveFormat sf = new SaveFormat();
        sf.SetUp(saver);
        string json = JsonUtility.ToJson(sf);

        using StreamWriter streamWriter = new StreamWriter(savePath);
        streamWriter.Write(json);
        streamWriter.Close();
    }

    public void LoadFromFile()
    {
        string loadPath = currentPath;

        using StreamReader streamReader = new StreamReader(loadPath);
        string json = streamReader.ReadToEnd();
        streamReader.Close();

        SaveFormat data = JsonUtility.FromJson<SaveFormat>(json);
        data.Update();
        Debug.Log("Data Loaded");
    }
}
