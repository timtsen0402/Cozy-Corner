using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Tool
{
    public static List<LudoPiece> TurnToTeam(int number)
    {
        LudoPiece.PieceColor color;

        switch (number)
        {
            case 1:
                color = LudoPiece.PieceColor.Orange;
                break;
            case 2:
                color = LudoPiece.PieceColor.Green;
                break;
            case 3:
                color = LudoPiece.PieceColor.Blue;
                break;
            case 4:
                color = LudoPiece.PieceColor.Red;
                break;
            default:
                throw new System.ArgumentException("Invalid player number", nameof(number));
        }

        return LudoPieceManager.Instance.GetPiecesByColor(color);
    }

    //前提為棋不在home
    public static bool isMovePossible(LudoPiece piece, int steps)
    {
        Space currentSpace = piece.CurrentSpace;

        for (int i = 0; i < steps; i++)
        {

            //首先這個棋的當前位置或下個位置不能是空的
            if (currentSpace == null || !currentSpace.UseNextSpace)
            {
                return false; // 路徑中斷或到達終點
            }

            // 檢查是否需要轉換到 next_space2
            if (currentSpace.NextSpace == piece.StartSpace && currentSpace.UseNextSpace2)
            {
                currentSpace = currentSpace.NextSpace2;
            }
            else
            {
                currentSpace = currentSpace.NextSpace;
            }

            Space spaceNext = currentSpace;

            //如果路徑上有其他棋則動不了
            if (spaceNext.CurrentPiece != null && i != steps - 1)
            {
                return false;
            }
            // 最後一步為己方也動不了
            if (i == steps - 1 && TurnToTeam(GameManager.Instance.CurrentPlayerTurn).Contains(spaceNext.CurrentPiece))
            {
                return false;
            }
        }

        return true; // 移動路徑上沒有阻礙
    }
    public static void SelectClickablePiece(List<LudoPiece> pieces)
    {
        foreach (LudoPiece piece in pieces)
        {
            Space space_start = piece.StartSpace;

            //踢掉不能選的
            if (piece.CurrentSpace.gameObject.layer == 6)//Home
            {
                if (DiceManager.Instance.GetTotalDiceResult() != 6 || pieces.Contains(space_start.CurrentPiece))
                {
                    continue;
                }
            }
            else//在Home以外
            {
                //路徑上沒棋
                //到的點有pos
                //到的點非己方
                if (!isMovePossible(piece, DiceManager.Instance.GetTotalDiceResult())) continue;
            }
            piece.IsClickable = true;
        }
    }
    public static List<LudoPiece> SelectAvailablePiece(List<LudoPiece> pieces)
    {
        List<LudoPiece> clickablePieces = new List<LudoPiece>();
        //踢掉不能選的
        foreach (LudoPiece piece in pieces)
        {
            //踢掉不能選的
            if (piece.CurrentSpace.gameObject.layer == 6)//Home
            {
                if (DiceManager.Instance.GetTotalDiceResult() != 6 || pieces.Contains(piece.StartSpace.CurrentPiece))
                {
                    continue;
                }
            }
            else//在Home以外
            {
                //路徑上沒棋
                //到的點有pos
                //到的點非己方
                if (!isMovePossible(piece, DiceManager.Instance.GetTotalDiceResult())) continue;
            }
            clickablePieces.Add(piece);
        }
        return clickablePieces;
    }

    public static bool isTripleThrowScenario(List<LudoPiece> pieces)
    {
        int pieceAtHome = pieces.Count(piece => piece.CurrentSpace.gameObject.layer == 6);
        bool pieceAtEnd4 = pieces.Any(piece => piece.CurrentSpace.tag == "End4");
        bool pieceAtEnd3 = pieces.Any(piece => piece.CurrentSpace.tag == "End3");
        bool pieceAtEnd2 = pieces.Any(piece => piece.CurrentSpace.tag == "End2");
        bool pieceAtEnd1 = pieces.Any(piece => piece.CurrentSpace.tag == "End1");

        //全在家或是vxxx或是vvxx或是vvvx而且其他都在家
        if (pieces.All(piece => piece.GetComponent<LudoPiece>().CheckCurrentSpace().gameObject.layer == 6) ||
        pieceAtHome == 3 && pieceAtEnd4 ||
        pieceAtHome == 2 && pieceAtEnd4 && pieceAtEnd3 ||
        pieceAtHome == 1 && pieceAtEnd4 && pieceAtEnd3 && pieceAtEnd2)
        {
            return true;

        }
        else return false;
    }
}
