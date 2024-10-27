using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static GameConstants;

public abstract class Team : MonoBehaviour
{
    public TeamState CurrentState { get; protected set; } = TeamState.AI_Peaceful;
    public Difficulty Difficulty { get; protected set; } = Difficulty.Peaceful;

    public string Name { get; protected set; }
    public string HexCode { get; protected set; }

    [Header("Basic Info")]
    [SerializeField] protected TeamData teamData;
    [field: SerializeField] public Space StartSpace { get; protected set; }

    [Header("Effects")]
    [SerializeField] protected GameObject effect;

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

    protected virtual void Start()
    {
        Difficulty = Difficulty.Peaceful;
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

    public virtual void CycleState()
    {
        CurrentState = (TeamState)(((int)CurrentState + 1) % 4);
        UpdateAIDifficulty();
    }

    protected virtual void UpdateAIDifficulty()
    {
        switch (CurrentState)
        {
            case TeamState.AI_Dumb:
                Difficulty = Difficulty.Dumb;
                break;
            case TeamState.AI_Peaceful:
                Difficulty = Difficulty.Peaceful;
                break;
            case TeamState.AI_Aggressive:
                Difficulty = Difficulty.Aggressive;
                break;
            default:
                Difficulty = Difficulty.Peaceful;
                break;
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
        return pieces.All(piece => piece.CurrentSpace.gameObject.IsInEndLayer());
    }

    public virtual Difficulty GetStrategy()
    {
        return Difficulty;
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
}
