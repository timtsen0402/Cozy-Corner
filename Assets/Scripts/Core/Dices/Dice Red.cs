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

    }
    protected override void OnMouseUp()
    {
        if (GameManager.Instance.CurrentGameMode == GameMode.Classic || GameManager.Instance.CurrentPlayerTurn != RedNumber)
        {
            return;
        }
        base.OnMouseUp();
    }
}
