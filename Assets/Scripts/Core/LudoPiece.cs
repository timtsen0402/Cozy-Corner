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
    public int killCount { get; private set; }
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
    }

    void Start()
    {

        IsMoving = false;

        originalColor = rend.material.color;
        ResetToHome();


    }

    void Update()
    {
        // ParticleEffectManager.Instance.PlayEffect("fire", transform.position);

        CurrentSpace = CheckCurrentSpace();
        //if (CurrentSpace == null) ResetToHome();
        if (IsClickable) OnClick();
        RigidbodySetting();
    }
    public void ResetToHome()
    {
        transform.position = homeSpace.ActualPosition;
        transform.rotation = LudoPieceManager.PieceRotation;
        if (!rb.isKinematic) rb.velocity = Vector3.zero;
    }
    void RigidbodySetting()
    {
        if (IsMoving)
        {
            // 移動時,讓物體有質量和不受重力影響
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        else
        {
            // 靜止時,讓物體受重力影響
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (IsMoving)
        {
            LudoPiece collisionPiece = collision.transform.gameObject.GetComponent<LudoPiece>();
            if (collisionPiece != null && !myTeam.GetAllPieces().Contains(collisionPiece))
            {
                LudoPiece piece = collision.transform.gameObject.GetComponent<LudoPiece>();
                piece.ResetToHome();
                AudioManager.Instance.PlaySFX("Kick");
                killCount++;
                //ParticleEffectManager.Instance.PlayEffect("fire", collision.transform.position);
            }
        }
    }

    public Space CheckCurrentSpace()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.collider.gameObject.GetComponent<Space>() == null) return CurrentSpace;
            return hit.collider.gameObject.GetComponent<Space>();
        }
        else
        {
            return CurrentSpace;
        }
    }
    public int GetDistanceToTheEnd()
    {
        Space currentSpace = CurrentSpace;
        int count = 0;
        while (currentSpace != null)
        {
            if (!currentSpace.UseNextSpace) //end
            {
                return count;
            }

            count++;

            // 檢查是否需要轉換到 next_space2
            if (currentSpace.NextSpace == startSpace && currentSpace.UseNextSpace2)
            {
                currentSpace = currentSpace.NextSpace2;
            }
            else
            {
                currentSpace = currentSpace.NextSpace;
            }
        }
        return count;
    }

    private IEnumerator MovePiece()
    {
        foreach (LudoPiece piece in myTeam.GetAllPieces())
        {
            piece.IsClickable = false;
        }
        IsMoving = true;
        int steps = DiceManager.Instance.GetCurrentDiceResult();

        for (int i = 0; i < steps; i++)
        {
            Space currentSpace = CheckCurrentSpace();

            if (currentSpace.NextSpace == null)
            {
                break;
            }

            Vector3 nextPosition;

            if (currentSpace.NextSpace == startSpace)
            {
                if (currentSpace.gameObject.layer == 6) // Home
                {
                    nextPosition = startSpace.ActualPosition;
                    yield return StartCoroutine(LudoPieceManager.Instance.MoveToPosition(this, nextPosition));
                    break;
                }
                //終點前
                else
                {
                    nextPosition = currentSpace.NextSpace2.ActualPosition;
                }
            }
            else
            {
                nextPosition = currentSpace.NextSpace.ActualPosition;
            }

            yield return StartCoroutine(LudoPieceManager.Instance.MoveToPosition(this, nextPosition));

            // 短暂暂停，让玩家能看清每一步的移动
            yield return new WaitForSeconds(0.1f);
        }

        GameManager.Instance.IsPieceMoved = true;
        IsMoving = false;
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
                StartCoroutine(MovePiece());
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
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.black;
        Gizmos.DrawRay(transform.position, Vector3.down);
    }
}