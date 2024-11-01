using static Tool;

public class TeamBlue : Team
{
    public static TeamBlue Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        SetSingleton();

        Name = "Blue";
        HexCode = GameConstants.BLUE_HEX_CODE;
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

    // all movable pieces can move 1 step, home piece can go out
    public override void ActivateSpecialFunction(LudoPiece piece)
    {
        AudioManager.Instance.PlaySFX("Team Blue");

        bool hasHomePiece = false;
        foreach (LudoPiece p in pieces)
        {
            if (!isMovePossible(p, 1)) continue;

            bool isHomePiece = p.CurrentSpace.gameObject.IsInHomeLayer();
            if (isHomePiece && hasHomePiece) continue;

            ParticleEffectManager.Instance.PlayEffect("Splash", p.transform.position);
            StartCoroutine(LudoPieceManager.Instance.MovePiece(p, 1));

            if (isHomePiece) hasHomePiece = true;
        }
    }
    public override void SetTeamStateDefaultClassic()
    {
        CurrentState = TeamState.AI_Aggressive;
    }
    public override void SetTeamStateDefaultCrazy()
    {
        CurrentState = TeamState.AI_Blue;
    }
}