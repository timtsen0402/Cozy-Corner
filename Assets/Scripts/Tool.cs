using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

using static GameController;
using Unity.VisualScripting;


public static class Tool
{
    public static bool AreAllChessOnLayer(GameObject team, int targetLayer)
    {
        return team.transform.Cast<Transform>()
            .All(child =>
            {
                LudoPiece piece = child.GetComponent<LudoPiece>();
                return piece != null && piece.CheckSelfPos() != null && piece.CheckSelfPos().layer == targetLayer;
            });
    }
    public static List<LudoPiece> TurnToTeam(int number)
    {
        switch (number)
        {
            case 1:
                return LudoPieceManager.Instance.GetPiecesByColor(LudoPiece.PieceColor.Orange);
            case 2:
                return LudoPieceManager.Instance.GetPiecesByColor(LudoPiece.PieceColor.Green);
            case 3:
                return LudoPieceManager.Instance.GetPiecesByColor(LudoPiece.PieceColor.Blue);
            case 4:
                return LudoPieceManager.Instance.GetPiecesByColor(LudoPiece.PieceColor.Red);
            default:
                return new List<LudoPiece>();
        }
    }

    public static bool isNextSpaceEmpty(GameObject space)
    {
        return !space.GetComponent<Space>().next_space.GetComponent<Space>().Pieced().exist;
    }
    //前提為棋不在home
    public static bool isMovePossible(GameObject piece, int steps)
    {
        LudoPiece chessPiece = piece.GetComponent<LudoPiece>();
        GameObject currentSpace = chessPiece.currentPosition;

        for (int i = 0; i < steps; i++)
        {

            //首先這個棋的當前位置或下個位置不能是空的
            if (currentSpace == null || currentSpace.GetComponent<Space>().next_space == null)
            {
                return false; // 路徑中斷或到達終點
            }

            // 檢查是否需要轉換到 next_space2
            if (currentSpace.GetComponent<Space>().next_space == chessPiece.start_space && currentSpace.GetComponent<Space>().next_space2 != null)
            {
                currentSpace = currentSpace.GetComponent<Space>().next_space2;
            }
            else
            {
                currentSpace = currentSpace.GetComponent<Space>().next_space;
            }

            Space spaceNext = currentSpace.GetComponent<Space>();

            //如果路徑上有其他棋則動不了
            if (spaceNext.Pieced().exist && i != steps - 1)
            {
                return false;
            }
            // 最後一步為己方也動不了
            if (i == steps - 1 && TurnToTeam(currentPlayerTurn).Contains(spaceNext.Pieced().piece))
            {
                return false;
            }
        }

        return true; // 移動路徑上沒有阻礙
    }
    public static void SelectClickableChess(List<LudoPiece> chesses)
    {
        //for every chesses
        foreach (LudoPiece piece in chesses)
        {
            //chess.GetComponent<Rigidbody>().isKinematic = true;
            Space space_start = piece.GetComponent<LudoPiece>().start_space.GetComponent<Space>();

            //踢掉不能選的
            if (piece.GetComponent<LudoPiece>().currentPosition.layer == 6)//Home
            {
                if (DiceManager.Instance.GetTotalDiceResult() != 6 || chesses.Contains(space_start.Pieced().piece))
                {
                    continue;
                }
            }
            else//在Home以外
            {
                //路徑上沒棋
                //到的點有pos
                //到的點非己方
                if (!isMovePossible(piece.gameObject, DiceManager.Instance.GetTotalDiceResult())) continue;
            }
            piece.GetComponent<LudoPiece>().isClickable = true;
        }
    }
    public static List<LudoPiece> SelectAvailableChess(List<LudoPiece> pieces)
    {
        List<LudoPiece> clickableChessPieces = new List<LudoPiece>();
        //踢掉不能選的
        foreach (LudoPiece piece in pieces)
        {

            Space space_start = piece.start_space.GetComponent<Space>();

            //踢掉不能選的
            if (piece.currentPosition.layer == 6)//Home
            {
                if (DiceManager.Instance.GetTotalDiceResult() != 6 || pieces.Contains(space_start.Pieced().piece))
                {
                    continue;
                }
            }
            else//在Home以外
            {
                //路徑上沒棋
                //到的點有pos
                //到的點非己方
                if (!isMovePossible(piece.gameObject, DiceManager.Instance.GetTotalDiceResult())) continue;
            }
            clickableChessPieces.Add(piece);
        }
        return clickableChessPieces;
    }
    public static bool isGameOver()
    {

        if (TurnToTeam(1).All(piece => piece.GetComponent<LudoPiece>().CheckSelfPos().layer == 9))
        {
            Debug.Log($"Winner is team 1");
            return true;
        }
        else if (TurnToTeam(2).All(piece => piece.GetComponent<LudoPiece>().CheckSelfPos().layer == 9))
        {
            Debug.Log($"Winner is team 2");
            return true;
        }
        else if (TurnToTeam(3).All(piece => piece.GetComponent<LudoPiece>().CheckSelfPos().layer == 9))
        {
            Debug.Log($"Winner is team 3");
            return true;
        }
        else if (TurnToTeam(4).All(piece => piece.GetComponent<LudoPiece>().CheckSelfPos().layer == 9))
        {
            Debug.Log($"Winner is team 4");
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool isTripleThrowScenario(List<LudoPiece> pieces)
    {
        int chessAtHome = pieces.Count(piece => piece.GetComponent<LudoPiece>().currentPosition.layer == 6);
        bool chessAtEnd4 = pieces.Any(piece => piece.GetComponent<LudoPiece>().currentPosition.tag == "End4");
        bool chessAtEnd3 = pieces.Any(piece => piece.GetComponent<LudoPiece>().currentPosition.tag == "End3");
        bool chessAtEnd2 = pieces.Any(piece => piece.GetComponent<LudoPiece>().currentPosition.tag == "End2");
        bool chessAtEnd1 = pieces.Any(piece => piece.GetComponent<LudoPiece>().currentPosition.tag == "End1");

        //全在家或是vxxx或是vvxx或是vvvx而且其他都在家
        if (pieces.All(piece => piece.GetComponent<LudoPiece>().CheckSelfPos().layer == 6) ||
        chessAtHome == 3 && chessAtEnd4 ||
        chessAtHome == 2 && chessAtEnd4 && chessAtEnd3 ||
        chessAtHome == 1 && chessAtEnd4 && chessAtEnd3 && chessAtEnd2)
        {
            return true;

        }
        else return false;
    }
}
