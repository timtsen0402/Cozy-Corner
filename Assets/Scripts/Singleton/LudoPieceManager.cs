using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using static Tool;
using static GameConstants;

public class LudoPieceManager : MonoBehaviour
{
    public static LudoPieceManager Instance { get; private set; }

    [field: SerializeField]
    public List<Team> AllTeams { get; private set; } = new List<Team>();

    public List<Team> UnfinishedTeams { get; set; }
    public List<Team> FinishedTeams { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        UnfinishedTeams = new List<Team>(AllTeams);
        FinishedTeams = new List<Team>();
    }

    public LudoPiece SelectPiece(TeamState teamState, List<LudoPiece> availablePieces)
    {
        var strategy = AIStrategies.GetStrategy(teamState);
        return strategy(availablePieces);
    }

    #region Get
    public List<LudoPiece> GetNearestPiecesToFinish(List<LudoPiece> pieces)
    {
        return pieces
           .OrderBy(piece => piece.GetDistanceToTheEnd())
           .ToList();
    }

    public List<Team> GetFinishedTeam()
    {
        List<Team> teams = new List<Team>();
        foreach (var team in AllTeams)
        {
            if (team.isFinished()) teams.Add(team);
        }
        return teams;
    }
    #endregion Get

    #region Move
    public IEnumerator MoveToPosition(LudoPiece piece, Vector3 targetPosition, float speed = 0.1f)
    {
        Tween moveTween = piece.transform.DOMove(targetPosition, speed).SetEase(Ease.OutQuad);
        AudioManager.Instance.PlaySFX("PieceWalk");

        yield return moveTween.WaitForCompletion();
    }

    public IEnumerator MovePiece(LudoPiece piece, int steps)
    {
        piece.IsMoving = true;
        // Special Move
        //
        if (GameManager.Instance.CurrentGameMode == GameMode.Crazy && steps == 6)
        {
            Space currentSpace = piece.GetCurrentSpace();
            //如果被選到的棋在家就出來 否則執行以下
            if (currentSpace.NextSpace == piece.startSpace && currentSpace.gameObject.IsInHomeLayer())
            {
                Vector3 nextPosition;
                nextPosition = piece.startSpace.ActualPosition;
                yield return MoveToPosition(piece, nextPosition);
                yield break;
            }
            TurnToTeam(GameManager.Instance.CurrentPlayerTurn).ActivateSpecialFunction(piece);
            yield break;
        }
        //
        for (int i = 0; i < steps; i++)
        {
            Space currentSpace = piece.GetCurrentSpace();

            if (currentSpace.NextSpace == null)
            {
                break;
            }

            Vector3 nextPosition;

            if (currentSpace.NextSpace == piece.startSpace)
            {
                if (piece.GetCurrentSpace().gameObject.IsInHomeLayer())//Home
                {
                    nextPosition = piece.startSpace.ActualPosition;
                    yield return MoveToPosition(piece, nextPosition);
                    break;
                }
                else //Path
                {
                    nextPosition = currentSpace.NextSpace2.ActualPosition;
                }
            }
            // other situations
            else
            {
                nextPosition = currentSpace.NextSpace.ActualPosition;
            }
            yield return MoveToPosition(piece, nextPosition);

            yield return new WaitForSeconds(0.3f);

        }

        // after moving
        if (piece.CurrentSpace.CurrentTree != null)
        {
            Destroy(piece.CurrentSpace.CurrentTree);
            // TODO: SE
        }

        piece.IsMoving = false;
    }
    #endregion Move
}