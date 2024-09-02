using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using static Dice;
using static Tool;
using static ToolSingleton;

public class GameController : MonoBehaviour
{

    public static Text hint;
    public static Text turnText;

    public static int numberOfPlayers = 4;
    public static int currentPlayerTurn = 1;

    public float delayBetweenTurns = 1f;
    public float diceRollDuration = 2f;

    public static GameObject selectedChess;
    public static bool isDiceThrown = false;
    public static bool isPieceMoved = false;
    // public static bool isNextRoll = false;
    public static int rollCount = 0;

    public static int humanPlayers;
    public static int AIPlayers;

    private void Start()
    {
        Loading();
        humanPlayers = PlayerPrefs.GetInt("HumanPlayers");
        AIPlayers = PlayerPrefs.GetInt("ComputerPlayers");

        hint.text = "Roll";
        StartCoroutine(GameLoop());
        Time.timeScale = 2;
    }

    IEnumerator GameLoop()
    {
        while (true)
        {
            yield return StartCoroutine(PlayerTurn(currentPlayerTurn));

            if (isGameOver()) break;

            ChangeTurn();

            yield return new WaitForSeconds(delayBetweenTurns);
        }
    }

    IEnumerator PlayerTurn(int playerIndex)
    {
        switch (humanPlayers)
        {
            case 0:
                yield return StartCoroutine(AIPlayerTurn());
                break;
            case 1:
                if (playerIndex == 1)
                {
                    // 人類玩家的回合，等待輸入
                    yield return StartCoroutine(HumanPlayerTurn());
                }
                else
                {
                    yield return StartCoroutine(AIPlayerTurn());
                }
                break;
            case 2:
                if (playerIndex == 1 || playerIndex == 2)
                {
                    // 人類玩家的回合，等待輸入
                    yield return StartCoroutine(HumanPlayerTurn());
                }
                else
                {
                    yield return StartCoroutine(AIPlayerTurn());
                }
                break;
            case 3:
                if (playerIndex == 1 || playerIndex == 2 || playerIndex == 3)
                {
                    // 人類玩家的回合，等待輸入
                    yield return StartCoroutine(HumanPlayerTurn());
                }
                else
                {
                    yield return StartCoroutine(AIPlayerTurn());
                }
                break;
            case 4:
                yield return StartCoroutine(HumanPlayerTurn());
                break;

        }
    }

    IEnumerator HumanPlayerTurn()
    {
        isDiceThrown = false;
        isPieceMoved = false;
        List<LudoPiece> allPieces = TurnToTeam(currentPlayerTurn);

        Debug.Log("請點擊滑鼠按鈕擲骰子");
        //等到玩家按骰子後，布林值會被刷新
        yield return new WaitUntil(() => !DiceManager.Instance.IsAnyDiceMoving && isDiceThrown);

        //決定可以被按的棋子
        SelectClickableChess(allPieces);

        //若全部都不行按
        if (allPieces.All(chess => !chess.GetComponent<LudoPiece>().isClickable))
        {
            if (isTripleThrowScenario(allPieces) && rollCount < 2)
            {
                //就可以刷3次6
                rollCount++;
                yield return StartCoroutine(HumanPlayerTurn());
            }
            if (DiceManager.Instance.GetTotalDiceResult() == 6)
            {
                yield return StartCoroutine(HumanPlayerTurn());

            }
            yield break;
        }

        Debug.Log("請點擊滑鼠移動棋子");

        yield return new WaitUntil(() => isPieceMoved);

        foreach (LudoPiece piece in allPieces)
        {
            piece.GetComponent<LudoPiece>().isClickable = false;
        }

        yield return new WaitForSeconds(1f);

        //若骰6則可以再一次
        if (DiceManager.Instance.GetTotalDiceResult() == 6)
            yield return StartCoroutine(HumanPlayerTurn());
        else
            yield return null;
    }

    IEnumerator AIPlayerTurn()
    {
        List<LudoPiece> allPieces = TurnToTeam(currentPlayerTurn);

        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        yield return StartCoroutine(AIRollDice());
        yield return new WaitUntil(() => !DiceManager.Instance.IsAnyDiceMoving);

        List<LudoPiece> availableChess = SelectAvailableChess(allPieces);


        if (availableChess.Count == 0)
        {
            //如果大家都在家就可以刷3次6
            if (isTripleThrowScenario(allPieces) && rollCount < 2)
            {
                rollCount++;
                yield return StartCoroutine(AIPlayerTurn());
            }
            if (DiceManager.Instance.GetTotalDiceResult() == 6)
            {
                yield return StartCoroutine(AIPlayerTurn());

            }

            yield break;

        }
        LudoPiece selectedPiece = ALGO_SelectRandomClickablePiece(availableChess);

        if (selectedPiece != null)
        {
            yield return StartCoroutine(Instance.AIMoveChess(selectedPiece));
        }

        availableChess = null;

        if (DiceManager.Instance.GetTotalDiceResult() == 6)
            yield return StartCoroutine(AIPlayerTurn());
        else
            yield return null;
    }


    IEnumerator AIRollDice()
    {
        float elapsedTime = 0f;
        diceRollDuration = Random.Range(1.5f, 3.5f);
        Dice dice = DiceManager.Instance.GetDice(0);
        dice.transform.rotation = new Quaternion(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90), 0);
        while (elapsedTime < diceRollDuration)
        {
            dice.ResetPosition();
            dice.transform.Rotate(dice.rotatingSpeed);
            elapsedTime += 0.01f;
            yield return null;
        }
    }
    LudoPiece ALGO_SelectRandomClickablePiece(List<LudoPiece> pieces)
    {
        // 如果有可點擊的棋子，隨機選擇一個
        if (pieces.Count > 0)
        {
            int randomIndex = Random.Range(0, pieces.Count);
            LudoPiece selectedPiece = pieces[randomIndex];
            return selectedPiece;
        }
        return null;
    }


    private void Update()
    {
        turnText.text = $"Player : {currentPlayerTurn}";
        hint.text = $"Dice : {DiceManager.Instance.GetTotalDiceResult()}";

    }
    public static void ChangeTurn()
    {
        currentPlayerTurn = (currentPlayerTurn % numberOfPlayers) + 1;
        // isDiceThrown = false;
        // isPieceMoved = false;
        rollCount = 0;
    }
    void Loading()
    {
        hint = GameObject.Find("Hint").GetComponent<Text>();
        turnText = GameObject.Find("Turn").GetComponent<Text>();
    }
}
