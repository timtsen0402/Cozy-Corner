using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static GameConstants;

public abstract class Team : MonoBehaviour
{
    public TeamState CurrentState { get; protected set; } = TeamState.Player;

    public string Name { get; protected set; }
    public string HexCode { get; protected set; }

    [Header("Basic Info")]
    [SerializeField] protected TeamData teamData;
    [field: SerializeField] public Space StartSpace { get; protected set; }

    protected List<LudoPiece> pieces = new List<LudoPiece>();


    protected virtual void Awake()
    {
        if (teamData != null)
        {
            InitializeTeam(teamData);
            CreatePieces();
        }
        else
        {
            Debug.LogError($"TeamData is not set for {gameObject.name}");
        }

        if (!AllTeams.Contains(this))
        {
            AllTeams.Add(this);
        }
    }

    protected virtual void InitializeTeam(TeamData data)
    {
        Name = data.teamName;
        HexCode = data.teamHexCode;
    }

    protected virtual void CreatePieces()
    {
        pieces = GetComponentsInChildren<LudoPiece>().ToList();
    }

    public virtual void ResetClickableState()
    {
        foreach (LudoPiece p in GetAllPieces())
        {
            p.IsClickable = false;
        }
    }

    // public virtual void CycleState()
    // {
    //     CurrentState = (TeamState)(((int)CurrentState + 1) % 4);
    // }
    public void CycleState(GameMode gameMode, Team team)
    {
        if (gameMode == GameMode.Classic)
        {
            // Classic 模式下只顯示基本 AI
            switch (CurrentState)
            {
                case TeamState.Player:
                    CurrentState = TeamState.AI_Dumb;
                    break;
                case TeamState.AI_Dumb:
                    CurrentState = TeamState.AI_Peaceful;
                    break;
                case TeamState.AI_Peaceful:
                    CurrentState = TeamState.AI_Aggressive;
                    break;
                default:
                    CurrentState = TeamState.Player;
                    break;
            }
        }
        else // Crazy 模式
        {
            // 根據顏色決定可用的 AI
            switch (CurrentState)
            {
                case TeamState.Player:
                    CurrentState = TeamState.AI_Dumb;
                    break;
                case TeamState.AI_Dumb:
                    switch (team)
                    {
                        case TeamOrange:
                            CurrentState = TeamState.AI_Orange;
                            break;
                        case TeamGreen:
                            CurrentState = TeamState.AI_Green;
                            break;
                        case TeamBlue:
                            CurrentState = TeamState.AI_Blue;
                            break;
                        case TeamRed:
                            CurrentState = TeamState.AI_Red;
                            break;
                    }
                    break;
                default:
                    CurrentState = TeamState.Player;
                    break;
            }
        }
    }

    #region Get
    public virtual string GetStateString()
    {
        return CurrentState == TeamState.Player ? "Player" : $"{CurrentState}";
    }

    public virtual List<LudoPiece> GetAllPieces()
    {
        return pieces;
    }

    public virtual List<LudoPiece> GetForemostPieces()
    {
        return pieces.OrderBy(piece => piece.GetDistanceToTheEnd()).ToList();
    }

    public virtual bool isFinished()
    {
        if (pieces == null) return false;

        return pieces.All(piece =>
            piece != null &&
            piece.CurrentSpace != null &&
            piece.CurrentSpace.gameObject != null &&
            piece.CurrentSpace.gameObject.IsInEndLayer());
    }

    public virtual TeamState GetStrategy()
    {
        return CurrentState;
    }

    public virtual int GetKillCount()
    {
        return pieces.Sum(piece => piece.killCount);
    }
    #endregion Get

    #region Reset
    public virtual void ResetPieces()
    {
        foreach (var piece in pieces)
        {
            piece.ResetToHome();
        }
    }
    #endregion Reset

    // customized special function
    public abstract void ActivateSpecialFunction(LudoPiece piece);


    public abstract void SetTeamStateDefaultClassic();
    public abstract void SetTeamStateDefaultCrazy();
}
