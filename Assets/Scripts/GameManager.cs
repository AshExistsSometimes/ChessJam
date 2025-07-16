using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Turn State")]
    public bool PlayersTurn = true;
    //public List<EnemyMovement> ActiveEnemies = new List<EnemyMovement>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional for scene persistence
    }

    public void EndPlayerTurn()
    {
        PlayersTurn = false;
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        // Let enemies react after a short delay
        yield return new WaitForSeconds(0.1f);

        //foreach (EnemyMovement enemy in ActiveEnemies)
        //{
        //    if (enemy != null)
        //        yield return enemy.TakeTurn();
        //}

        //// Clear the list for next turn
        //ActiveEnemies.Clear();

        //// Small delay before returning control to player
        //yield return new WaitForSeconds(0.1f);

        PlayersTurn = true;
    }

    //public void RegisterActiveEnemy(EnemyMovement enemy)
    //{
    //    if (!ActiveEnemies.Contains(enemy))
    //        ActiveEnemies.Add(enemy);
    //}
}