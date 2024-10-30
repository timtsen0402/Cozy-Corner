using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static GameConstants;
using static Tool;

public abstract class Dice : MonoBehaviour
{
    public bool isCheckingForSettle { get; protected set; }
    public bool hasSettled { get; protected set; }
    public bool isRollFinished { get; protected set; }
    public int diceResult { get; protected set; }
    public Rigidbody rb { get; protected set; }

    private bool isManuallyRotating = false;

    protected virtual void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {
        ResetIfDrop();
        CheckIsSettled();
    }
    private void ResetIfDrop()
    {
        if (gameObject.transform.position.y < -10f)
        {
            transform.position = DiceRotatingPos;
        }
    }
    private void CheckIsSettled()
    {
        if (!isCheckingForSettle)
        {
            StartCoroutine(CheckForSettle());
        }
    }

    // is dice moving or not
    protected virtual bool IsDiceSettled()
    {
        if (isManuallyRotating) return false;
        return rb.velocity.magnitude < VelocityThreshold &&
               rb.angularVelocity.magnitude < AngularVelocityThreshold;
    }

    protected virtual IEnumerator CheckForSettle()
    {
        isCheckingForSettle = true;
        float elapsedTime = 0f;

        while (elapsedTime < SettleTime)
        {
            if (!IsDiceSettled())
            {
                isCheckingForSettle = false;
                hasSettled = false;
                yield break;
            }

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        hasSettled = true;
        isCheckingForSettle = false;
        isRollFinished = true;
    }

    private void UpdateDiceResult()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit))
        {
            diceResult = GetDiceResult(hit.collider.gameObject.name);
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
    public abstract void ResetPosition();
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
        return diceResult;
    }
    #endregion Get

    #region  OnMouse
    protected virtual void OnMouseDown()
    {
        DiceManager.Instance.RotatingSpeed = new Vector3
        (
        Random.Range(4f, 7f) * (Random.value < 0.5f ? 1 : -1),
        Random.Range(4f, 7f) * (Random.value < 0.5f ? 1 : -1),
        Random.Range(4f, 7f) * (Random.value < 0.5f ? 1 : -1)
        );
    }
    protected virtual void OnMouseDrag()
    {
        List<LudoPiece> allColorPieces = TurnToTeam(GameManager.Instance.CurrentPlayerTurn).GetAllPieces();
        bool isAllUnclickable = allColorPieces.All(piece => !piece.IsClickable);
        // able to roll the dice
        if (TurnToTeam(GameManager.Instance.CurrentPlayerTurn).CurrentState == TeamState.Player && isRollFinished && isAllUnclickable)
        {
            rb.useGravity = false;
            isManuallyRotating = true;
            transform.position = DiceRotatingPos;
            transform.Rotate(DiceManager.Instance.RotatingSpeed);

            hasSettled = false;
        }
    }
    protected virtual void OnMouseUp()
    {
        Vector3 velocity = rb.velocity;
        velocity.y = 0;
        rb.velocity = velocity;

        GameManager.Instance.IsDiceThrown = true;
        rb.useGravity = true;
        isRollFinished = false;
        isManuallyRotating = false;

    }
    #endregion  OnMouse
}