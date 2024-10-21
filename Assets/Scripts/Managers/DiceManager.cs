using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static GameConstants;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance { get; private set; }

    public bool IsAnyDiceMoving { get; private set; }

    [SerializeField]
    private List<GameObject> allDiceObjects = new List<GameObject>();
    private List<Dice> diceScripts = new List<Dice>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SetAllDices();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        IsAnyDiceMoving = CheckIfAnyDiceMoving();
    }

    public bool CheckIfAnyDiceMoving()
    {
        foreach (var dice in diceScripts)
        {
            if (!dice.isStop())
            {
                return true;
            }
        }
        return false;
    }

    public void SetAllDices()
    {
        for (int i = 0; i < allDiceObjects.Count; i++)
        {
            GameObject dice = Instantiate(allDiceObjects[i], DiceSleepingPositions[i], allDiceObjects[i].transform.rotation);
            diceScripts.Add(dice.GetComponent<Dice>());
        }
    }


    #region Get
    private Dice GetDice(int index)
    {
        if (index >= 0 && index < diceScripts.Count)
        {
            return diceScripts[index];
        }
        Debug.LogWarning($"Dice index {index} is out of range.");
        return null;
    }

    private int GetDiceResult(int index)
    {
        Dice dice = GetDice(index);
        return dice != null ? dice.GetLatestDiceResult() : 0;
    }

    public Dice GetCurrentDice()
    {
        return GameManager.Instance.CurrentGameMode == GameMode.Classic
            ? GetDice(0)
            : GetDice(GameManager.Instance.CurrentPlayerTurn);
    }

    public int GetCurrentDiceResult()
    {
        return GameManager.Instance.CurrentGameMode == GameMode.Classic
            ? GetDiceResult(0)
            : GetDiceResult(GameManager.Instance.CurrentPlayerTurn);
    }

    #endregion Get

    #region Reset
    public void ResetDice(int index)
    {
        Dice dice = GetDice(index);
        if (dice != null)
        {
            dice.ResetPosition();
            dice.transform.eulerAngles = DiceRotation;
        }
    }

    #endregion Reset



    #region Roll

    public IEnumerator AIRollDice(Dice dice)
    {
        float elapsedTime = 0f;
        float diceRollDuration = Random.Range(1.5f, 3f);

        dice.transform.rotation = Random.rotation;

        while (elapsedTime < diceRollDuration)
        {
            dice.transform.position = DiceRotatingPos;
            dice.transform.Rotate(DiceRotatingSpeed);

            elapsedTime += 0.01f;
            yield return null;
        }
    }

    #endregion Roll

}

