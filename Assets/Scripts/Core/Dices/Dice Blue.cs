using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tool;
using static GameConstants;

public class DiceBlue : Dice
{
    private Team myTeam = TeamBlue.Instance;

    public override void ResetPosition()
    {
        transform.position = DiceSleepingPositions[BlueNumber];
    }
    protected override void OnMouseDrag()
    {
        // if not your dice, refuse it
        if (GameManager.Instance.CurrentGameMode == GameMode.Classic || GameManager.Instance.CurrentPlayerTurn != BlueNumber) return;

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
