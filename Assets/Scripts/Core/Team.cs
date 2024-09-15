using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class Team : MonoBehaviour
{

    public TeamState CurrentState { get; private set; } = TeamState.AI_Peaceful;

    public static Team Orange { get; private set; }
    public static Team Green { get; private set; }
    public static Team Blue { get; private set; }
    public static Team Red { get; private set; }

    public static List<Team> AllTeams { get; private set; }

    public string Name { get; private set; }
    public string HexCode { get; private set; }

    [SerializeField] private TeamData teamData;
    public Difficulty Difficulty = Difficulty.Peaceful;
    [field: SerializeField]
    public Space StartSpace { get; private set; }
    private List<LudoPiece> pieces = new List<LudoPiece>();

    // 靜態構造函數
    static Team()
    {
        AllTeams = new List<Team>();
    }

    private void Awake()
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

    }
    private void Start()
    {
        Difficulty = Difficulty.Peaceful;
    }

    private void InitializeTeam(TeamData data)
    {
        Name = data.teamName;
        HexCode = data.teamHexCode;

        switch (Name)
        {
            case "Orange": Orange = this; break;
            case "Green": Green = this; break;
            case "Blue": Blue = this; break;
            case "Red": Red = this; break;
        }

        if (!AllTeams.Contains(this))
        {
            AllTeams.Add(this);
        }
    }

    private void CreatePieces()
    {
        pieces = GetComponentsInChildren<LudoPiece>().ToList();
    }

    public void CycleState()
    {
        CurrentState = (TeamState)(((int)CurrentState + 1) % 4);
        UpdateAIDifficulty();
    }

    private void UpdateAIDifficulty()
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
    public string GetStateString()
    {
        return CurrentState == TeamState.Player ? "Player" : $"{CurrentState}";
    }

    //方法:得到所有這隊的棋子
    public List<LudoPiece> GetAllPieces()
    {
        return pieces;
    }
    //方法:得到這隊最前面的棋子
    public List<LudoPiece> GetForemostPieces()
    {
        return pieces.OrderBy(piece => piece.GetDistanceToTheEnd()).ToList();
    }
    // 方法:得到這隊是否已經結束
    public bool isFinished()
    {
        if (!pieces.All(piece => piece.CurrentSpace.gameObject.layer == 9)) return false;
        return true;
    }
    // public bool isFinished()
    // {
    //     if (pieces == null || pieces.Count == 0)
    //     {
    //         Debug.LogWarning($"Team {Name}: pieces 集合為空或為 null");
    //         return false;
    //     }

    //     foreach (var piece in pieces)
    //     {
    //         if (piece == null)
    //         {
    //             Debug.LogError($"Team {Name}: 在 pieces 集合中發現 null 的 piece");
    //             return false;
    //         }

    //         Space currentSpace = piece.CheckCurrentSpace();
    //         if (currentSpace == null)
    //         {
    //             Debug.LogWarning($"Team {Name}: Piece {piece.name} 的 CheckCurrentSpace() 返回 null");
    //             return false;
    //         }

    //         if (currentSpace.gameObject == null)
    //         {
    //             Debug.LogError($"Team {Name}: Piece {piece.name} 的 currentSpace.gameObject 為 null");
    //             return false;
    //         }

    //         if (currentSpace.gameObject.layer != 9)
    //         {
    //             Debug.Log($"Team {Name}: Piece {piece.name} 不在正確的 layer 上。當前 layer: {currentSpace.gameObject.layer}");
    //             return false;
    //         }
    //     }

    //     return true;
    // }
    //方法:得到這隊的AI戰術
    public Difficulty GetStrategy()
    {
        return Difficulty;
    }

    public int GetKillCount()
    {
        int totalKillCount = 0;
        foreach (LudoPiece piece in pieces)
        {
            totalKillCount += piece.killCount;
        }
        return totalKillCount;
    }

    public void ResetPieces()
    {
        foreach (var piece in pieces)
        {
            piece.ResetToHome();
        }
    }

    public static void LogAllTeamsStatus()
    {
        Debug.Log($"Total teams in AllTeams: {AllTeams.Count}");
        foreach (var team in AllTeams)
        {
            Debug.Log($"Team: {team.Name}, Piece count: {team.pieces.Count}, Is finished: {team.isFinished()}");
        }
    }
}
