using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    //格子之實際位置
    public Vector3 actual_position;
    //是否開啟下一個格子
    public bool useNext_space = true;
    public bool useNext_space2 = false;
    //下一個格子的GameObject
    public GameObject next_space;
    public GameObject next_space2;

    //格子是否有棋
    public (bool exist, LudoPiece piece) Pieced()
    {
        Vector3 abovePosition = actual_position + Vector3.up * 3f;
        RaycastHit hit;
        if (Physics.Raycast(abovePosition, Vector3.down, out hit, 3f) && hit.collider.gameObject.layer == 10) //Chess
            return (true, hit.collider.gameObject.GetComponent<LudoPiece>());
        return (false, null);
    }
}
