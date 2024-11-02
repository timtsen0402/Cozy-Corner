using UnityEngine;

public class TeamRed : Team
{
    public static TeamRed Instance { get; private set; }

    public int ExtraSteps { get; private set; } = 0;
    private int getExtraStepTriggerCount = -1;

    [Header("Only in Team Red")]
    [SerializeField] int ExtraStepLimitation = 3;

    protected override void Awake()
    {
        base.Awake();
        SetSingleton();

        Name = "Red";
        HexCode = GameConstants.RED_HEX_CODE;
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

    // get extra steps after move
    public override void ActivateSpecialFunction(LudoPiece piece)
    {
        ParticleEffectManager.Instance.PlayEffect("Zap", piece.transform.position);
        AudioManager.Instance.PlaySFX("Team Red");
        getExtraStepTriggerCount += 1;

        // get n extra steps after get n times spacial pattern
        if (ExtraSteps < ExtraStepLimitation && getExtraStepTriggerCount >= ExtraSteps)
        {
            ExtraSteps += 1;
            getExtraStepTriggerCount = -1;
        }
    }
    public override void SetTeamStateDefaultClassic()
    {
        CurrentState = TeamState.AI_Aggressive;
    }
    public override void SetTeamStateDefaultCrazy()
    {
        CurrentState = TeamState.AI_Red;
    }
}