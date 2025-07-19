using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public Collider overlapDetection;
    public Collider groundDetection;
    public Collider inputDetectCollider;

    [SerializeField] private LayerMask obstacleMask;

    [Header("Colors")]
    [SerializeField] private Color safeColor = Color.white;
    [SerializeField] private Color dangerColor = Color.red;
    [SerializeField] private Color highlightSafeColor = new Color(1f, 1f, 1f, 1f); // Brighter white
    [SerializeField] private Color highlightDangerColor = new Color(1f, 0.5f, 0.5f, 1f); // Brighter red

    private SpriteRenderer spriteRenderer;
    private bool isDangerous;
    private bool isHighlighted;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (!overlapDetection || !groundDetection || !inputDetectCollider)
            SetActive(false); // Initially hidden
    }

    private void Update()
    {
        if (!GameManager.Instance.PlayersTurn)
        {
            SetActive(false);
            return;
        }

        ValidatePosition();
    }

    private void ValidatePosition()
    {
        bool isBlocked = Physics.CheckBox(
            overlapDetection.bounds.center,
            overlapDetection.bounds.extents,
            Quaternion.identity,
            obstacleMask
        );

        bool hasGround = Physics.CheckBox(
            groundDetection.bounds.center,
            groundDetection.bounds.extents,
            Quaternion.identity,
            ~LayerMask.GetMask("IgnoreRaycast")
        );

        // Check for enemy offensive positions
        isDangerous = false;
        Collider[] hits = Physics.OverlapBox(
            overlapDetection.bounds.center,
            overlapDetection.bounds.extents,
            Quaternion.identity
        );

        foreach (var hit in hits)
        {
            EnemyMovePosition enemyPos = hit.GetComponent<EnemyMovePosition>();
            if (enemyPos != null && enemyPos.OffensiveMovement)
            {
                isDangerous = true;
                break;
            }
        }

        if (!isHighlighted)
        {
            spriteRenderer.color = isDangerous ? dangerColor : safeColor;
        }

        SetActive(!isBlocked && hasGround);
    }

    private void SetActive(bool state)
    {
        spriteRenderer.enabled = state;
        inputDetectCollider.enabled = state;
    }

    private void OnMouseEnter()
    {
        if (!spriteRenderer.enabled) return;

        isHighlighted = true;
        spriteRenderer.color = isDangerous ? highlightDangerColor : highlightSafeColor;
    }

    private void OnMouseExit()
    {
        if (!spriteRenderer.enabled) return;

        isHighlighted = false;
        spriteRenderer.color = isDangerous ? dangerColor : safeColor;
    }

    private void OnMouseDown()
    {
        if (!GameManager.Instance.PlayersTurn) return;

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            player.MoveTo(transform.position);

        isHighlighted = false;
    }

}

