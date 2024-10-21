using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Tool
{
    public static Team TurnToTeam(int turn)
    {
        switch (turn)
        {
            case 1:
                return TeamOrange.Instance;
            case 2:
                return TeamGreen.Instance;
            case 3:
                return TeamBlue.Instance;
            case 4:
                return TeamRed.Instance;
            default:
                return null;
        }
    }

    public static bool isMovePossible(LudoPiece piece, int steps)
    {
        Space currentSpace = piece.CurrentSpace;

        for (int i = 0; i < steps; i++)
        {
            // piece at last space can't move
            if (currentSpace == null || !currentSpace.UseNextSpace) return false;

            currentSpace = (currentSpace.NextSpace == piece.startSpace && currentSpace.UseNextSpace2)
                ? currentSpace.NextSpace2
                : currentSpace.NextSpace;

            Space spaceNext = currentSpace;
            // if any piece or tree on the path, can't move
            if ((spaceNext.CurrentPiece != null || spaceNext.CurrentTree != null) && i != steps - 1) return false;

            // if ally piece on the target space, can't move
            if (i == steps - 1 && TurnToTeam(GameManager.Instance.CurrentPlayerTurn).GetAllPieces().Contains(spaceNext.CurrentPiece)) return false;

        }
        return true; // movable confirmed
    }

    // 待修改
    public static void SelectClickablePiece(List<LudoPiece> pieces)
    {
        foreach (LudoPiece piece in pieces)
        {
            Space space_start = piece.startSpace;

            if (piece.CurrentSpace.gameObject.IsInHomeLayer())
            {
                if (DiceManager.Instance.GetCurrentDiceResult() != 6 || pieces.Contains(space_start.CurrentPiece))
                {
                    continue;
                }
            }
            else
            {
                if (!isMovePossible(piece, DiceManager.Instance.GetCurrentDiceResult())) continue;
            }
            piece.IsClickable = true;
        }
    }
    //

    public static List<LudoPiece> SelectAvailablePiece(List<LudoPiece> pieces)
    {
        List<LudoPiece> clickablePieces = new List<LudoPiece>();

        // special pattern
        if (GameManager.Instance.CurrentGameMode == GameMode.Crazy && DiceManager.Instance.GetCurrentDiceResult() == 6)
        {
            foreach (LudoPiece piece in pieces)
            {
                // unavailable at end space
                if (piece.CurrentSpace.gameObject.layer != 9)
                {
                    clickablePieces.Add(piece);
                }
            }
            return clickablePieces;
        }

        // normal number
        foreach (LudoPiece piece in pieces)
        {
            // piece at home
            if (piece.CurrentSpace.gameObject.IsInHomeLayer())
            {
                if (DiceManager.Instance.GetCurrentDiceResult() != 6 || pieces.Contains(piece.startSpace.CurrentPiece))
                {
                    continue;
                }
            }
            // otherwise
            else
            {
                if (!isMovePossible(piece, DiceManager.Instance.GetCurrentDiceResult())) continue;
            }
            clickablePieces.Add(piece);
        }
        return clickablePieces;
    }

    public static bool isTripleThrowScenario(List<LudoPiece> pieces)
    {
        int pieceAtHome = pieces.Count(piece => piece.CurrentSpace.gameObject.IsInHomeLayer());
        bool pieceAtEnd4 = pieces.Any(piece => piece.CurrentSpace.tag == "End4");
        bool pieceAtEnd3 = pieces.Any(piece => piece.CurrentSpace.tag == "End3");
        bool pieceAtEnd2 = pieces.Any(piece => piece.CurrentSpace.tag == "End2");
        bool pieceAtEnd1 = pieces.Any(piece => piece.CurrentSpace.tag == "End1");

        // 4 situations (vxxx or vvxx or vvvx or vvvv)
        if (pieces.All(piece => piece.GetComponent<LudoPiece>().CheckCurrentSpace().gameObject.IsInHomeLayer()) ||
        pieceAtHome == 3 && pieceAtEnd4 ||
        pieceAtHome == 2 && pieceAtEnd4 && pieceAtEnd3 ||
        pieceAtHome == 1 && pieceAtEnd4 && pieceAtEnd3 && pieceAtEnd2)
        {
            return true;
        }
        return false;
    }
}
