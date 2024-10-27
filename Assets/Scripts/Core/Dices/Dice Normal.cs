using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tool;
using static GameConstants;
using Unity.VisualScripting;

public class DiceNormal : Dice
{
    private Team myTeam = TeamOrange.Instance;

    public override void ResetPosition()
    {
        transform.position = DiceSleepingPositions[0];
    }
    protected override void OnMouseDrag()
    {
        // if not your dice, refuse it
        if (GameManager.Instance.CurrentGameMode == GameMode.Crazy) return;

        base.OnMouseDrag();

    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        // print(hasSettled);
    }
}
