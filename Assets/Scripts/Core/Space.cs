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

    // 新增一個序列化字段來在Inspector中顯示
    [SerializeField]
    private LudoPiece currentPieceInspector;

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

    private void Update()
    {
        // 在Update中更新Inspector顯示的值
        currentPieceInspector = PieceInCurrentSpace();
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

    private GameObject TreeInCurrentSpace()
    {
        RaycastHit hit;
        if (Physics.Raycast(abovePosition, Vector3.down, out hit, 3f, LayerMask.GetMask("Tree")))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    // Uncomment for debugging
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(abovePosition, Vector3.down);
    // }
}