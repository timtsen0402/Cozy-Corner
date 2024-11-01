using System.Collections.Generic;
using UnityEngine;

public class AIStrategies : MonoBehaviour
{
    public delegate LudoPiece AIStrategy(List<LudoPiece> availablePieces);

    private static readonly Dictionary<TeamState, AIStrategy> StrategyMap = new Dictionary<TeamState, AIStrategy>
{
    { TeamState.AI_Dumb, RandomSelect },
    { TeamState.AI_Peaceful, FrontmostFirst },
    { TeamState.AI_Aggressive, AttackFirst },

    { TeamState.AI_Orange, CustomizedStrategy(TeamOrange.Instance) },
    { TeamState.AI_Green, CustomizedStrategy(TeamGreen.Instance) },
    { TeamState.AI_Blue, CustomizedStrategy(TeamBlue.Instance) },
    { TeamState.AI_Red, CustomizedStrategy(TeamRed.Instance) },
};

    public static AIStrategy GetStrategy(TeamState teamState)
    {
        if (StrategyMap.TryGetValue(teamState, out var strategy))
        {
            return strategy;
        }
        return RandomSelect;
    }
    public static LudoPiece RandomSelect(List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;

        int randomIndex = Random.Range(0, pieces.Count);
        LudoPiece selectedPiece = pieces[randomIndex];
        return selectedPiece;
    }

    /*-----------------------------------------------Only for Classic Mode-------------------------------------------------------------*/

    public static LudoPiece FrontmostFirst(List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;
        if (pieces.Count == 1) return pieces[0];

        // if movable chesses number is 2,3 or 4
        List<LudoPiece> nearestPieces = LudoPieceManager.Instance.GetNearestPiecesToFinish(pieces);
        return nearestPieces[0];
    }

    public static LudoPiece AttackFirst(List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;
        if (pieces.Count == 1) return pieces[0];

        int steps = DiceManager.Instance.GetCurrentDiceResult();
        // if movable chesses number is 2,3 or 4
        List<LudoPiece> nearestPieces = LudoPieceManager.Instance.GetNearestPiecesToFinish(pieces);

        LudoPiece result = CheckKickOrEnd(nearestPieces, steps) ??
                CheckGoOut(nearestPieces, steps);

        // 4.Move the frontmost
        return result ?? nearestPieces[0];
    }

    /*-----------------------------------------------Only for Crazy Mode-------------------------------------------------------------*/
    private static AIStrategy CustomizedStrategy(Team team)
    {
        return (pieces) => CustomizedStrategyImplementation(team, pieces);
    }

    private static LudoPiece CustomizedStrategyImplementation(Team team, List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;
        if (pieces.Count == 1) return pieces[0];

        int steps = DiceManager.Instance.GetCurrentDiceResult();
        List<LudoPiece> nearestPieces = LudoPieceManager.Instance.GetNearestPiecesToFinish(pieces);
        LudoPiece result = null;

        switch (team)
        {
            case TeamOrange:
                result = CheckExplode(nearestPieces, steps) ??
                        CheckKickOrEnd(nearestPieces, steps) ??
                        CheckGoOut(nearestPieces, steps);
                break;

            case TeamGreen:
                result = CheckPlantTree(nearestPieces) ??
                        CheckKickOrEnd(nearestPieces, steps) ??
                        CheckGoOut(nearestPieces, steps);
                break;

            case TeamBlue:
                if (steps == 6) return nearestPieces[0];
                result = CheckKickOrEnd(nearestPieces, steps) ??
                        CheckGoOut(nearestPieces, steps);
                break;

            case TeamRed:
                result = CheckKickOrEnd(nearestPieces, steps) ??
                        CheckGoOut(nearestPieces, steps);
                break;
        }

        return result ?? nearestPieces[0];
    }
    /*--------------------------------------------------------------------------------------------------------------------------------*/
    private static LudoPiece CheckKickOrEnd(List<LudoPiece> pieces, int steps)
    {
        foreach (LudoPiece piece in pieces)
        {
            Space currentSpace = piece.CurrentSpace;
            for (int i = 0; i < steps; i++)
            {
                currentSpace = currentSpace.NextSpace;
                Space spaceNext = currentSpace;
                // 吃子
                if (i == steps - 1 && spaceNext.CurrentPiece != null &&
                    !Tool.TurnToTeam(GameManager.Instance.CurrentPlayerTurn).GetAllPieces().Contains(spaceNext.CurrentPiece))
                {
                    return piece;
                }
                // 到終點
                if (i == steps - 1 && spaceNext.CurrentPiece == null &&
                    spaceNext.gameObject.IsInEndLayer())
                {
                    return piece;
                }
            }
        }
        return null;
    }

    private static LudoPiece CheckGoOut(List<LudoPiece> pieces, int steps)
    {
        if (steps == 6)
        {
            foreach (LudoPiece piece in pieces)
            {
                if (piece.CurrentSpace.gameObject.IsInHomeLayer())
                {
                    return piece;
                }
            }
        }
        return null;
    }

    private static LudoPiece CheckExplode(List<LudoPiece> pieces, int steps)
    {
        if (steps == 6)
        {
            foreach (LudoPiece piece in pieces)
            {
                if (piece.CurrentSpace.gameObject.IsInHomeLayer()) continue;

                if (piece.CurrentSpace.NextSpace.CurrentPiece != null ||
                    piece.CurrentSpace.PreviousSpace.CurrentPiece != null)
                {
                    return piece;
                }
            }
        }
        return null;
    }

    private static LudoPiece CheckPlantTree(List<LudoPiece> pieces)
    {
        foreach (LudoPiece piece in pieces)
        {
            Space nextSpace = piece.CurrentSpace.NextSpace;
            if (nextSpace.CurrentPiece != null || nextSpace.CurrentTree != null)
                return piece;
        }
        return null;
    }
}