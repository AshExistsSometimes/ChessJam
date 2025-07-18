using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnemyMovePosition : MonoBehaviour
{
    public bool StandardMovement = true;
    public bool OffensiveMovement = true;

    [Tooltip("Objects on these layers will block this move position.")]
    public LayerMask blockingLayers;

    private EnemyAI parentAI;
    private BoxCollider boxCollider;
    private readonly HashSet<Collider> blockingObjects = new();

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        parentAI = GetComponentInParent<EnemyAI>();
        if (parentAI == null)
        {
            Debug.LogError($"EnemyMovePosition '{name}' has no EnemyAI on parent.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsBlockingLayer(other.gameObject.layer))
        {
            blockingObjects.Add(other);
            UpdateRegistration();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (blockingObjects.Remove(other))
        {
            UpdateRegistration();
        }
    }

    private bool IsBlockingLayer(int layer)
    {
        return (blockingLayers.value & (1 << layer)) != 0;
    }

    private void UpdateRegistration()
    {
        if (parentAI == null) return;

        if (blockingObjects.Count > 0)
        {
            parentAI.movePositions.Remove(this);
        }
        else
        {
            if (!parentAI.movePositions.Contains(this))
                parentAI.movePositions.Add(this);
        }
    }
    public bool IsPlayerOnPosition()
    {
        Collider[] hits = Physics.OverlapBox(
            transform.position,
            Vector3.one * 0.4f,
            Quaternion.identity,
            LayerMask.GetMask("Player")
        );

        return hits.Length > 0;
    }
}


