using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using static Tool;

public class LudoPiece : MonoBehaviour
{

    public Team myTeam { get; private set; }

    [field: SerializeField]
    public Space homeSpace { get; private set; }
    public Space startSpace { get; private set; }

    public Space CurrentSpace { get; private set; }
    public int killCount;
    public bool IsClickable { get; set; }
    public bool IsMoving { get; set; } = false;

    Rigidbody rb;
    Renderer rend;
    Color originalColor;

    void Awake()
    {
        myTeam = GetComponentInParent<Team>();
        rb = GetComponent<Rigidbody>();
        rend = gameObject.GetComponent<Renderer>();
        startSpace = myTeam.StartSpace;
        originalColor = rend.material.color;
    }

    void Start()
    {
        ResetToHome();
    }

    void Update()
    {
        if (IsClickable) OnClick();

        // detect current space
        CurrentSpace = CheckCurrentSpace();

        // ensure this piece is on the space
        if (CurrentSpace == null) ResetToHome();
    }
    public void ResetToHome()
    {
        transform.position = homeSpace.ActualPosition;
        transform.rotation = LudoPieceManager.PieceRotation;
        if (!rb.isKinematic) rb.velocity = Vector3.zero;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!IsMoving) return;

        LudoPiece collisionPiece = collision.gameObject.GetComponent<LudoPiece>();

        if (collisionPiece != null && !myTeam.GetAllPieces().Contains(collisionPiece))
        {
            collisionPiece.ResetToHome();
            AudioManager.Instance.PlaySFX("Kick");
            killCount++;
        }
    }

    public Space CheckCurrentSpace()
    {
        return Physics.Raycast(transform.position, Vector3.down, out var hit)
               && hit.collider.TryGetComponent(out Space space)
            ? space
            : CurrentSpace;
    }

    public int GetDistanceToTheEnd()
    {
        int count = 0;
        for (Space space = CurrentSpace; space != null; count++)
        {
            if (!space.UseNextSpace) return count;

            space = (space.NextSpace == startSpace && space.UseNextSpace2)
                ? space.NextSpace2
                : space.NextSpace;
        }
        return count;
    }

    #region  OnMouse
    public void OnClick()
    {
        if (Input.GetMouseButtonDown(0)) // 檢測左鍵點擊
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                myTeam.ResetClickableState();
                GameManager.Instance.ChosenPiece = this;
                StartCoroutine(LudoPieceManager.Instance.AIMovePiece(this, DiceManager.Instance.GetCurrentDiceResult()));
            }
        }
    }
    void OnMouseEnter()
    {
        if (IsClickable)
        {
            rend.material.color = Color.black;
        }
    }
    void OnMouseExit()
    {
        rend.material.color = originalColor;
    }
    #endregion  OnMouse
}