using static GameConstants;

public class DiceNormal : Dice
{
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
    protected override void OnMouseUp()
    {
        // if not your dice, refuse it
        if (GameManager.Instance.CurrentGameMode == GameMode.Crazy) return;
        base.OnMouseUp();
    }
}
