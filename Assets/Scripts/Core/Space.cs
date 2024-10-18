using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{

    [field: SerializeField]
    public Vector3 ActualPosition { get; private set; }
    [field: SerializeField]
    public bool UseNextSpace { get; private set; }
    [field: SerializeField]
    public bool UseNextSpace2 { get; private set; }
    [field: SerializeField]
    public Space NextSpace { get; private set; }
    [field: SerializeField]
    public Space NextSpace2 { get; private set; }
    [field: SerializeField]
    public Space PreviousSpace { get; private set; }

    private Vector3 abovePosition;

    public LudoPiece CurrentPiece
    {
        get
        {
            return PieceInCurrentSpace();
        }
    }
    public GameObject CurrentTree
    {
        get
        {
            return TreeInCurrentSpace();
        }
    }

    private void Start()
    {
        abovePosition = ActualPosition + Vector3.up * 3f;
    }
    // private void Update()
    // {
    //     if (CurrentTree != null)
    //     {
    //         Debug.Log(CurrentTree.name);
    //         Debug.Log(CurrentTree.tag);
    //         Debug.Log(CurrentTree.layer);
    //     }
    // }

    private LudoPiece PieceInCurrentSpace()
    {
        RaycastHit hit;
        if (Physics.Raycast(abovePosition, Vector3.down, out hit, 3f, LayerMask.GetMask("Piece")))
        {
            return hit.collider.gameObject.GetComponent<LudoPiece>();
        }
        return null;
    }

    private GameObject TreeInCurrentSpace()
    {
        RaycastHit hit;
        if (Physics.Raycast(abovePosition, Vector3.down, out hit, 3f, LayerMask.GetMask("Tree")))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(abovePosition, Vector3.down);
    }
}
