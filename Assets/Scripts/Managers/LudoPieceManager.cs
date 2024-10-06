using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using DG.Tweening;

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
        UnfinishedTeams = new List<Team>(Team.AllTeams);
        FinishedTeams = new List<Team>();

    }

    private void InitializePieceDictionary()
    {
        piecesByTeam = new Dictionary<Team, List<LudoPiece>>();
        foreach (Team team in Team.AllTeams)
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
        foreach (var team in Team.AllTeams)
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

    public IEnumerator AIMovePiece(LudoPiece piece)
    {
        piece.IsMoving = true;
        int steps = DiceManager.Instance.GetDiceResult(0);
        for (int i = 0; i < steps; i++)
        {
            Space currentSpace = piece.CheckCurrentSpace();

            if (currentSpace.NextSpace == null)
            {
                break;
            }

            Vector3 nextPosition;

            if (currentSpace.NextSpace == piece.startSpace)
            {
                if (piece.CheckCurrentSpace().gameObject.layer == 6)//Home
                {
                    nextPosition = piece.startSpace.ActualPosition;
                    yield return MoveToPosition(piece, nextPosition);
                    break;
                }
                else if (piece.CheckCurrentSpace().gameObject.layer == 8)//Path
                {
                    nextPosition = currentSpace.NextSpace2.ActualPosition;

                }
                else
                {
                    nextPosition = currentSpace.NextSpace.ActualPosition;
                }
            }
            else
            {
                nextPosition = currentSpace.NextSpace.ActualPosition;
            }
            yield return MoveToPosition(piece, nextPosition);

            // 短暂暂停，让玩家能看清每一步的移动
            yield return new WaitForSeconds(0.1f);

        }
        piece.IsMoving = false;
        GameManager.Instance.IsPieceMoved = true;
    }
    #endregion Move
}