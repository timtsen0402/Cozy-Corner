using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dice : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 rotatingPos = new Vector3(-24f, 7f, 0);
    public Vector3 rotatingSpeed = new Vector3(2f, 3f, 2f);
    public int dice_result;

    float velocityThreshold = 0.01f;
    float angularVelocityThreshold = 0.01f;
    float settleTime = 0.5f;
    bool isCheckingForSettle = false;
    bool hasSettled = false;
    bool isRollFinished = false;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rotatingPos = new Vector3(-24f, 7f, 0);
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

    public void ResetPosition()
    {
        gameObject.transform.position = rotatingPos;
    }

    // is dice moving or not
    private bool IsDiceSettled()
    {
        return rb.velocity.magnitude < velocityThreshold &&
               rb.angularVelocity.magnitude < angularVelocityThreshold;
    }

    private IEnumerator CheckForSettle()
    {
        isCheckingForSettle = true;
        float elapsedTime = 0f;

        while (elapsedTime < settleTime)
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
        // Debug.Log("骰子已停止運動！");

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

    public int GetDiceResult(string result)
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
    void OnMouseDrag()
    {
        List<LudoPiece> allColorPieces = Tool.TurnToTeam(GameManager.Instance.CurrentPlayerTurn);
        bool isAllUnclickable = allColorPieces.All(piece => !piece.IsClickable);
        // 若為人類玩家 且停止 且任何棋是不可被點擊的狀態
        if (GameManager.Instance.CurrentPlayerTurn <= GameManager.Instance.HumanPlayers && isRollFinished && isAllUnclickable)
        {
            transform.position = rotatingPos;
            transform.Rotate(rotatingSpeed);
            GameManager.Instance.IsDiceThrown = true;
            GameManager.Instance.IsPieceMoved = false;
            hasSettled = false;
        }
    }
    void OnMouseUp()
    {
        isRollFinished = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.up);
    }
    void OnCollisionEnter(Collision collision)
    {
        // AudioManager.Instance.PlaySFX("DiceRoll");
    }


}