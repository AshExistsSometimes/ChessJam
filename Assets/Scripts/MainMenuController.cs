using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public CanvasGroup titleScreenGroup;
    public RectTransform mainMenuPanel;
    public RectTransform levelSelectPanel;
    public RectTransform controlsPanel;

    [Header("Level Select ScrollView")]
    public GameObject levelButtonPrefab;
    public Transform levelListContent;

    [Header("Animation Settings")]
    public float fadeDuration = 1f;
    public float slideDuration = 0.5f;

    [Header("Target Positions")]
    public Vector2 mainMenuTargetPosition = new Vector2(200f, 0f);
    public Vector2 levelSelectTargetPosition = new Vector2(0f, 0f);

    [System.Serializable]
    public struct LevelData
    {
        public string sceneName;
        public string displayName;
    }

    [Header("Level Data")]
    public LevelData[] levels;

    private bool titleFinished = false;

    void Start()
    {
        titleScreenGroup.alpha = 1f;
        titleScreenGroup.blocksRaycasts = true;
        mainMenuPanel.gameObject.SetActive(false);
        levelSelectPanel.gameObject.SetActive(false);

        InitializeSaveData();
    }

    void Update()
    {
        if (!titleFinished && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(FadeOutTitleAndShowMainMenu());
        }

        if (levelSelectPanel.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(SwitchToMainMenu());
        }


        if (Input.GetKeyDown(KeyCode.BackQuote)) // BackQuote is the `~` key
        {
            Debug.Log($"Save file path: {SaveSystem.GetSaveFilePath()}");
        }
    }


    void InitializeSaveData()
    {
        if (!SaveSystem.SaveExists())
        {
            SaveData data = new SaveData();

            if (levels != null && levels.Length > 0)
            {
                data.lastLevelName = levels[0].sceneName; // first level from array
            }
            else
            {
                data.lastLevelName = "Level1"; // fallback default
            }

            SaveSystem.Save(data);
            Debug.Log($"Save initialized with first level: {data.lastLevelName}");
        }
    }

    IEnumerator FadeOutTitleAndShowMainMenu()
    {
        titleFinished = true;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            float t = timer / fadeDuration;
            titleScreenGroup.alpha = Mathf.Lerp(1f, 0f, t);
            timer += Time.deltaTime;
            yield return null;
        }

        titleScreenGroup.alpha = 0f;
        titleScreenGroup.blocksRaycasts = false;
        titleScreenGroup.gameObject.SetActive(false);

        mainMenuPanel.gameObject.SetActive(true);
        OnMainMenuLoad();
        yield return StartCoroutine(SlideInPanel(mainMenuPanel));
    }


    // BUTTONS -----------------------------
    public void OnContinuePressed()
    {
        SaveData data = SaveSystem.Load();

        if (data != null && !string.IsNullOrEmpty(data.lastLevelName))
        {
            SceneManager.LoadScene(data.lastLevelName);
        }
        else
        {
            int nextSceneIndex = Mathf.Min(SceneManager.sceneCountInBuildSettings - 1, SceneManager.GetActiveScene().buildIndex + 1);
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    public void OnLoadLevelPressed()
    {
        StartCoroutine(SwitchToLevelSelect());
    }

    public void OnQuitGamePressed()
    {
        Application.Quit();
    }

    public void OnControlsPressed()
    {
        StartCoroutine(SwitchToControls());
    }

    // --------------------------------------



    public void OnBackFromLevelSelect()
    {
        StartCoroutine(SwitchToMainMenu());
    }

    IEnumerator SwitchToLevelSelect()
    {
        yield return StartCoroutine(SlideOutPanel(mainMenuPanel));
        levelSelectPanel.gameObject.SetActive(true);
        mainMenuPanel.gameObject.SetActive(false);

        PopulateLevelButtons();
        yield return StartCoroutine(SlideInPanel(levelSelectPanel));
    }

    IEnumerator SwitchToMainMenu()
    {
        yield return StartCoroutine(SlideOutPanel(levelSelectPanel));
        levelSelectPanel.gameObject.SetActive(false);

        mainMenuPanel.gameObject.SetActive(true);
        yield return StartCoroutine(SlideInPanel(mainMenuPanel));
    }

    IEnumerator SlideInPanel(RectTransform panel)
    {
        Vector2 start = new Vector2(-Screen.width, 0);
        Vector2 end = GetTargetPosition(panel);
        float time = 0f;
        panel.anchoredPosition = start;

        while (time < slideDuration)
        {
            panel.anchoredPosition = Vector2.Lerp(start, end, time / slideDuration);
            time += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = end;
    }

    IEnumerator SlideOutPanel(RectTransform panel)
    {
        Vector2 start = panel.anchoredPosition;
        Vector2 end = new Vector2(-Screen.width, 0);
        float time = 0f;

        while (time < slideDuration)
        {
            panel.anchoredPosition = Vector2.Lerp(start, end, time / slideDuration);
            time += Time.deltaTime;
            yield return null;
        }

        panel.anchoredPosition = end;
    }

    Vector2 GetTargetPosition(RectTransform panel)
    {
        if (panel == mainMenuPanel)
            return mainMenuTargetPosition;
        if (panel == levelSelectPanel)
            return levelSelectTargetPosition;
        return Vector2.zero;
    }

    void PopulateLevelButtons()
    {
        foreach (Transform child in levelListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (LevelData level in levels)
        {
            GameObject button = Instantiate(levelButtonPrefab, levelListContent);

            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = level.displayName;
            else
                Debug.LogWarning("No TextMeshProUGUI component found on level button prefab.");

            string sceneToLoad = level.sceneName;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log($"Loading scene: {sceneToLoad}");
                SceneManager.LoadScene(sceneToLoad);
            });
        }
    }

    IEnumerator SwitchToControls()
    {
        yield return StartCoroutine(SlideOutPanel(mainMenuPanel));
        mainMenuPanel.gameObject.SetActive(false);

        controlsPanel.gameObject.SetActive(true);
        yield return StartCoroutine(SlideInPanel(controlsPanel));
    }

    public void OnBackFromControls()
    {
        StartCoroutine(SwitchBackFromControls());
    }

    IEnumerator SwitchBackFromControls()
    {
        yield return StartCoroutine(SlideOutPanel(controlsPanel));
        controlsPanel.gameObject.SetActive(false);

        mainMenuPanel.gameObject.SetActive(true);
        yield return StartCoroutine(SlideInPanel(mainMenuPanel));
    }

    private void OnMainMenuLoad()
    {
        DestroyIfExists<PauseMenuController>();
        DestroyIfExists<GameManager>();
        DestroyIfExists<SoundManager>();
    }

    private void DestroyIfExists<T>() where T : MonoBehaviour
    {
        T instance = FindObjectOfType<T>();
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
    }
}

