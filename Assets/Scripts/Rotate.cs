using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;
public class Rotate : MonoBehaviour
{
    public static Rigidbody rb;
    public static Vector3 rotatingPos = new Vector3(0, 7f, 0);
    public static Vector3 rotatingSpeed = new Vector3(2f, 2f, 2f);
    // public static bool isPressed = false;
    public static int dice_result;


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
    }
    public static int GetDiceResult(string result)
    {
        if (result == "surface1")
        {
            return 1;
        }
        else if (result == "surface2")
        {
            return 2;
        }
        else if (result == "surface3")
        {
            return 3;
        }
        else if (result == "surface4")
        {
            return 4;
        }
        else if (result == "surface5")
        {
            return 5;
        }
        else if (result == "surface6")
        {
            return 6;
        }
        else
        {
            rb.AddForce(Vector3.up * 100f, ForceMode.Impulse);
            return 0;
        }
    }
    public static (bool isStop, int dice_result) StopDetermination()
    {
        //骰子落下靜止後
        if (Mathf.Abs(rb.velocity.magnitude) < .01f && dice.transform.position.y < 2f)
        {

            RaycastHit hit;
            Physics.Raycast(dice.transform.position, Vector3.up, out hit);
            //得到骰子結果
            dice_result = GetDiceResult(hit.collider.gameObject.name);

            //可再繼續擲骰
            // isPressed = false;
            return (true, dice_result);
        }
        else return (false, 0);
    }
    void OnMouseDrag()
    {
        // if (!isPressed)
        // {
        transform.position = rotatingPos;
        transform.Rotate(rotatingSpeed);
        isDiceThrown = true;
        isChessMoved = false;
        // }
    }
}
