using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIStrategies : MonoBehaviour
{

    public delegate LudoPiece AIStrategy(List<LudoPiece> availablePieces);

    private static readonly Dictionary<TeamState, AIStrategy> StrategyMap = new Dictionary<TeamState, AIStrategy>
    {
        { TeamState.AI_Dumb, RandomSelect },
        { TeamState.AI_Peaceful, FrontmostFirst },
        { TeamState.AI_Aggressive, AttackFirst }
    };

    public static AIStrategy GetStrategy(TeamState teamState)
    {
        if (StrategyMap.TryGetValue(teamState, out var strategy))
        {
            return strategy;
        }
        return FrontmostFirst;
    }
    public static LudoPiece RandomSelect(List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;

        int randomIndex = Random.Range(0, pieces.Count);
        LudoPiece selectedPiece = pieces[randomIndex];
        return selectedPiece;
    }

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

    public static LudoPiece OrangeStrategy(List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;
        if (pieces.Count == 1) return pieces[0];

        int steps = DiceManager.Instance.GetCurrentDiceResult();
        // if movable chesses number is 2,3 or 4
        List<LudoPiece> nearestPieces = LudoPieceManager.Instance.GetNearestPiecesToFinish(pieces);

        // 1. Explode to impact other pieces
        foreach (LudoPiece piece in nearestPieces)
        {
            if (steps == 6 &&
            (piece.CurrentSpace.NextSpace.CurrentPiece != null ||
            piece.CurrentSpace.PreviousSpace.CurrentPiece != null))
            {
                return piece;
            }
        }

        // 2. Kick or Go to the end
        foreach (LudoPiece piece in nearestPieces)
        {
            Space currentSpace = piece.CurrentSpace;

            for (int i = 0; i < steps; i++)
            {
                currentSpace = currentSpace.NextSpace;
                Space spaceNext = currentSpace;
                if (i == steps - 1 && spaceNext.CurrentPiece != null && !Tool.TurnToTeam(GameManager.Instance.CurrentPlayerTurn).GetAllPieces().Contains(spaceNext.CurrentPiece))
                {
                    return piece;
                }
                if (i == steps - 1 && spaceNext.CurrentPiece == null && spaceNext.gameObject.IsInEndLayer())
                {
                    return piece;
                }
            }
        }

        // 3. Go Out
        foreach (LudoPiece piece in nearestPieces)
        {
            if (steps == 6 && piece.CurrentSpace.gameObject.IsInHomeLayer())
            {
                return piece;
            }
        }

        // 4. Move the frontmost
        return nearestPieces[0];
    }

    public static LudoPiece GreenStrategy(List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;
        if (pieces.Count == 1) return pieces[0];

        int steps = DiceManager.Instance.GetCurrentDiceResult();
        // if movable chesses number is 2,3 or 4
        List<LudoPiece> nearestPieces = LudoPieceManager.Instance.GetNearestPiecesToFinish(pieces);

        // 1. Plant a tree and kick someone
        foreach (LudoPiece piece in nearestPieces)
        {
            Space nextSpace = piece.CurrentSpace.NextSpace;
            if (nextSpace.CurrentPiece != null || nextSpace.CurrentTree != null) return piece;
        }

        // 2. Kick or Go to the end
        foreach (LudoPiece piece in nearestPieces)
        {
            Space currentSpace = piece.CurrentSpace;

            for (int i = 0; i < steps; i++)
            {
                currentSpace = currentSpace.NextSpace;
                Space spaceNext = currentSpace;
                if (i == steps - 1 && spaceNext.CurrentPiece != null && !Tool.TurnToTeam(GameManager.Instance.CurrentPlayerTurn).GetAllPieces().Contains(spaceNext.CurrentPiece))
                {
                    return piece;
                }
                if (i == steps - 1 && spaceNext.CurrentPiece == null && spaceNext.gameObject.IsInEndLayer())
                {
                    return piece;
                }
            }
        }

        // 3. Go Out
        foreach (LudoPiece piece in nearestPieces)
        {
            if (steps == 6 && piece.CurrentSpace.gameObject.IsInHomeLayer())
            {
                return piece;
            }
        }

        // 4. Move the frontmost
        return nearestPieces[0];
    }
    public static LudoPiece BlueStrategy(List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;
        if (pieces.Count == 1) return pieces[0];

        int steps = DiceManager.Instance.GetCurrentDiceResult();
        // if movable chesses number is 2,3 or 4
        List<LudoPiece> nearestPieces = LudoPieceManager.Instance.GetNearestPiecesToFinish(pieces);

        // 1. Move the frontmost
        if (steps == 6) return nearestPieces[0];

        // 2. Kick or Go to the end
        foreach (LudoPiece piece in nearestPieces)
        {
            Space currentSpace = piece.CurrentSpace;

            for (int i = 0; i < steps; i++)
            {
                currentSpace = currentSpace.NextSpace;
                Space spaceNext = currentSpace;
                if (i == steps - 1 && spaceNext.CurrentPiece != null && !Tool.TurnToTeam(GameManager.Instance.CurrentPlayerTurn).GetAllPieces().Contains(spaceNext.CurrentPiece))
                {
                    return piece;
                }
                if (i == steps - 1 && spaceNext.CurrentPiece == null && spaceNext.gameObject.IsInEndLayer())
                {
                    return piece;
                }
            }
        }

        // 3. Go Out
        foreach (LudoPiece piece in nearestPieces)
        {
            if (steps == 6 && piece.CurrentSpace.gameObject.IsInHomeLayer())
            {
                return piece;
            }
        }

        // 4. Move the frontmost
        return nearestPieces[0];
    }

    public static LudoPiece RedStrategy(List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;
        if (pieces.Count == 1) return pieces[0];

        int steps = DiceManager.Instance.GetCurrentDiceResult();
        // if movable chesses number is 2,3 or 4
        List<LudoPiece> nearestPieces = LudoPieceManager.Instance.GetNearestPiecesToFinish(pieces);

        // 1. Kick or Go to the end
        foreach (LudoPiece piece in nearestPieces)
        {
            Space currentSpace = piece.CurrentSpace;

            for (int i = 0; i < steps; i++)
            {
                currentSpace = currentSpace.NextSpace;
                Space spaceNext = currentSpace;
                if (i == steps - 1 && spaceNext.CurrentPiece != null && !Tool.TurnToTeam(GameManager.Instance.CurrentPlayerTurn).GetAllPieces().Contains(spaceNext.CurrentPiece))
                {
                    return piece;
                }
                if (i == steps - 1 && spaceNext.CurrentPiece == null && spaceNext.gameObject.IsInEndLayer())
                {
                    return piece;
                }
            }
        }

        // 3. Go Out
        foreach (LudoPiece piece in nearestPieces)
        {
            if (steps == 6 && piece.CurrentSpace.gameObject.IsInHomeLayer())
            {
                return piece;
            }
        }

        // 4. Move the frontmost
        return nearestPieces[0];
    }
    /*----------*/

    private static LudoPiece BasicCheck(List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;
        if (pieces.Count == 1) return pieces[0];
        return null;
    }

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

    // 橘色特有的爆炸檢查
    private static LudoPiece CheckExplode(List<LudoPiece> pieces, int steps)
    {
        if (steps == 6)
        {
            foreach (LudoPiece piece in pieces)
            {
                if (piece.CurrentSpace.NextSpace.CurrentPiece != null ||
                    piece.CurrentSpace.PreviousSpace.CurrentPiece != null)
                {
                    return piece;
                }
            }
        }
        return null;
    }

    // 綠色特有的種樹檢查
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

    public static LudoPiece CustomizedStrategy(Team team, List<LudoPiece> pieces)
    {
        // 基本檢查
        LudoPiece basicResult = BasicCheck(pieces);
        if (basicResult != null) return basicResult;

        int steps = DiceManager.Instance.GetCurrentDiceResult();
        List<LudoPiece> nearestPieces = LudoPieceManager.Instance.GetNearestPiecesToFinish(pieces);
        LudoPiece result = null;

        // 根據不同顏色執行不同策略
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
}