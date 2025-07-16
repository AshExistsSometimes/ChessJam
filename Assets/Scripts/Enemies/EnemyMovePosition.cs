using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovePosition : MonoBehaviour
{
    public bool StandardMovement = true;
    public bool OffensiveMovement = true;

    // Checks if player is currently overlapping this move position
    public bool IsPlayerOnPosition()
    {
        // Assuming the MoveButton has a trigger collider attached for detection
        Collider[] hits = Physics.OverlapBox(transform.position, Vector3.one * 0.4f, Quaternion.identity, LayerMask.GetMask("Player"));
        return hits.Length > 0;
    }
}

