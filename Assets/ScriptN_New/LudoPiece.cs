using UnityEngine;


public class LudoPiece : MonoBehaviour
{
    public enum PieceColor
    {
        Blue,
        Red,
        Green,
        Yellow
    }
    [SerializeField]
    private PieceColor color;

    //public PieceColor Color => color;

    public int position = 0;  // 起始位置
    public bool isHome = true;
    public bool isFinished = false;

    public virtual void Move(int steps)
    {
        if (isHome)
        {
            isHome = false;
            position = 1;
        }
        else
        {
            position += steps;
        }

        if (position >= 57)  // 假設57步為終點
        {
            isFinished = true;
            position = 57;
        }

        // 更新棋子在Unity場景中的位置
        UpdateVisualPosition();
    }

    public virtual void SendHome()
    {
        isHome = true;
        position = 0;
        UpdateVisualPosition();
    }

    protected virtual void UpdateVisualPosition()
    {
        // 這裡實現根據position更新棋子在Unity場景中的實際位置
        // 例如：transform.position = CalculateWorldPosition(position);
    }
}
