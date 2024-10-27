using static GameConstants;

public class DiceGreen : Dice
{
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
        // if not your dice, refuse it
        if (GameManager.Instance.CurrentGameMode == GameMode.Classic || GameManager.Instance.CurrentPlayerTurn != GreenNumber) return;
        base.OnMouseUp();
    }
}
