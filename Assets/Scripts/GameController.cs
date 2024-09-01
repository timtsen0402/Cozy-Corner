using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using static Rotate;
using static Tool;
using static ToolSingleton;

public class GameController : MonoBehaviour
{
    #region chess objects
    public static GameObject chessO1;
    public static GameObject chessO2;
    public static GameObject chessO3;
    public static GameObject chessO4;

    public static GameObject chessG1;
    public static GameObject chessG2;
    public static GameObject chessG3;
    public static GameObject chessG4;

    public static GameObject chessB1;
    public static GameObject chessB2;
    public static GameObject chessB3;
    public static GameObject chessB4;

    public static GameObject chessR1;
    public static GameObject chessR2;
    public static GameObject chessR3;
    public static GameObject chessR4;
    #endregion



    public static GameObject dice;
    public static Text hint;
    public static Text turnText;

    public static int numberOfPlayers = 4;
    public static int currentPlayerTurn = 1;

    public float delayBetweenTurns = 1f;
    public float diceRollDuration = 2f;

    public static GameObject selectedChess;
    public static bool isDiceThrown = false;
    public static bool isChessMoved = false;
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
        isChessMoved = false;
        GameObject[] all_chesses = TurnToTeam(currentPlayerTurn);

        Debug.Log("請點擊滑鼠按鈕擲骰子");
        //等到玩家按骰子後，布林值會被刷新
        yield return new WaitUntil(() => StopDetermination().isStop && isDiceThrown);

        //決定可以被按的棋子
        SelectClickableChess(all_chesses);

        //若全部都不行按
        if (all_chesses.All(chess => !chess.GetComponent<Chess>().isClickable))
        {
            if (isTripleThrowScenario(all_chesses) && rollCount < 2)
            {
                //就可以刷3次6
                rollCount++;
                yield return StartCoroutine(HumanPlayerTurn());
            }
            if (GetLatestDiceResult() == 6)
            {
                yield return StartCoroutine(HumanPlayerTurn());

            }
            yield break;
        }

        Debug.Log("請點擊滑鼠移動棋子");

        yield return new WaitUntil(() => isChessMoved);

        foreach (GameObject chess in all_chesses)
        {
            chess.GetComponent<Chess>().isClickable = false;
        }

        yield return new WaitForSeconds(1f);

        //若骰6則可以再一次
        if (GetLatestDiceResult() == 6)
            yield return StartCoroutine(HumanPlayerTurn());
        else
            yield return null;
    }

    IEnumerator AIPlayerTurn()
    {
        GameObject[] all_chesses = TurnToTeam(currentPlayerTurn);

        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        yield return StartCoroutine(AIRollDice());
        yield return new WaitUntil(() => StopDetermination().isStop);

        List<GameObject> availableChess = SelectAvailableChess(all_chesses);


        if (availableChess.Count == 0)
        {
            //如果大家都在家就可以刷3次6
            if (isTripleThrowScenario(all_chesses) && rollCount < 2)
            {
                rollCount++;
                yield return StartCoroutine(AIPlayerTurn());
            }
            if (GetLatestDiceResult() == 6)
            {
                yield return StartCoroutine(AIPlayerTurn());

            }

            yield break;

        }
        GameObject selectedChess = ALGO_SelectRandomClickableChess(availableChess);

        if (selectedChess != null)
        {
            yield return StartCoroutine(Instance.AIMoveChess(selectedChess));
        }

        availableChess = null;

        if (GetLatestDiceResult() == 6)
            yield return StartCoroutine(AIPlayerTurn());
        else
            yield return null;
    }


    IEnumerator AIRollDice()
    {
        float elapsedTime = 0f;
        diceRollDuration = Random.Range(1.5f, 3.5f);
        dice.transform.rotation = new Quaternion(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90), 0);
        while (elapsedTime < diceRollDuration)
        {
            dice.transform.position = rotatingPos;
            dice.transform.Rotate(rotatingSpeed);
            elapsedTime += 0.01f;
            yield return null;
        }
    }
    GameObject ALGO_SelectRandomClickableChess(List<GameObject> chesses)
    {
        // 如果有可點擊的棋子，隨機選擇一個
        if (chesses.Count > 0)
        {
            int randomIndex = Random.Range(0, chesses.Count);
            GameObject selectedChess = chesses[randomIndex];
            return selectedChess;
        }
        return null;
    }


    private void Update()
    {
        turnText.text = $"Player : {currentPlayerTurn}";
        hint.text = $"Dice : {dice_result}";

    }
    public static void ChangeTurn()
    {
        currentPlayerTurn = (currentPlayerTurn % numberOfPlayers) + 1;
        // isDiceThrown = false;
        // isChessMoved = false;
        rollCount = 0;
    }
    void Loading()
    {
        chessO1 = GameObject.Find("Chesses/Team Orange/Chess (1)");
        chessO2 = GameObject.Find("Chesses/Team Orange/Chess (2)");
        chessO3 = GameObject.Find("Chesses/Team Orange/Chess (3)");
        chessO4 = GameObject.Find("Chesses/Team Orange/Chess (4)");

        chessR1 = GameObject.Find("Chesses/Team Red/Chess (1)");
        chessR2 = GameObject.Find("Chesses/Team Red/Chess (2)");
        chessR3 = GameObject.Find("Chesses/Team Red/Chess (3)");
        chessR4 = GameObject.Find("Chesses/Team Red/Chess (4)");

        chessB1 = GameObject.Find("Chesses/Team Blue/Chess (1)");
        chessB2 = GameObject.Find("Chesses/Team Blue/Chess (2)");
        chessB3 = GameObject.Find("Chesses/Team Blue/Chess (3)");
        chessB4 = GameObject.Find("Chesses/Team Blue/Chess (4)");

        chessG1 = GameObject.Find("Chesses/Team Green/Chess (1)");
        chessG2 = GameObject.Find("Chesses/Team Green/Chess (2)");
        chessG3 = GameObject.Find("Chesses/Team Green/Chess (3)");
        chessG4 = GameObject.Find("Chesses/Team Green/Chess (4)");

        dice = GameObject.Find("dice/Cube");
        hint = GameObject.Find("Hint").GetComponent<Text>();
        turnText = GameObject.Find("Turn").GetComponent<Text>();
    }
}
