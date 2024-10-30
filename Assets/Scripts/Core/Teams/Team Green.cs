using UnityEngine;

public class TeamGreen : Team
{
    public static TeamGreen Instance { get; private set; }

    [Header("Only in Team Green")]
    [SerializeField]
    private GameObject tree;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // plant a tree(barrier) and move 1 step
    public override void ActivateSpecialFunction(LudoPiece piece)
    {
        ParticleEffectManager.Instance.PlayEffect("Plant", piece.transform.position);
        AudioManager.Instance.PlaySFX("Team Green");

        Instantiate(tree, new Vector3(piece.transform.position.x, 1.22f, piece.transform.position.z), Quaternion.identity);
        StartCoroutine(LudoPieceManager.Instance.MovePiece(piece, 1));
    }
}