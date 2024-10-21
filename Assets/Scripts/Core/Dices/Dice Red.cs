using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tool;
using static GameConstants;

public class DiceRed : Dice
{
    private Team myTeam = TeamRed.Instance;

    public override void ResetPosition()
    {
        transform.position = DiceSleepingPositions[RedNumber];
    }
    protected override void OnMouseDrag()
    {
        // if not your dice, refuse it
        if (GameManager.Instance.CurrentGameMode == GameMode.Classic || GameManager.Instance.CurrentPlayerTurn != RedNumber) return;

        base.OnMouseDrag();

        // List<LudoPiece> allColorPieces = TurnToTeam(GameManager.Instance.CurrentPlayerTurn).GetAllPieces();
        // bool isAllUnclickable = allColorPieces.All(piece => !piece.IsClickable);
        // // 若為人類玩家 且停止 且任何棋是不可被點擊的狀態
        // if (GameManager.Instance.CurrentPlayerTurn <= GameManager.Instance.HumanPlayers && isRollFinished && isAllUnclickable)
        // {
        //     rb.useGravity = false;
        //     transform.position = DiceRotatingPos;
        //     transform.Rotate(DiceRotatingSpeed);
        //     GameManager.Instance.IsDiceThrown = true;
        //     GameManager.Instance.IsPieceMoved = false;
        //     hasSettled = false;
        // }
    }
}
