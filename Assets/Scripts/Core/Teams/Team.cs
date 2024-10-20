// using UnityEngine;
// using UnityEditor;
// using System.Collections.Generic;
// using System.Linq;

// public class Team : MonoBehaviour
// {

//     public TeamState CurrentState { get; private set; } = TeamState.AI_Peaceful;

//     public static Team Orange { get; private set; }
//     public static Team Green { get; private set; }
//     public static Team Blue { get; private set; }
//     public static Team Red { get; private set; }

//     public static List<Team> AllTeams { get; private set; }

//     public string Name { get; private set; }
//     public string HexCode { get; private set; }

//     [SerializeField] private TeamData teamData;
//     public Difficulty Difficulty = Difficulty.Peaceful;
//     [field: SerializeField]
//     public Space StartSpace { get; private set; }
//     private List<LudoPiece> pieces = new List<LudoPiece>();

//     // 靜態構造函數
//     static Team()
//     {
//         AllTeams = new List<Team>();
//     }

//     private void Awake()
//     {
//         if (teamData != null)
//         {
//             InitializeTeam(teamData);
//             CreatePieces();
//         }
//         else
//         {
//             Debug.LogError($"TeamData is not set for {gameObject.name}");
//         }

//     }
//     private void Start()
//     {
//         Difficulty = Difficulty.Peaceful;
//     }

//     private void InitializeTeam(TeamData data)
//     {
//         Name = data.teamName;
//         HexCode = data.teamHexCode;

//         switch (Name)
//         {
//             case "Orange": Orange = this; break;
//             case "Green": Green = this; break;
//             case "Blue": Blue = this; break;
//             case "Red": Red = this; break;
//         }

//         if (!AllTeams.Contains(this))
//         {
//             AllTeams.Add(this);
//         }
//     }

//     private void CreatePieces()
//     {
//         pieces = GetComponentsInChildren<LudoPiece>().ToList();
//     }

//     public void CycleState()
//     {
//         CurrentState = (TeamState)(((int)CurrentState + 1) % 4);
//         UpdateAIDifficulty();
//     }

//     private void UpdateAIDifficulty()
//     {
//         switch (CurrentState)
//         {
//             case TeamState.AI_Dumb:
//                 Difficulty = Difficulty.Dumb;
//                 break;
//             case TeamState.AI_Peaceful:
//                 Difficulty = Difficulty.Peaceful;
//                 break;
//             case TeamState.AI_Aggressive:
//                 Difficulty = Difficulty.Aggressive;
//                 break;
//             default:
//                 Difficulty = Difficulty.Peaceful;
//                 break;
//         }
//     }
//     public string GetStateString()
//     {
//         return CurrentState == TeamState.Player ? "Player" : $"{CurrentState}";
//     }

//     //方法:得到所有這隊的棋子
//     public List<LudoPiece> GetAllPieces()
//     {
//         return pieces;
//     }
//     //方法:得到這隊最前面的棋子
//     public List<LudoPiece> GetForemostPieces()
//     {
//         return pieces.OrderBy(piece => piece.GetDistanceToTheEnd()).ToList();
//     }
//     // 方法:得到這隊是否已經結束
//     public bool isFinished()
//     {
//         if (!pieces.All(piece => piece.CurrentSpace.gameObject.layer == 9)) return false;
//         return true;
//     }

//     //方法:得到這隊的AI戰術
//     public Difficulty GetStrategy()
//     {
//         return Difficulty;
//     }

//     public int GetKillCount()
//     {
//         int totalKillCount = 0;
//         foreach (LudoPiece piece in pieces)
//         {
//             totalKillCount += piece.killCount;
//         }
//         return totalKillCount;
//     }

//     public void ResetPieces()
//     {
//         foreach (var piece in pieces)
//         {
//             piece.ResetToHome();
//         }
//     }

//     public static void LogAllTeamsStatus()
//     {
//         Debug.Log($"Total teams in AllTeams: {AllTeams.Count}");
//         foreach (var team in AllTeams)
//         {
//             Debug.Log($"Team: {team.Name}, Piece count: {team.pieces.Count}, Is finished: {team.isFinished()}");
//         }
//     }

//     public void ActivateOrangeSpecialFunction()
//     {
//         Debug.Log("Activating Orange team's special function");
//         // 實現橙色隊伍的特殊功能
//         // 例如：所有棋子向前移動一格
//     }

//     public void ActivateGreenSpecialFunction()
//     {
//         Debug.Log("Activating Green team's special function");
//         // 實現綠色隊伍的特殊功能
//         // 留下一個路障然後前進一格
//     }

//     public void ActivateBlueSpecialFunction()
//     {
//         Debug.Log("Activating Blue team's special function");
//         // 實現藍色隊伍的特殊功能
//         // 例如：獲得一次額外的骰子機會
//     }

//     public void ActivateRedSpecialFunction()
//     {
//         Debug.Log("Activating Red team's special function");
//         // 實現紅色隊伍的特殊功能
//         // 例如：所有棋子變得無敵，持續一回合
//     }
// }

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class Team : MonoBehaviour
{
    public TeamState CurrentState { get; protected set; } = TeamState.AI_Peaceful;

    public static List<Team> AllTeams { get; private set; } = new List<Team>();

    public string Name { get; protected set; }
    public string HexCode { get; protected set; }

    [Header("Basic Info")]
    [SerializeField] protected TeamData teamData;
    [field: SerializeField] public Space StartSpace { get; protected set; }

    [Header("Effects")]
    [SerializeField] protected GameObject effect;

    public Difficulty Difficulty { get; protected set; } = Difficulty.Peaceful;

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
        return pieces.All(piece => piece.CurrentSpace.gameObject.layer == 9);
    }

    public virtual Difficulty GetStrategy()
    {
        return Difficulty;
    }

    public virtual int GetKillCount()
    {
        return pieces.Sum(piece => piece.killCount);
    }

    public virtual void ResetPieces()
    {
        foreach (var piece in pieces)
        {
            piece.ResetToHome();
        }
    }

    public abstract void ActivateSpecialFunction(LudoPiece piece);

    public static void LogAllTeamsStatus()
    {
        Debug.Log($"Total teams in AllTeams: {AllTeams.Count}");
        foreach (var team in AllTeams)
        {
            Debug.Log($"Team: {team.Name}, Piece count: {team.GetAllPieces().Count}, Is finished: {team.isFinished()}");
        }
    }
}
