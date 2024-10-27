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
    }
    protected override void OnMouseUp()
    {
        if (GameManager.Instance.CurrentGameMode == GameMode.Classic || GameManager.Instance.CurrentPlayerTurn != BlueNumber)
        {

            return;
        }
        base.OnMouseUp();
    }
}
