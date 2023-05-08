using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallOfCapybaraEntrance : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(!PlayerManager.CapybaraDatabase.IsFirstSpawn);
    }
}
