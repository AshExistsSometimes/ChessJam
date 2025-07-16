using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public Collider overlapDetection;
    public Collider groundDetection;
    public Collider inputDetectCollider;

    [SerializeField] private LayerMask obstacleMask;

    private SpriteRenderer spriteRenderer;

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


        bool hasGround = Physics.CheckBox(groundDetection.bounds.center, groundDetection.bounds.extents, Quaternion.identity, ~LayerMask.GetMask("IgnoreRaycast"));

        SetActive(!isBlocked && hasGround);
    }

    private void SetActive(bool state)
    {
        spriteRenderer.enabled = state;
        inputDetectCollider.enabled = state;
    }

    private void OnMouseDown()
    {
        if (!GameManager.Instance.PlayersTurn) return;

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            player.MoveTo(transform.position);
    }
}
