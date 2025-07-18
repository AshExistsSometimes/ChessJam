using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    [Header("Singleton Prefabs")]
    public GameObject gameManagerPrefab;
    public GameObject soundManagerPrefab;

    [Header("Scene Specific Prefabs")]
    public GameObject pauseMenuPrefab;         
    public GameObject playerPrefab;
    public GameObject cameraControllerPrefab;   

    private void Awake()
    {
        // Ensure GameManager singleton exists
        if (GameManager.Instance == null)
        {
            Instantiate(gameManagerPrefab);
        }

        // Ensure SoundManager singleton exists
        if (SoundManager.Instance == null)
        {
            Instantiate(soundManagerPrefab);
        }

        string currentScene = SceneManager.GetActiveScene().name;

        // Don't instantiate scene-specific prefabs in MainMenu
        if (currentScene != "Main_Menu")
        {
            // Instantiate PauseMenu
            if (FindObjectOfType<PauseMenuController>() == null)
            {
                Instantiate(pauseMenuPrefab);
            }

            // Instantiate Player
            if (FindObjectOfType<PlayerMovement>() == null)
            {
                Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            }

            // Instantiate CameraController
            if (FindObjectOfType<CameraController>() == null)
            {
                Instantiate(cameraControllerPrefab, Vector3.zero, Quaternion.identity);
            }
        }
    }
}
