using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIStrategies : MonoBehaviour
{

    public delegate LudoPiece AIStrategy(List<LudoPiece> availablePieces);

    // 使用字典來映射難度級別和相應的策略
    private static readonly Dictionary<Difficulty, AIStrategy> StrategyMap = new Dictionary<Difficulty, AIStrategy>
    {
        { Difficulty.Dumb, RandomSelect },
        { Difficulty.Peaceful, FrontmostFirst },
        { Difficulty.Aggressive, AttackFirst }
    };

    // 獲取指定難度的策略
    public static AIStrategy GetStrategy(Difficulty difficulty)
    {
        if (StrategyMap.TryGetValue(difficulty, out var strategy))
        {
            return strategy;
        }
        return AttackFirst; // 默認策略
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

    // 1.Kick 2.Go out from home 3.Go to the end 4.Move the frontmost
    public static LudoPiece AttackFirst(List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;
        if (pieces.Count == 1) return pieces[0];

        int steps = DiceManager.Instance.GetDiceResult(0);
        // if movable chesses number is 2,3 or 4
        List<LudoPiece> nearestPieces = LudoPieceManager.Instance.GetNearestPiecesToFinish(pieces);
        foreach (LudoPiece piece in nearestPieces)
        {
            Space currentSpace = piece.CurrentSpace;

            for (int i = 0; i < steps; i++)
            {
                currentSpace = currentSpace.NextSpace;
                Space spaceNext = currentSpace;
                // 1.kick
                if (i == steps - 1 && spaceNext.CurrentPiece != null && !Tool.TurnToTeam(GameManager.Instance.CurrentPlayerTurn).GetAllPieces().Contains(spaceNext.CurrentPiece))
                {
                    return piece;
                }
                // 2.Go Out
                if (steps == 6 && piece.CurrentSpace.gameObject.layer == 6)
                {
                    return piece;
                }
                // 3.Go to the end
                if (i == steps - 1 && spaceNext.CurrentPiece == null && spaceNext.gameObject.layer == 9)
                {
                    return piece;
                }
            }
        }
        // 4.Move the frontmost
        return nearestPieces[0];
    }
}
// TODO:  GoHomeFirst, ...etc

