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
        SetSingleton();

        Name = "Green";
        HexCode = GameConstants.GREEN_HEX_CODE;
    }
    protected override void SetSingleton()
    {
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

        Instantiate(tree, piece.CurrentSpace.ActualPosition, Quaternion.identity);
        StartCoroutine(LudoPieceManager.Instance.MovePiece(piece, 1));
    }
    public override void SetTeamStateDefaultClassic()
    {
        CurrentState = TeamState.AI_Aggressive;
    }
    public override void SetTeamStateDefaultCrazy()
    {
        CurrentState = TeamState.AI_Green;
    }
}