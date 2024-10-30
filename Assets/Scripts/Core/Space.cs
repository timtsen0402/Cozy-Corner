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
    private void Start()
    {
        abovePosition = ActualPosition + Vector3.up * 3f;
    }

    private T CheckObject<T>(string layerName) where T : Object
    {
        RaycastHit hit;
        if (Physics.Raycast(abovePosition, Vector3.down, out hit, 3f, LayerMask.GetMask(layerName)))
        {
            return typeof(T) == typeof(GameObject) ?
                hit.collider.gameObject as T :
                hit.collider.gameObject.GetComponent<T>();
        }
        return null;
    }

    public LudoPiece CurrentPiece => CheckObject<LudoPiece>("Piece");
    public GameObject CurrentTree => CheckObject<GameObject>("Tree");

}