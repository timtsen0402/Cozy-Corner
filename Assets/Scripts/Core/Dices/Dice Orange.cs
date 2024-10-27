using static GameConstants;

public class DiceOrange : Dice
{
    public override void ResetPosition()
    {
        transform.position = DiceSleepingPositions[OrangeNumber];
    }
    protected override void OnMouseDrag()
    {
        // if not your dice, refuse it
        if (GameManager.Instance.CurrentGameMode == GameMode.Classic || GameManager.Instance.CurrentPlayerTurn != OrangeNumber) return;
        base.OnMouseDrag();
    }
    protected override void OnMouseUp()
    {
        // if not your dice, refuse it
        if (GameManager.Instance.CurrentGameMode == GameMode.Classic || GameManager.Instance.CurrentPlayerTurn != OrangeNumber) return;
        base.OnMouseUp();
    }
}
