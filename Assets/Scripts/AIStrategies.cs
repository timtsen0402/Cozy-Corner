using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIStrategies : MonoBehaviour
{
    public static LudoPiece RandomSelect(List<LudoPiece> pieces)
    {
        if (pieces.Count == 0) return null;

        int randomIndex = Random.Range(0, pieces.Count);
        LudoPiece selectedPiece = pieces[randomIndex];
        return selectedPiece;
    }
}

// TODO: AttackFirst, GoHomeFirst, ...etc

