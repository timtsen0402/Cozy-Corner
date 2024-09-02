using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LudoPieceManager : MonoBehaviour
{
    public static LudoPieceManager Instance { get; private set; }

    // 使用字典來按顏色存儲棋子
    private Dictionary<LudoPiece.PieceColor, List<LudoPiece>> piecesByColor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePieceDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePieceDictionary()
    {
        piecesByColor = new Dictionary<LudoPiece.PieceColor, List<LudoPiece>>();
        foreach (LudoPiece.PieceColor color in System.Enum.GetValues(typeof(LudoPiece.PieceColor)))
        {
            piecesByColor[color] = new List<LudoPiece>();
        }
    }

    private void Start()
    {
        CollectAllPieces();
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
        if (!piecesByColor[piece.color].Contains(piece))
        {
            piecesByColor[piece.color].Add(piece);
        }
    }

    public List<LudoPiece> GetPiecesByColor(LudoPiece.PieceColor color)
    {
        return piecesByColor[color];
    }

    public LudoPiece GetPiece(LudoPiece.PieceColor color, int index)
    {
        List<LudoPiece> pieces = piecesByColor[color];
        if (index >= 0 && index < pieces.Count)
        {
            return pieces[index];
        }
        Debug.LogWarning($"Piece index {index} for color {color} is out of range.");
        return null;
    }

    public void SetPiecesClickable(LudoPiece.PieceColor color, bool clickable)
    {
        foreach (var piece in piecesByColor[color])
        {
            piece.isClickable = clickable;
        }
    }

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

    public int GetPieceCount(LudoPiece.PieceColor color)
    {
        return piecesByColor[color].Count;
    }

    public List<LudoPiece> GetAllPieces()
    {
        return piecesByColor.Values.SelectMany(list => list).ToList();
    }

    // 其他可能需要的方法...
}