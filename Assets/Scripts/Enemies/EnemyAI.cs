using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public List<EnemyMovePosition> movePositions;
    public float moveSpeed = 3f;
    public float liftHeight = 1f;

    private Transform playerTransform;

    private bool playerDetected = false;

    private void Start()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("PlayerMovement not found in scene.");
    }

    public IEnumerator TakeTurn()
    {
        EnemyMovePosition targetPos = ChooseMovePosition();

        if (targetPos != null)
        {
            yield return StartCoroutine(MoveToPosition(targetPos.transform.position));
        }
        else
        {
            Debug.Log($"{gameObject.name} has no valid move positions.");
        }
    }

    private EnemyMovePosition ChooseMovePosition()
    {
        Vector3 playerPos = playerTransform.position;
        EnemyMovePosition offensiveTileWithPlayer = null;

        // First, find if player is standing exactly on any tile
        foreach (var pos in movePositions)
        {
            // Assume tiles are aligned on a grid, so just check if playerPos is approximately the same as tile position (within a small threshold)
            if (Vector3.Distance(pos.transform.position, playerPos) < 0.1f)
            {
                if (pos.OffensiveMovement)
                {
                    // Player is on an offensive tile - enemy must move there
                    offensiveTileWithPlayer = pos;
                    break;
                }
                else if (pos.StandardMovement && !pos.OffensiveMovement)
                {
                    // Player is on standard but not offensive tile - enemy should NOT move here
                    // We'll handle by excluding this tile later
                    // So just note we found player here
                }
            }
        }

        if (offensiveTileWithPlayer != null)
        {
            Debug.Log($"Player on offensive tile {offensiveTileWithPlayer.name}, enemy will move there.");
            return offensiveTileWithPlayer;
        }

        // Otherwise, pick closest standard tile, excluding the tile player is on if it is NOT offensive
        EnemyMovePosition closestPos = null;
        float closestDistSqr = Mathf.Infinity;

        foreach (var pos in movePositions)
        {
            if (!pos.StandardMovement)
                continue;

            // Exclude tile player is standing on if it is NOT offensive
            if (Vector3.Distance(pos.transform.position, playerPos) < 0.1f && !pos.OffensiveMovement)
            {
                Debug.Log($"Skipping tile {pos.name} because player is on it and it is not offensive.");
                continue;
            }

            float distSqr = (pos.transform.position - playerPos).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closestPos = pos;
            }
        }

        if (closestPos != null)
            Debug.Log($"Enemy moving to closest standard tile {closestPos.name}");
        else
            Debug.LogWarning("No valid standard tile found for enemy to move.");

        return closestPos;
    }

    public void OnPlayerDetected()
    {
        if (!playerDetected)
        {
            playerDetected = true;
            if (!GameManager.Instance.ActiveEnemies.Contains(this))
            {
                GameManager.Instance.ActiveEnemies.Add(this);
                Debug.Log($"{gameObject.name} added to ActiveEnemies");
            }
        }
    }

    public void OnPlayerLost()
    {
        if (playerDetected)
        {
            playerDetected = false;
            if (GameManager.Instance.ActiveEnemies.Contains(this))
            {
                GameManager.Instance.ActiveEnemies.Remove(this);
                Debug.Log($"{gameObject.name} removed from ActiveEnemies");
            }
        }
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        Vector3 startPos = transform.position;
        Vector3 liftedStart = startPos + Vector3.up * liftHeight;
        Vector3 liftedTarget = target + Vector3.up * liftHeight;

        float t = 0f;

        // Lift up
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, liftedStart, t);
            yield return null;
        }

        t = 0f;

        // Move horizontally
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(liftedStart, liftedTarget, t);
            yield return null;
        }

        t = 0f;

        // Lower down
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(liftedTarget, target, t);
            yield return null;
        }

        // Snap to grid
        transform.position = new Vector3(
            Mathf.Round(target.x),
            Mathf.Round(target.y),
            Mathf.Round(target.z)
        );

        CheckForPlayerOverlap();
    }

    private void CheckForPlayerOverlap()
    {
        Vector3 enemyPos = transform.position;
        Vector3 playerPos = GameManager.Instance.Player.transform.position;

        if (Vector3.Distance(enemyPos, playerPos) < 0.1f)
        {
            Debug.Log("Enemy overlapped player on enemy turn — player dies!");
            GameManager.Instance.Player.playerModel.SetActive(false);
            GameManager.Instance.Player.HandleDeath();
        }
    }
}
