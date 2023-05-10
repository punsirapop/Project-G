using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Continue : MonoBehaviour
{
    Button _Continue;
    // json save path
    string path = "";
    string persistentPath = "";
    protected string currentPath;

    // Start is called before the first frame update
    void Start()
    {
        _Continue = GetComponent<Button>();

        path = Path.Combine(Application.dataPath, "Save", "Save.json");
        persistentPath = Path.Combine(Application.persistentDataPath, "Save", "Save.json");

        currentPath = persistentPath;

        _Continue.interactable = File.Exists(path);
    }
}
