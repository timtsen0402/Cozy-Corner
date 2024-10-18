using UnityEngine;

public class TeamOrange : Team
{
    public static TeamOrange Instance { get; private set; }

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
        ParticleEffectManager.Instance.PlayEffect(effect, piece.transform.position);
        AudioManager.Instance.PlaySFX("Team Orange");

        //炸兩側的棋子回家
        Space currentSpace = piece.CurrentSpace;
        Space nextSpace = currentSpace.NextSpace;
        Space previousSpace = currentSpace.PreviousSpace;
        if (nextSpace.CurrentPiece != null && !nextSpace.CurrentPiece.CompareTag("Orange"))
        {
            nextSpace.CurrentPiece.ResetToHome();
            piece.killCount++;
        }
        if (previousSpace.CurrentPiece != null && !previousSpace.CurrentPiece.CompareTag("Orange"))
        {
            previousSpace.CurrentPiece.ResetToHome();
            piece.killCount++;
        }
    }
}