using System.Collections.Generic;
using UnityEngine;

public class TeamOrange : Team
{
    public static TeamOrange Instance { get; private set; }

    [Header("Only in Team Orange")]
    [SerializeField] private Space end1;
    [SerializeField] private Space end2;
    [SerializeField] private Space end3;
    [SerializeField] private Space end4;
    List<Space> spaces = new List<Space>();

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

        spaces.Add(end4);
        spaces.Add(end3);
        spaces.Add(end2);
        spaces.Add(end1);
    }

    public override void ActivateSpecialFunction(LudoPiece piece)
    {
        ParticleEffectManager.Instance.PlayEffect(effect, piece.transform.position);
        AudioManager.Instance.PlaySFX("Team Orange");

        //炸兩側的棋子回家
        Space currentSpace = piece.CurrentSpace;
        Space nextSpace = currentSpace.NextSpace;
        Space previousSpace = currentSpace.PreviousSpace;

        piece.killCount += ImpactAfterExplosion(nextSpace);
        piece.killCount += ImpactAfterExplosion(previousSpace);
    }

    private int ImpactAfterExplosion(Space space)
    {
        int kill = 0;
        if (space.CurrentPiece != null)
        {
            if (space.CurrentPiece.CompareTag("Orange"))
            {
                // 送他去終點
                foreach (var s in spaces)
                {
                    if (s.CurrentPiece == null)
                    {
                        space.CurrentPiece.transform.position = s.ActualPosition;
                        AudioManager.Instance.PlaySFX("Transport");
                        return 0;
                    }
                }
            }
            else
            {
                space.CurrentPiece.ResetToHome();
                kill++;
            }
        }
        else if (space.CurrentTree != null)
        {
            Destroy(space.CurrentTree);
        }
        return kill;
    }
}