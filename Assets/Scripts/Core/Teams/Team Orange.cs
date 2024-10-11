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
        Debug.Log("Activating Orange team's special function");
        // 爆炸影響前後的棋子
        ParticleEffectManager.Instance.PlayEffect(effect, piece.transform.position);

        AudioManager.Instance.PlaySFX("Team Orange");
    }
}