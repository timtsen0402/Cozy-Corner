using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using static Tool;

public class LudoPiece : MonoBehaviour
{
    public enum PieceColor
    {
        Orange,
        Green,
        Blue,
        Red,
    }
    [field: SerializeField]
    public PieceColor Color { get; private set; }
    [field: SerializeField]
    public Space HomeSpace { get; private set; }
    [field: SerializeField]
    public Space StartSpace { get; private set; }
    public Space CurrentSpace { get; private set; }
    public bool IsClickable { get; set; }
    public bool IsMoving { get; set; }

    Rigidbody rb;
    Renderer rend;
    Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        IsMoving = false;
        rend = gameObject.GetComponent<Renderer>();
        originalColor = rend.material.color;
        transform.position = HomeSpace.ActualPosition;
        transform.rotation = Quaternion.Euler(new Vector3(270f, 0f, 0f));
    }

    void Update()
    {
        CurrentSpace = CheckCurrentSpace();
        if (CurrentSpace == null) ResetToHome();
        if (IsClickable) OnClick();



        RigidbodySetting();
    }
    public void ResetToHome()
    {
        transform.position = HomeSpace.ActualPosition;
        transform.rotation = Quaternion.Euler(new Vector3(270f, 0f, 0f));
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
            // 給被撞物體一個力
            Rigidbody targetRb = collision.gameObject.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                Vector3 direction = (collision.transform.position - transform.position).normalized;

                // 對被碰撞物體施加力
                targetRb.AddForce(direction * 5f, ForceMode.Impulse);
                AudioManager.Instance.PlaySFX("Kick");
            }
        }
    }

    public Space CheckCurrentSpace()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            return hit.collider.gameObject.GetComponent<Space>();
        }
        else
        {
            return CurrentSpace;
        }
    }
    public void OnClick()
    {
        if (Input.GetMouseButtonDown(0)) // 檢測左鍵點擊
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                StartCoroutine(MovePiece());
                // selectedPiece = gameObject;
            }
        }
    }
    private IEnumerator MovePiece()
    {
        int steps = DiceManager.Instance.GetTotalDiceResult();

        for (int i = 0; i < steps; i++)
        {
            Space currentSpace = CheckCurrentSpace();

            if (currentSpace.NextSpace == null)
            {
                break;
            }

            Vector3 nextPosition;

            if (currentSpace.NextSpace == StartSpace)
            {
                if (currentSpace.gameObject.layer == 6) // Home
                {
                    nextPosition = StartSpace.ActualPosition;
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

    }


    void OnMouseEnter()
    {
        if (IsClickable)
        {
            rend.material.color = UnityEngine.Color.black;
        }
    }
    void OnMouseExit()
    {
        rend.material.color = originalColor;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.black;
        Gizmos.DrawRay(transform.position, Vector3.down);
    }
}