using System.Collections.Generic;
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
        //ParticleEffectManager.Instance.PlayEffect(effect, piece.transform.position);
        AudioManager.Instance.PlaySFX("Team Blue");

        // 新想法: 非家裡棋都前進一步
        // List<LudoPiece> piecesOutOfHome = new List<LudoPiece>();
        // foreach (LudoPiece p in pieces)
        // {
        //     if (p.CurrentSpace.gameObject.layer != 6)
        //     {
        //         piecesOutOfHome.Add(p);
        //     }
        // }
        bool hasHomePiece = false;
        foreach (LudoPiece p in pieces)
        {
            if (!Tool.isMovePossible(p, 1)) continue;

            bool isHomePiece = p.CurrentSpace.gameObject.layer == 6;
            if (isHomePiece && hasHomePiece) continue;

            ParticleEffectManager.Instance.PlayEffect(effect, p.transform.position);
            StartCoroutine(LudoPieceManager.Instance.AIMovePiece(p, 1));

            if (isHomePiece) hasHomePiece = true;
        }
    }
}