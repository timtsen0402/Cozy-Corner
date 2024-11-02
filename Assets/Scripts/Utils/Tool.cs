using System.Collections.Generic;
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

    public static void SelectClickablePiece(List<LudoPiece> pieces)
    {
        GetValidPieces(pieces).ForEach(piece => piece.IsClickable = true);
    }

    public static List<LudoPiece> SelectAvailablePiece(List<LudoPiece> pieces)
    {
        return GetValidPieces(pieces);
    }

    private static List<LudoPiece> GetValidPieces(List<LudoPiece> pieces)
    {
        List<LudoPiece> clickablePieces = new List<LudoPiece>();
        bool isSpecialPattern = GameManager.Instance.CurrentGameMode == GameMode.Crazy &&
                           DiceManager.Instance.GetCurrentDiceResult() == 6;

        foreach (LudoPiece piece in pieces)
        {
            bool isClickable = false;

            if (piece.CurrentSpace.gameObject.IsInHomeLayer() && pieces.Contains(piece.startSpace.CurrentPiece)) continue;


            if (isSpecialPattern)
            {
                if (piece.CurrentSpace.gameObject.IsInEndLayer()) continue;

                LudoPiece nextPiece = piece.CurrentSpace.NextSpace.CurrentPiece;
                switch (piece.myTeam)
                {
                    case TeamOrange:
                        isClickable = true;
                        break;
                    case TeamGreen:
                        isClickable = nextPiece?.myTeam != TeamGreen.Instance;
                        break;
                    case TeamBlue:
                        isClickable = nextPiece?.myTeam != TeamBlue.Instance;
                        break;
                    case TeamRed:
                        isClickable = true;
                        break;
                }

            }
            else
            {
                if (piece.CurrentSpace.gameObject.IsInHomeLayer())
                {
                    isClickable = DiceManager.Instance.GetCurrentDiceResult() == 6;
                }
                else
                {
                    isClickable = isMovePossible(piece, DiceManager.Instance.GetCurrentDiceResult());
                }
            }

            if (isClickable) clickablePieces.Add(piece);
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
        if (pieces.All(piece => piece.GetComponent<LudoPiece>().GetCurrentSpace().gameObject.IsInHomeLayer()) ||
        pieceAtHome == 3 && pieceAtEnd4 ||
        pieceAtHome == 2 && pieceAtEnd4 && pieceAtEnd3 ||
        pieceAtHome == 1 && pieceAtEnd4 && pieceAtEnd3 && pieceAtEnd2)
        {
            return true;
        }
        return false;
    }
}
