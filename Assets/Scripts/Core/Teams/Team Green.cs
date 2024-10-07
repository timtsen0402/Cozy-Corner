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

    protected override void InitializeTeam(TeamData data)
    {
        base.InitializeTeam(data);
        // Any Orange-specific initialization
    }

    public override void ActivateSpecialFunction(LudoPiece piece)
    {
        //生成在要移動的棋上的pos
        Instantiate(tree, piece.transform.position, Quaternion.identity);
    }
}