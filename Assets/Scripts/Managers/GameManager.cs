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

    public GameMode CurrentGameMode { get; set; }

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
        AudioManager.Instance.PlayBgmRandomly();
        CameraManager.Instance.SetInitialCameraPosition(TitleView);

        Time.timeScale = 3f;
    }

    public void START_GAME()
    {
        HumanPlayers = GetHumanPlayers();
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        yield return null;
        while (!isGameOver() && UIManager.Instance.gameStarted)
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

        Debug.Log("Please click the dice");

        // wait until player roll the dice
        yield return new WaitUntil(() => IsDiceThrown && !DiceManager.Instance.IsAnyDiceMoving);

        // check all clicable pieces
        SelectClickablePiece(allPieces);

        // if all pieces are unclickable
        if (allPieces.All(piece => !piece.IsClickable))
        {
            if (isTripleThrowScenario(allPieces) && RollCount < 2)
            {
                RollCount++;
                yield return StartCoroutine(HumanPlayerTurn());
            }
            if (DiceManager.Instance.GetCurrentDiceResult() == 6)
            {
                yield return StartCoroutine(HumanPlayerTurn());

            }
            yield break;
        }

        Debug.Log("請點擊滑鼠移動棋子");

        // wait until player move the piece
        yield return new WaitUntil(() => IsPieceMoved);

        yield return new WaitForSeconds(1f);

        //若骰6則可以再一次
        if (DiceManager.Instance.GetCurrentDiceResult() == 6)
            yield return StartCoroutine(HumanPlayerTurn());
        else
            yield return null;
    }

    IEnumerator AIPlayerTurn()
    {
        List<LudoPiece> allPieces = TurnToTeam(CurrentPlayerTurn).GetAllPieces();

        yield return new WaitForSeconds(Random.Range(0.5f, 1f));

        yield return StartCoroutine(DiceManager.Instance.AIRollDice(DiceManager.Instance.GetCurrentDice()));

        yield return new WaitUntil(() => !DiceManager.Instance.IsAnyDiceMoving);
        yield return new WaitForSeconds(1f);

        List<LudoPiece> availablePieces = SelectAvailablePiece(allPieces);


        if (availablePieces.Count == 0)
        {
            //如果大家都在家就可以刷3次6
            if (isTripleThrowScenario(allPieces) && RollCount < 2)
            {
                RollCount++;
                yield return StartCoroutine(AIPlayerTurn());
            }
            if (DiceManager.Instance.GetCurrentDiceResult() == 6)
            {
                yield return StartCoroutine(AIPlayerTurn());
            }

            yield break;

        }
        LudoPiece selectedPiece;
        selectedPiece = LudoPieceManager.Instance.SelectPiece(TurnToTeam(CurrentPlayerTurn).GetStrategy(), availablePieces);

        // move piece
        if (selectedPiece != null)
        {
            yield return StartCoroutine(LudoPieceManager.Instance.AIMovePiece(selectedPiece, DiceManager.Instance.GetCurrentDiceResult()));

            if (TurnToTeam(CurrentPlayerTurn) == TeamRed.Instance && isMovePossible(selectedPiece, TeamRed.Instance.ExtraSteps))
            {
                yield return new WaitForSeconds(1f);
                StartCoroutine(LudoPieceManager.Instance.AIMovePiece(selectedPiece, TeamRed.Instance.ExtraSteps));
            }
        }

        // Reset
        availablePieces = null;

        if (DiceManager.Instance.GetCurrentDiceResult() == 6)
            yield return StartCoroutine(AIPlayerTurn());
        else
            yield return null;
    }

    public void ChangeTurn()
    {

        CurrentPlayerTurn = (CurrentPlayerTurn % TotalPlayers) + 1;
        UIManager.Instance.SetUpTurnFlag();
        RollCount = 0;

        if (CurrentGameMode == GameMode.Classic) return;

        // Reset last dices
        if (CurrentPlayerTurn == 1)
        {
            DiceManager.Instance.ResetDice(4);
        }
        else
        {
            DiceManager.Instance.ResetDice(CurrentPlayerTurn - 1);
        }
        //

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
        foreach (var team in AllTeams)
        {
            string str = team.GetStateString();
            if (str == "Player") count++;
        }
        return count;
    }
}