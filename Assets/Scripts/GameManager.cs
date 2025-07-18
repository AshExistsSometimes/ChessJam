using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool PlayersTurn = true;
    public List<EnemyAI> ActiveEnemies = new List<EnemyAI>();

    public PlayerMovement Player { get; private set; }

    private void Awake()
    {

        // INSTANTIATION
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Destroy if this is the Main Menu scene
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Destroy(gameObject);
        }


        Player = FindObjectOfType<PlayerMovement>();

        if (Player == null)
            Debug.LogError("PlayerMovement script not found in scene!");
    }

    private void Start()
    {
        StartCoroutine(GameLoop());

        GridOverlay.Instance?.RefreshGrid();
    }

    private void Update()
    {
        if (!PlayersTurn && ActiveEnemies.Count == 0)
        {
            PlayersTurn = true;
        }

        if (Player == null)
        {
            Player = FindObjectOfType<PlayerMovement>();
        }
    }

    public void EndPlayerTurn()
    {
        PlayersTurn = false;
        GridOverlay.Instance.RefreshGrid();
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            if (PlayersTurn)
            {
                // Wait for player to finish turn (signal elsewhere)
                yield return new WaitUntil(() => !PlayersTurn);
            }
            else
            {
                // Enemy turns
                foreach (EnemyAI enemy in ActiveEnemies)
                {
                    if (enemy != null)
                        yield return enemy.TakeTurn();
                }

                // After all enemies moved, switch back to player turn after a short delay
                yield return new WaitForSeconds(0.1f);
                PlayersTurn = true;
            }

            yield return null;
        }
    }

    
}