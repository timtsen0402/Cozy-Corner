using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using static GameController;
using static Rotate;
using static Tool;
using static ToolSingleton;

public class Chess : MonoBehaviour
{
    public enum PieceColor
    {
        Blue,
        Red,
        Green,
        Yellow
    }
    [SerializeField]
    private PieceColor color;

    public bool isClickable = false;
    public GameObject home_space;
    public GameObject start_space;
    public GameObject currentPosition;
    Renderer rend;
    Color originalColor;

    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    void Update()
    {
        if (isClickable)
            OnClick();
        currentPosition = CheckSelfPos();

        // not in appropriate position
        if (!new int[] { 6, 7, 8, 9 }.Contains(currentPosition.layer))
        {
            transform.position = home_space.GetComponent<Space>().actual_position;
            transform.rotation = Quaternion.Euler(new Vector3(270f, 0f, 0f));
        }
    }

    public GameObject CheckSelfPos()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }
    public void OnClick()
    {
        if (Input.GetMouseButtonDown(0)) // 檢測左鍵點擊
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                print("move");
                StartCoroutine(MoveChess());
                selectedChess = gameObject;
            }
        }
    }
    public IEnumerator MoveChess()
    {
        int steps = StopDetermination().dice_result;

        for (int i = 0; i < steps; i++)
        {
            Space currentSpace = CheckSelfPos().GetComponent<Space>();

            if (currentSpace.next_space == null)
            {
                break;
            }

            Vector3 nextPosition;

            if (currentSpace.next_space == start_space)
            {
                if (currentSpace.gameObject.layer == 6) // Home
                {
                    nextPosition = start_space.GetComponent<Space>().actual_position;
                    yield return StartCoroutine(Instance.MoveToPosition(gameObject, nextPosition));
                    break;
                }
                //終點前
                else
                {
                    nextPosition = currentSpace.next_space2.GetComponent<Space>().actual_position;
                }
            }
            else
            {
                nextPosition = currentSpace.next_space.GetComponent<Space>().actual_position;
            }

            yield return StartCoroutine(Instance.MoveToPosition(gameObject, nextPosition));

            // 短暂暂停，让玩家能看清每一步的移动
            yield return new WaitForSeconds(0.1f);
        }

        isChessMoved = true;

    }


    void OnMouseEnter()
    {
        if (isClickable)
        {
            rend.material.color = Color.black;
        }
    }
    void OnMouseExit()
    {
        rend.material.color = originalColor;
    }
}
//CheckSelfPos()跟currentPosition不一樣