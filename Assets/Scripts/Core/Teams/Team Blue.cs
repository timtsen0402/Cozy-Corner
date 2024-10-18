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

        // shield effect
        Renderer renderer = piece.GetComponent<Renderer>();
        Material material = renderer.material;
        material.SetFloat("_Metallic", 1f);
        material.SetFloat("_Glossiness", 1f);
        //

        //實際功能
    }
}