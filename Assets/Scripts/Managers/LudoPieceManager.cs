using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class LudoPieceManager : MonoBehaviour
{
    public static LudoPieceManager Instance { get; private set; }
    public List<LudoPiece.PieceColor> UnfinishedColors { get; set; }
    public List<LudoPiece.PieceColor> FinishedColors { get; set; }
    // 使用字典來按顏色存儲棋子
    private Dictionary<LudoPiece.PieceColor, List<LudoPiece>> piecesByColor;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
            InitializePieceDictionary();
            CollectAllPieces();
        }
        else
        {
            Destroy(gameObject);
        }

        UnfinishedColors = System.Enum.GetValues(typeof(LudoPiece.PieceColor))
                                    .Cast<LudoPiece.PieceColor>()
                                    .ToList();
        FinishedColors = new List<LudoPiece.PieceColor>();
    }


    private void InitializePieceDictionary()
    {
        piecesByColor = new Dictionary<LudoPiece.PieceColor, List<LudoPiece>>();
        foreach (LudoPiece.PieceColor color in System.Enum.GetValues(typeof(LudoPiece.PieceColor)))
        {
            piecesByColor[color] = new List<LudoPiece>();
        }
    }

    private void CollectAllPieces()
    {
        LudoPiece[] allPieces = FindObjectsOfType<LudoPiece>();
        foreach (LudoPiece piece in allPieces)
        {
            AddPiece(piece);
        }
    }

    public void AddPiece(LudoPiece piece)
    {
        if (!piecesByColor[piece.Color].Contains(piece))
        {
            piecesByColor[piece.Color].Add(piece);
        }
    }
    #region Get

    public LudoPiece SelectPiece(AIStrategies.Difficulty difficulty, List<LudoPiece> availablePieces)
    {
        var strategy = AIStrategies.GetStrategy(difficulty);
        return strategy(availablePieces);
    }

    public List<LudoPiece> GetNearestPiecesToFinishByColor(List<LudoPiece> pieces)
    {
        return pieces
           .OrderBy(piece => piece.GetDistanceToTheEnd())
           .ToList();
    }
    //sortedList1.Sort();
    public int GetColorKillCount(LudoPiece.PieceColor color)
    {
        List<LudoPiece> colorPieces = GetPiecesByColor(color);
        int totalKillCount = 0;
        foreach (LudoPiece piece in colorPieces)
        {
            totalKillCount += piece.killCount;
        }
        return totalKillCount;
    }

    public string GetHexCode(LudoPiece.PieceColor color)
    {
        switch (color)
        {
            case LudoPiece.PieceColor.Orange:
                return "#FF8C00";
            case LudoPiece.PieceColor.Green:
                return "#228B22";
            case LudoPiece.PieceColor.Blue:
                return "#1E90FF";
            case LudoPiece.PieceColor.Red:
                return "#CD5C5C";
            default:
                return "#FFFFFF"; // white
        }
    }
    public bool isCertainColorTeamEnd(LudoPiece.PieceColor color)
    {
        List<LudoPiece> certainColorPieces = GetPiecesByColor(color);
        if (!certainColorPieces.All(piece => piece.CheckCurrentSpace().gameObject.layer == 9)) return false;
        return true;

    }
    public List<LudoPiece> GetAllPieces()
    {
        var allPieces = piecesByColor.Values.SelectMany(list => list).ToList();
        return allPieces;
    }

    public List<LudoPiece> GetPiecesByColor(LudoPiece.PieceColor color)
    {
        var pieces = GetAllPieces().Where(p => p.Color == color).ToList();
        return pieces;
    }

    public LudoPiece GetPiece(LudoPiece.PieceColor color, int index)
    {
        List<LudoPiece> pieces = piecesByColor[color];
        if (index >= 0 && index < pieces.Count)
        {
            return pieces[index];
        }
        return null;
    }
    #endregion Get

    #region Reset
    public void ResetPieces(LudoPiece.PieceColor color)
    {
        foreach (var piece in piecesByColor[color])
        {
            piece.ResetToHome();
        }
    }

    public void ResetAllPieces()
    {
        foreach (var pieceList in piecesByColor.Values)
        {
            foreach (var piece in pieceList)
            {
                piece.ResetToHome();
            }
        }
    }
    #endregion Reset

    #region Move
    public IEnumerator MoveToPosition(LudoPiece piece, Vector3 targetPosition, float speed = 0.1f)
    {
        Tween moveTween = piece.transform.DOMove(targetPosition, speed).SetEase(Ease.OutQuad);
        AudioManager.Instance.PlaySFX("PieceWalk");

        yield return moveTween.WaitForCompletion();
    }

    public IEnumerator AIMovePiece(LudoPiece piece)
    {
        piece.IsMoving = true;
        int steps = DiceManager.Instance.GetTotalDiceResult();
        for (int i = 0; i < steps; i++)
        {
            Space currentSpace = piece.CheckCurrentSpace();

            if (currentSpace.NextSpace == null)
            {
                break;
            }

            Vector3 nextPosition;

            if (currentSpace.NextSpace == piece.StartSpace)
            {
                if (piece.CheckCurrentSpace().gameObject.layer == 6)//Home
                {
                    nextPosition = piece.StartSpace.ActualPosition;
                    yield return MoveToPosition(piece, nextPosition);
                    break;
                }
                else if (piece.CheckCurrentSpace().gameObject.layer == 8)//Path
                {
                    nextPosition = currentSpace.NextSpace2.ActualPosition;

                }
                else
                {
                    nextPosition = currentSpace.NextSpace.ActualPosition;
                }
            }
            else
            {
                nextPosition = currentSpace.NextSpace.ActualPosition;
            }
            yield return MoveToPosition(piece, nextPosition);

            // 短暂暂停，让玩家能看清每一步的移动
            yield return new WaitForSeconds(0.1f);

        }
        piece.IsMoving = false;
        GameManager.Instance.IsPieceMoved = true;
    }
    #endregion Move

    #region Debug
    public void LogAllPieces()
    {
        Debug.Log($"Total pieces: {GetAllPieces().Count}");
        foreach (var piece in GetAllPieces())
        {
            Debug.Log($"Piece: {piece.name}, Color: {piece.Color}");
        }
    }
    #endregion Debug
}