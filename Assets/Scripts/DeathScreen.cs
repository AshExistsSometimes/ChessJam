using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    public GameObject deathScreenUI;
    public Image fadePanel;
    public float fadeDuration = 1f;

    private PlayerMovement player;

    private void Awake()
    {
        // Try auto-assign
        if (deathScreenUI == null)
        {
            deathScreenUI = transform.Find("DeathScreenUI")?.gameObject;
        }

        if (fadePanel == null)
        {
            fadePanel = GetComponentInChildren<Image>();
        }

        if (deathScreenUI == null)
        {
            Debug.LogWarning("DeathScreenUI is missing or not assigned on DeathScreen.");
        }

        FindPlayer();
    }

    private void FindPlayer()
    {
        player = FindObjectOfType<PlayerMovement>();
        if (player == null)
            Debug.LogWarning("DeathScreen: Could not find PlayerMovement in scene.");
    }

    public void ShowDeathScreen()
    {
        deathScreenUI = transform.Find("DeathScreenUI")?.gameObject;

        if (deathScreenUI == null)
        {
            Debug.LogWarning("DeathScreenUI is missing. Cannot show death screen.");
            return;
        }

        deathScreenUI.SetActive(true);
        StartCoroutine(FadeToBlack());
    }

    public void HideDeathScreen()
    {
        if (deathScreenUI != null)
            deathScreenUI.SetActive(false);
    }

    IEnumerator FadeToBlack()
    {
        if (fadePanel == null)
            yield break;

        Color color = fadePanel.color;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }
    }

// Called by button in Inspector
public void RetryLevel()
    {
        player.InitializePlayer();
        HideDeathScreen();
        GameManager.Instance.ActiveEnemies.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Called by button in Inspector
    public void QuitToTitle()
    {
        GameManager.Instance.ActiveEnemies.Clear();
        HideDeathScreen();
        SceneManager.LoadScene("Main_Menu");
    }
}
