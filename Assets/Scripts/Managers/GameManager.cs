using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using static Tool;
using static GameConstants;
// using System;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    public int CurrentPlayerTurn { get; private set; } = 1;

    public int HumanPlayers { get; private set; } = 0;
    public int AIPlayers { get; private set; } = 4;

    public bool IsDiceThrown { get; set; }
    public bool IsPieceMoved { get; set; }
    public int RollCount { get; set; }

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
        HumanPlayers = GetHumanPlayers();
        StartCoroutine(GameLoop());
        Time.timeScale = 3;
    }

    IEnumerator GameLoop()
    {
        yield return null;
        while (!isGameOver())
        {
            // hasn't already end
            if (!TurnToTeam(CurrentPlayerTurn).isFinished())
            {
                yield return null;
                yield return StartCoroutine(PlayerTurn(CurrentPlayerTurn));
            }
            ChangeTurn();
        }
    }

    private IEnumerator PlayerTurn(int playerIndex)
    {
        return playerIndex <= HumanPlayers ? HumanPlayerTurn() : AIPlayerTurn();
    }


    IEnumerator HumanPlayerTurn()
    {
        IsDiceThrown = false;
        IsPieceMoved = false;
        List<LudoPiece> allPieces = TurnToTeam(CurrentPlayerTurn).GetAllPieces();

        Debug.Log("請點擊滑鼠按鈕擲骰子");
        //等到玩家按骰子後，布林值會被刷新
        yield return new WaitUntil(() => !DiceManager.Instance.IsAnyDiceMoving && IsDiceThrown);

        //決定可以被按的棋子
        SelectClickablePiece(allPieces);

        //若全部都不行按
        if (allPieces.All(piece => !piece.IsClickable))
        {
            if (isTripleThrowScenario(allPieces) && RollCount < 2)
            {
                //就可以刷3次6
                RollCount++;
                yield return StartCoroutine(HumanPlayerTurn());
            }
            if (DiceManager.Instance.GetTotalDiceResult() == 6)
            {
                yield return StartCoroutine(HumanPlayerTurn());

            }
            yield break;
        }

        Debug.Log("請點擊滑鼠移動棋子");

        yield return new WaitUntil(() => IsPieceMoved);

        yield return new WaitForSeconds(1f);

        //若骰6則可以再一次
        if (DiceManager.Instance.GetTotalDiceResult() == 6)
            yield return StartCoroutine(HumanPlayerTurn());
        else
            yield return null;
    }

    IEnumerator AIPlayerTurn()
    {
        List<LudoPiece> allPieces = TurnToTeam(CurrentPlayerTurn).GetAllPieces();

        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        yield return StartCoroutine(DiceManager.Instance.AIRollDice(DiceManager.Instance.GetDice(0)));
        yield return new WaitUntil(() => !DiceManager.Instance.IsAnyDiceMoving);

        List<LudoPiece> availablePieces = SelectAvailablePiece(allPieces);


        if (availablePieces.Count == 0)
        {
            //如果大家都在家就可以刷3次6
            if (isTripleThrowScenario(allPieces) && RollCount < 2)
            {
                RollCount++;
                yield return StartCoroutine(AIPlayerTurn());
            }
            if (DiceManager.Instance.GetTotalDiceResult() == 6)
            {
                yield return StartCoroutine(AIPlayerTurn());

            }

            yield break;

        }
        LudoPiece selectedPiece;
        selectedPiece = LudoPieceManager.Instance.SelectPiece(TurnToTeam(CurrentPlayerTurn).GetStrategy(), availablePieces);

        if (selectedPiece != null)
        {
            yield return StartCoroutine(LudoPieceManager.Instance.AIMovePiece(selectedPiece));
        }

        availablePieces = null;

        if (DiceManager.Instance.GetTotalDiceResult() == 6)
            yield return StartCoroutine(AIPlayerTurn());
        else
            yield return null;
    }

    public void ChangeTurn()
    {

        CurrentPlayerTurn = (CurrentPlayerTurn % TotalPlayers) + 1;
        BackGroundManager.Instance.SetUpTurnFlag();
        RollCount = 0;
    }


    // 有3隊都達到終點時才算遊戲結束
    bool isGameOver()
    {
        List<Team> teamsToMove = new List<Team>();

        // 首先，找出所有已完成的隊伍
        foreach (Team team in LudoPieceManager.Instance.UnfinishedTeams)
        {
            if (team.isFinished())
            {
                teamsToMove.Add(team);
            }
        }

        // 然後，在單獨的循環中移動這些隊伍
        foreach (Team team in teamsToMove)
        {
            LudoPieceManager.Instance.FinishedTeams.Add(team);
            LudoPieceManager.Instance.UnfinishedTeams.Remove(team);
            AudioManager.Instance.PlaySFX("Finish");
        }



        if (LudoPieceManager.Instance.FinishedTeams.Count >= 3)
        {
            CurrentPlayerTurn = 0;
            return true;
        }
        return false;
    }
    int GetHumanPlayers()
    {
        int count = 0;
        foreach (var team in Team.AllTeams)
        {
            string str = team.GetStateString();
            if (str == "Player") count++;
        }
        return count;
    }
}