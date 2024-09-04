using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStrategies : MonoBehaviour
{
    public static LudoPiece RandomSelect(List<LudoPiece> pieces)
    {
        // 如果有可點擊的棋子，隨機選擇一個
        if (pieces.Count > 0)
        {
            int randomIndex = Random.Range(0, pieces.Count);
            LudoPiece selectedPiece = pieces[randomIndex];
            return selectedPiece;
        }
        return null;
    }

    // TODO: AttackFirst, GoHomeFirst, ...etc
}
