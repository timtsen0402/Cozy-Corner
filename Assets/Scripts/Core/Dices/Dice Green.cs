using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tool;
using static GameConstants;

public class DiceGreen : Dice
{
    private Team myTeam = TeamGreen.Instance;

    public override void ResetPosition()
    {
        transform.position = DiceSleepingPositions[GreenNumber];
    }
    protected override void OnMouseDrag()
    {
        // if not your dice, refuse it
        if (GameManager.Instance.CurrentGameMode == GameMode.Classic || GameManager.Instance.CurrentPlayerTurn != GreenNumber) return;

        base.OnMouseDrag();

    }
    protected override void OnMouseUp()
    {
        if (GameManager.Instance.CurrentGameMode == GameMode.Classic || GameManager.Instance.CurrentPlayerTurn != GreenNumber)
        {

            return;
        }
        base.OnMouseUp();
    }
}
