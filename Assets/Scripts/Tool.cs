using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

using static GameController;
using static Rotate;
using Unity.VisualScripting;


public static class Tool
{
    public static bool AreAllChessOnLayer(GameObject team, int targetLayer)
    {
        return team.transform.Cast<Transform>()
            .All(child =>
            {
                Chess chess = child.GetComponent<Chess>();
                return chess != null && chess.CheckSelfPos() != null && chess.CheckSelfPos().layer == targetLayer;
            });
    }
    public static GameObject[] TurnToTeam(int number)
    {
        switch (number)
        {
            case 1:
                return new GameObject[] { chessO1, chessO2, chessO3, chessO4 };
            case 2:
                return new GameObject[] { chessG1, chessG2, chessG3, chessG4 };
            case 3:
                return new GameObject[] { chessB1, chessB2, chessB3, chessB4 };
            case 4:
                return new GameObject[] { chessR1, chessR2, chessR3, chessR4 };
            default:
                return null;
        }
    }

    public static bool isNextSpaceEmpty(GameObject space)
    {
        return !space.GetComponent<Space>().next_space.GetComponent<Space>().Chessed().exist;
    }
    //前提為棋不在home
    public static bool isMovePossible(GameObject chess, int steps)
    {
        Chess chessPiece = chess.GetComponent<Chess>();
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
            if (spaceNext.Chessed().exist && i != steps - 1)
            {
                return false;
            }
            // 最後一步為己方也動不了
            if (i == steps - 1 && TurnToTeam(currentPlayerTurn).Contains(spaceNext.Chessed().chess))
            {
                return false;
            }
        }

        return true; // 移動路徑上沒有阻礙
    }
    public static void SelectClickableChess(GameObject[] chesses)
    {
        //for every chesses
        foreach (GameObject chess in chesses)
        {
            //chess.GetComponent<Rigidbody>().isKinematic = true;
            Space space_start = chess.GetComponent<Chess>().start_space.GetComponent<Space>();

            //踢掉不能選的
            if (chess.GetComponent<Chess>().currentPosition.layer == 6)//Home
            {
                if (GetLatestDiceResult() != 6 || chesses.Contains(space_start.Chessed().chess))
                {
                    continue;
                }
            }
            else//在Home以外
            {
                //路徑上沒棋
                //到的點有pos
                //到的點非己方
                if (!isMovePossible(chess, GetLatestDiceResult())) continue;
            }
            chess.GetComponent<Chess>().isClickable = true;
        }
    }
    public static List<GameObject> SelectAvailableChess(GameObject[] chesses)
    {
        List<GameObject> clickableChessPieces = new List<GameObject>();
        //踢掉不能選的
        foreach (GameObject chess in chesses)
        {
            //chess.GetComponent<Rigidbody>().isKinematic = true;
            Space space_start = chess.GetComponent<Chess>().start_space.GetComponent<Space>();

            //踢掉不能選的
            if (chess.GetComponent<Chess>().currentPosition.layer == 6)//Home
            {
                if (GetLatestDiceResult() != 6 || chesses.Contains(space_start.Chessed().chess))
                {
                    continue;
                }
            }
            else//在Home以外
            {
                //路徑上沒棋
                //到的點有pos
                //到的點非己方
                if (!isMovePossible(chess, GetLatestDiceResult())) continue;
            }
            clickableChessPieces.Add(chess);
        }
        return clickableChessPieces;
    }
    public static bool isGameOver()
    {

        if (TurnToTeam(1).All(chess => chess.GetComponent<Chess>().CheckSelfPos().layer == 9))
        {
            Debug.Log($"Winner is team 1");
            return true;
        }
        else if (TurnToTeam(2).All(chess => chess.GetComponent<Chess>().CheckSelfPos().layer == 9))
        {
            Debug.Log($"Winner is team 2");
            return true;
        }
        else if (TurnToTeam(3).All(chess => chess.GetComponent<Chess>().CheckSelfPos().layer == 9))
        {
            Debug.Log($"Winner is team 3");
            return true;
        }
        else if (TurnToTeam(4).All(chess => chess.GetComponent<Chess>().CheckSelfPos().layer == 9))
        {
            Debug.Log($"Winner is team 4");
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool isTripleThrowScenario(GameObject[] chesses)
    {
        int chessAtHome = chesses.Count(chess => chess.GetComponent<Chess>().currentPosition.layer == 6);
        bool chessAtEnd4 = chesses.Any(chess => chess.GetComponent<Chess>().currentPosition.tag == "End4");
        bool chessAtEnd3 = chesses.Any(chess => chess.GetComponent<Chess>().currentPosition.tag == "End3");
        bool chessAtEnd2 = chesses.Any(chess => chess.GetComponent<Chess>().currentPosition.tag == "End2");
        bool chessAtEnd1 = chesses.Any(chess => chess.GetComponent<Chess>().currentPosition.tag == "End1");

        //全在家或是vxxx或是vvxx或是vvvx而且其他都在家
        if (chesses.All(chess => chess.GetComponent<Chess>().CheckSelfPos().layer == 6) ||
        chessAtHome == 3 && chessAtEnd4 ||
        chessAtHome == 2 && chessAtEnd4 && chessAtEnd3 ||
        chessAtHome == 1 && chessAtEnd4 && chessAtEnd3 && chessAtEnd2)
        {
            return true;

        }
        else return false;
    }
}
