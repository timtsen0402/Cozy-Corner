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

    private Vector3 abovePosition;

    public LudoPiece CurrentPiece
    {
        get
        {
            return PieceInCurrentSpace();
        }
    }

    private void Start()
    {
        abovePosition = ActualPosition + Vector3.up * 3f;
    }

    private LudoPiece PieceInCurrentSpace()
    {
        RaycastHit hit;
        if (Physics.Raycast(abovePosition, Vector3.down, out hit, 3f, LayerMask.GetMask("Piece")))
        {
            return hit.collider.gameObject.GetComponent<LudoPiece>();
        }
        return null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(abovePosition, Vector3.down);
    }
}