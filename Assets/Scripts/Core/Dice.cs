using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static GameConstants;

public class Dice : MonoBehaviour
{

    public Vector3 rotatingPos = new Vector3(0, 7f, 25f);
    public Vector3 rotatingSpeed = new Vector3(5f, 5f, 5f);
    public float speed = 10f;


    public int dice_result { get; private set; }
    public Rigidbody rb { get; private set; }


    public bool isCheckingForSettle { get; private set; }
    public bool hasSettled { get; private set; }
    public bool isRollFinished { get; private set; }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rotatingPos = DiceRotatingPos;
    }

    void Update()
    {
        if (gameObject.transform.position.y < -10f)
        {
            ResetPosition();
        }

        if (!isCheckingForSettle)
        {
            StartCoroutine(CheckForSettle());
        }
    }

    // is dice moving or not
    private bool IsDiceSettled()
    {
        return rb.velocity.magnitude < DiceManager.VelocityThreshold &&
               rb.angularVelocity.magnitude < DiceManager.AngularVelocityThreshold;
    }

    private IEnumerator CheckForSettle()
    {
        isCheckingForSettle = true;
        float elapsedTime = 0f;

        while (elapsedTime < DiceManager.SettleTime)
        {
            if (!IsDiceSettled())
            {
                isCheckingForSettle = false;
                hasSettled = false;
                yield break;
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        hasSettled = true;
        isCheckingForSettle = false;

        UpdateDiceResult();
        isRollFinished = true;
    }

    private void UpdateDiceResult()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit))
        {
            dice_result = GetDiceResult(hit.collider.gameObject.name);
        }
    }

    private int GetDiceResult(string result)
    {
        switch (result)
        {
            case "surface1": return 1;
            case "surface2": return 2;
            case "surface3": return 3;
            case "surface4": return 4;
            case "surface5": return 5;
            case "surface6": return 6;
            default: return 0;
        }
    }
    #region  Reset
    public void ResetPosition()
    {
        gameObject.transform.position = rotatingPos;
    }
    #endregion Reset

    #region Get
    public bool isStop()
    {
        return hasSettled;
    }
    public int GetLatestDiceResult()
    {
        if (hasSettled)
        {
            UpdateDiceResult();
        }
        return dice_result;
    }
    #endregion Get

    #region  OnMouse
    private void OnMouseDrag()
    {
        List<LudoPiece> allColorPieces = Tool.TurnToTeam(GameManager.Instance.CurrentPlayerTurn).GetAllPieces();
        bool isAllUnclickable = allColorPieces.All(piece => !piece.IsClickable);
        // 若為人類玩家 且停止 且任何棋是不可被點擊的狀態
        if (GameManager.Instance.CurrentPlayerTurn <= GameManager.Instance.HumanPlayers && isRollFinished && isAllUnclickable)
        {
            rb.useGravity = false;
            transform.position = rotatingPos;
            transform.Rotate(rotatingSpeed);
            GameManager.Instance.IsDiceThrown = true;
            GameManager.Instance.IsPieceMoved = false;
            hasSettled = false;
        }
    }
    private void OnMouseUp()
    {
        rb.useGravity = true;
        isRollFinished = false;
    }
    #endregion  OnMouse
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.up);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // AudioManager.Instance.PlaySFX("DiceRoll");
    }


}