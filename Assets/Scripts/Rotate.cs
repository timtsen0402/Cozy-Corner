using System.Collections;
using UnityEngine;
using static GameController;

public class Rotate : MonoBehaviour
{
    public static Rigidbody rb;
    public static Vector3 rotatingPos = new Vector3(-20f, 7f, 0);
    public static Vector3 rotatingSpeed = new Vector3(2f, 3f, 2f);
    public static int dice_result;

    public float velocityThreshold = 0.01f;
    public float angularVelocityThreshold = 0.01f;
    public float settleTime = 0.5f;
    private bool isCheckingForSettle = false;
    private bool hasSettled = false;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (gameObject.transform.position.y < -10f)
        {
            gameObject.transform.position = rotatingPos;
        }

        if (!isCheckingForSettle)
        {
            StartCoroutine(CheckForSettle());
        }
    }

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
        Debug.Log("骰子已停止運動！");

        // 骰子停止後，更新結果
        UpdateDiceResult();
    }

    private void UpdateDiceResult()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit))
        {
            dice_result = GetDiceResult(hit.collider.gameObject.name);
        }
    }

    public static int GetDiceResult(string result)
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

    public static (bool isStop, int dice_result) StopDetermination()
    {
        Rotate rotateScript = FindObjectOfType<Rotate>();
        return (rotateScript.hasSettled, dice_result);
    }
    public static int GetLatestDiceResult()
    {
        Rotate rotateScript = FindObjectOfType<Rotate>();
        if (rotateScript != null && rotateScript.hasSettled)
        {
            rotateScript.UpdateDiceResult();
        }
        return dice_result;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.up);
    }

    void OnMouseDrag()
    {
        transform.position = rotatingPos;
        transform.Rotate(rotatingSpeed);
        isDiceThrown = true;
        isChessMoved = false;
        hasSettled = false;  // 重置停止狀態
    }
}