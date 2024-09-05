using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using static Tool;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [SerializeField] private Text hintText;
    [SerializeField] private Text turnText;

    public int NumberOfPlayers { get; private set; } = 4;
    public int CurrentPlayerTurn { get; private set; } = 1;

    [SerializeField] private float delayBetweenTurns = 1f;
    // [SerializeField] private float diceRollDuration = 2f;

    // public GameObject SelectedPiece { get; set; }
    public bool IsDiceThrown { get; set; }
    public bool IsPieceMoved { get; set; }
    public int RollCount { get; set; }

    public int HumanPlayers { get; private set; }
    public int AIPlayers { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Loading();
        HumanPlayers = PlayerPrefs.GetInt("HumanPlayers");
        AIPlayers = PlayerPrefs.GetInt("ComputerPlayers");

        hintText.text = "Roll";
        StartCoroutine(GameLoop());
        Time.timeScale = 2;
        AudioManager.Instance.PlayBgmRandomly();
    }

    IEnumerator GameLoop()
    {
        while (!isGameOver())
        {
            yield return StartCoroutine(PlayerTurn(CurrentPlayerTurn));
            ChangeTurn();
            yield return new WaitForSeconds(delayBetweenTurns);
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
        List<LudoPiece> allPieces = TurnToTeam(CurrentPlayerTurn);

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

        foreach (LudoPiece piece in allPieces)
        {
            piece.GetComponent<LudoPiece>().IsClickable = false;
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
        List<LudoPiece> allPieces = TurnToTeam(CurrentPlayerTurn);

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
        LudoPiece selectedPiece = AIStrategies.RandomSelect(availablePieces);

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

    private void UpdateUI()
    {
        turnText.text = $"Player : {CurrentPlayerTurn}";
        hintText.text = $"Dice : {DiceManager.Instance.GetTotalDiceResult()}";
    }

    private void Update()
    {
        UpdateUI();
    }
    public void ChangeTurn()
    {
        CurrentPlayerTurn = (CurrentPlayerTurn % NumberOfPlayers) + 1;
        RollCount = 0;
    }
    void Loading()
    {
        hintText = GameObject.Find("Hint").GetComponent<Text>();
        turnText = GameObject.Find("Turn").GetComponent<Text>();
    }
    bool isGameOver()
    {

        if (TurnToTeam(1).All(piece => piece.CheckCurrentSpace().gameObject.layer == 9))
        {
            Debug.Log($"Winner is team 1");
            return true;
        }
        else if (TurnToTeam(2).All(piece => piece.CheckCurrentSpace().gameObject.layer == 9))
        {
            Debug.Log($"Winner is team 2");
            return true;
        }
        else if (TurnToTeam(3).All(piece => piece.CheckCurrentSpace().gameObject.layer == 9))
        {
            Debug.Log($"Winner is team 3");
            return true;
        }
        else if (TurnToTeam(4).All(piece => piece.CheckCurrentSpace().gameObject.layer == 9))
        {
            Debug.Log($"Winner is team 4");
            return true;
        }
        else
        {
            return false;
        }
    }
}
