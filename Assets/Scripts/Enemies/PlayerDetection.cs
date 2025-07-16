using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    private EnemyAI enemyAI;

    private void Awake()
    {
        // Assume this script is on the DetectionRange collider, child of enemy
        enemyAI = GetComponentInParent<EnemyAI>();
        if (enemyAI == null)
        {
            Debug.LogError("PlayerDetection could not find EnemyAI in parent!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log($"{enemyAI.name}: Player entered detection range");
            enemyAI.OnPlayerDetected();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log($"{enemyAI.name}: Player left detection range");
            enemyAI.OnPlayerLost();
        }
    }
}
