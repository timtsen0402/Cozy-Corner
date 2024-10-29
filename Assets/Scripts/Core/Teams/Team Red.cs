using UnityEngine;

public class TeamRed : Team
{
    public static TeamRed Instance { get; private set; }

    public int ExtraSteps { get; private set; } = 0;

    [Header("Only in Team Red")]
    [SerializeField] int ExtraStepLimitation = 3;

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

    // get extra step(s) after move
    public override void ActivateSpecialFunction(LudoPiece piece)
    {
        ParticleEffectManager.Instance.PlayEffect("Zap", piece.transform.position);
        AudioManager.Instance.PlaySFX("Team Red");

        if (ExtraSteps < ExtraStepLimitation)
        {
            ExtraSteps += 1;
        }
    }
}