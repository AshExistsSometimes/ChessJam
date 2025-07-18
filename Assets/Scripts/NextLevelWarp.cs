using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelWarp : MonoBehaviour
{
    [Tooltip("If left empty, will load the next scene in Build Settings.")]
    public string sceneToLoad;

    [Tooltip("How long to wait before loading the next scene.")]
    public float waitTime = 1.5f;

    [Tooltip("Spin speed for the player model during the delay.")]
    public float spinSpeed = 360f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            StartCoroutine(LoadNextLevelCoroutine(other.transform.root.gameObject));
        }
    }

    private IEnumerator LoadNextLevelCoroutine(GameObject player)
    {
        // Attempt to spin player model if present
        Transform model = player.transform.Find("Model"); // adjust this path if yours differs

        yield return new WaitForSeconds(0.5f);

        float elapsed = 0f;
        while (elapsed < waitTime)
        {
            if (model != null)
            {
                model.Rotate(Vector3.right, spinSpeed * Time.deltaTime);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Load the next scene
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;

            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextIndex);
            }
            else
            {
                Debug.LogWarning("NextLevelWarp: No next scene in Build Settings.");
            }
        }
    }
}
