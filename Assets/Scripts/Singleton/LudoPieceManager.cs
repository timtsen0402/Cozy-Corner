using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using static Tool;
using Unity.VisualScripting;
using static GameConstants;

public class LudoPieceManager : MonoBehaviour
{
    public static LudoPieceManager Instance { get; private set; }
    public static Quaternion PieceRotation = Quaternion.Euler(new Vector3(270f, 0f, 0f));
    public List<Team> UnfinishedTeams { get; set; }
    public List<Team> FinishedTeams { get; set; }
    // 使用字典來按顏色存儲棋子
    private Dictionary<Team, List<LudoPiece>> piecesByTeam;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePieceDictionary();
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

    private void InitializePieceDictionary()
    {
        piecesByTeam = new Dictionary<Team, List<LudoPiece>>();
        foreach (Team team in AllTeams)
        {
            piecesByTeam[team] = new List<LudoPiece>();
        }
    }

    #region Get

    public LudoPiece SelectPiece(Difficulty difficulty, List<LudoPiece> availablePieces)
    {
        var strategy = AIStrategies.GetStrategy(difficulty);
        return strategy(availablePieces);
    }

    public List<LudoPiece> GetNearestPiecesToFinish(List<LudoPiece> pieces)
    {
        return pieces
           .OrderBy(piece => piece.GetDistanceToTheEnd())
           .ToList();
    }

    public string GetHexCode(Team team)
    {
        return team.HexCode;
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