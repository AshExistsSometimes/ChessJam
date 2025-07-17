using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public GameObject[] moveButtons;
    public float moveSpeed = 5f;
    public float liftHeight = 0.5f;

    [Header("Death Settings")]
    public GameObject playerModel;

    private bool isMoving = false;
    public bool isDead = false;

    [SerializeField] private LayerMask moveButtonLayer;

    private void Awake()
    {
        if (playerModel != null)
            playerModel.SetActive(true);
    }

    void Update()
    {
        if (isDead) return; // No input if dead

        // Only allow input if it's the player's turn and they're not already moving
        if (!GameManager.Instance.PlayersTurn || isMoving) return;

        foreach (var button in moveButtons)
            button.SetActive(true);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, moveButtonLayer))
            {
                MoveButton button = hit.collider.GetComponentInParent<MoveButton>();
                if (button != null)
                {
                    MoveTo(button.transform.position);
                }
            }
        }
    }

    public void MoveTo(Vector3 target)
    {
        if (isMoving) return;

        foreach (var button in moveButtons)
            button.SetActive(false);

        StartCoroutine(MoveRoutine(target));
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        isMoving = true;

        Vector3 startPos = transform.position;

        // Step 1: Lift
        Vector3 liftPos = new Vector3(startPos.x, startPos.y + liftHeight, startPos.z);
        yield return LerpPosition(startPos, liftPos, moveSpeed);

        // Step 2: Move horizontally to target.x/z (keeping lifted Y)
        Vector3 moveXZ = new Vector3(target.x, liftPos.y, target.z);
        yield return LerpPosition(liftPos, moveXZ, moveSpeed);

        // Step 3: Lower back down
        Vector3 lowered = new Vector3(target.x, startPos.y, target.z);
        yield return LerpPosition(moveXZ, lowered, moveSpeed);

        // Step 4: Snap to grid
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y,
            Mathf.Round(transform.position.z)
        );

        // Step 5: Check for enemy collision
        CheckForEnemyOverlap();

        // Step 6: End turn
        GameManager.Instance.EndPlayerTurn();
        isMoving = false;
    }

    private IEnumerator LerpPosition(Vector3 from, Vector3 to, float speed)
    {
        float elapsed = 0f;
        float duration = Vector3.Distance(from, to) / speed;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;
    }

    private void CheckForEnemyOverlap()
    {
        Vector3 playerPos = transform.position;

        foreach (EnemyAI enemy in GameManager.Instance.ActiveEnemies)
        {
            if (enemy == null) continue;

            if (Vector3.Distance(enemy.transform.position, playerPos) < 0.1f)
            {
                Debug.Log($"Killed {enemy}");
                Destroy(enemy.gameObject);

                // Optionally remove from active enemies list
                GameManager.Instance.ActiveEnemies.Remove(enemy);
                break;
            }
        }
    }


    public void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        if (playerModel != null)
            playerModel.SetActive(false);

        // Enable Death screen UI
    }
}
