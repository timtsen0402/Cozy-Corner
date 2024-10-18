using System.Linq;
using UnityEngine;
using System.Collections;
using static Tool;
using Unity.IO.LowLevel.Unsafe;

public class TeamRed : Team
{
    public static TeamRed Instance { get; private set; }

    public int ExtraSteps { get; private set; } = 0;



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

    // void Update()
    // {
    //     Debug.Log(ExtraSteps);
    //     // if (Tool.TurnToTeam(GameManager.Instance.CurrentPlayerTurn) == TeamRed.Instance)
    //     // {
    //     //     Debug.Log(this.ToString());

    //     // }
    // }

    protected override void InitializeTeam(TeamData data)
    {
        base.InitializeTeam(data);
        // Any Orange-specific initialization
    }

    public override void ActivateSpecialFunction(LudoPiece piece)
    {
        ParticleEffectManager.Instance.PlayEffect(effect, piece.transform.position);
        AudioManager.Instance.PlaySFX("Team Red");


        ExtraSteps += 1;
    }
}