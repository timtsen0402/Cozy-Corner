using UnityEngine;
using static GameConstants;

public class LudoPiece : MonoBehaviour
{
    [field: SerializeField]
    public Space homeSpace { get; private set; }
    public Space startSpace { get; private set; }
    public Space CurrentSpace { get; private set; }

    [HideInInspector] public int killCount;
    [HideInInspector] public bool IsClickable;
    [HideInInspector] public bool IsMoving = false;

    private Team myTeam;
    private Rigidbody rb;
    private Renderer rend;
    private Color originalColor;

    void Awake()
    {
        myTeam = GetComponentInParent<Team>();
        rb = GetComponent<Rigidbody>();
        rend = gameObject.GetComponent<Renderer>();
        startSpace = myTeam.StartSpace;
        originalColor = rend.material.color;
    }

    void Start()
    {
        ResetToHome();
    }

    void Update()
    {
        if (IsClickable) OnClick();

        // detect current space
        CurrentSpace = GetCurrentSpace();

        // ensure this piece is on the space
        if (CurrentSpace == null) ResetToHome();
    }

    public void ResetToHome()
    {
        transform.position = homeSpace.ActualPosition;
        transform.rotation = PieceRotation;
        if (!rb.isKinematic) rb.velocity = Vector3.zero;
    }

    //  for killing other pieces
    void OnCollisionEnter(Collision collision)
    {
        if (!IsMoving) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Tree"))
        {
            Destroy(collision.gameObject);
        }

        LudoPiece collisionPiece = collision.gameObject.GetComponent<LudoPiece>();
        if (collisionPiece != null && !myTeam.GetAllPieces().Contains(collisionPiece))
        {
            collisionPiece.ResetToHome();
            AudioManager.Instance.PlaySFX("Kick");
            killCount++;
        }
    }

    #region Get
    public Space GetCurrentSpace()
    {
        return Physics.Raycast(transform.position, Vector3.down, out var hit)
               && hit.collider.TryGetComponent(out Space space)
            ? space
            : CurrentSpace;
    }

    public int GetDistanceToTheEnd()
    {
        int count = 0;
        for (Space space = CurrentSpace; space != null; count++)
        {
            if (!space.UseNextSpace) return count;

            space = (space.NextSpace == startSpace && space.UseNextSpace2)
                ? space.NextSpace2
                : space.NextSpace;
        }
        return count;
    }
    #endregion Get

    #region  OnMouse
    // for moving piece
    public void OnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // move an reset clickable pieces
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                myTeam.ResetClickableState();
                GameManager.Instance.ChosenPiece = this;
                StartCoroutine(LudoPieceManager.Instance.MovePiece(this, DiceManager.Instance.GetCurrentDiceResult()));
            }
        }
    }
    // effect
    void OnMouseEnter()
    {
        if (IsClickable)
        {
            rend.material.color = Color.black;
        }
    }
    void OnMouseExit()
    {
        rend.material.color = originalColor;
    }
    #endregion  OnMouse
}