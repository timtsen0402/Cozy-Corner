using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static GameConstants;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance { get; private set; }

    public List<GameObject> AllDiceObjects = new List<GameObject>();
    public bool IsAnyDiceMoving { get; private set; }

    public static float VelocityThreshold = 0.01f;
    public static float AngularVelocityThreshold = 0.01f;
    public static float SettleTime = 0.1f;

    private List<Dice> diceScripts = new List<Dice>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
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
            if (dice == null) return false;
            if (!dice.isStop())
            {
                return true;
            }
        }
        return false;
    }

    public void SetAllDices()
    {
        for (int i = 0; i < AllDiceObjects.Count; i++)
        {
            GameObject dice = Instantiate(AllDiceObjects[i], DiceSleepingPositions[i], AllDiceObjects[i].transform.rotation);
            diceScripts.Add(dice.GetComponent<Dice>());
        }
    }


    #region Get
    // 新增方法：獲取指定索引的骰子
    public Dice GetDice(int index)
    {
        if (index >= 0 && index < diceScripts.Count)
        {
            return diceScripts[index];
        }
        Debug.LogWarning($"Dice index {index} is out of range.");
        return null;
    }

    // 新增方法：獲取指定骰子的結果
    public int GetDiceResult(int index)
    {
        Dice dice = GetDice(index);
        return dice != null ? dice.GetLatestDiceResult() : 0;
    }

    #endregion Get

    #region Reset
    // 新增方法：重置指定骰子的位置
    public void ResetDicePosition(int index)
    {
        Dice dice = GetDice(index);
        if (dice != null)
        {
            dice.ResetPosition();
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
            dice.ResetPosition();
            dice.transform.Rotate(dice.rotatingSpeed);

            elapsedTime += 0.01f;
            yield return null;
        }
    }

    #endregion Roll
}

