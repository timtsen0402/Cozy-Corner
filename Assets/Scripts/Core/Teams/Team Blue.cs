using UnityEngine;

public class TeamBlue : Team
{
    public static TeamBlue Instance { get; private set; }

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
        AudioManager.Instance.PlaySFX("Team Blue");

        Debug.Log("Activating Blue team's special function");
        // 獲得防護罩
    }
}