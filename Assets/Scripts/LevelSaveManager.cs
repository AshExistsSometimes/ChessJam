using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSaveManager : MonoBehaviour
{
    private SaveData currentSaveData;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadSaveData();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSaveData.lastLevelName = scene.name;
        SaveSystem.Save(currentSaveData);
    }

    private void LoadSaveData()
    {
        SaveData data = SaveSystem.Load();
        if (data != null)
        {
            currentSaveData = data;
        }
        else
        {
            currentSaveData = new SaveData(); // start fresh
        }
    }
}