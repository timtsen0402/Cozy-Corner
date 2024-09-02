using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance { get; private set; }

    public List<Dice> dices = new List<Dice>();
    public bool IsAnyDiceMoving { get; private set; }

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

    private void Update()
    {
        IsAnyDiceMoving = CheckIfAnyDiceMoving();
    }

    public bool CheckIfAnyDiceMoving()
    {
        foreach (var dice in dices)
        {
            if (!dice.isStop())
            {
                return true;
            }
        }
        return false;
    }

    public int GetTotalDiceResult()
    {
        int total = 0;
        foreach (var dice in dices)
        {
            total += dice.GetLatestDiceResult();
        }
        return total;
    }

    public static int GetDiceResult(string result)
    {
        switch (result)
        {
            case "surface1": return 1;
            case "surface2": return 2;
            case "surface3": return 3;
            case "surface4": return 4;
            case "surface5": return 5;
            case "surface6": return 6;
            default: return 0;
        }
    }

    // 新增方法：獲取指定索引的骰子
    public Dice GetDice(int index)
    {
        if (index >= 0 && index < dices.Count)
        {
            return dices[index];
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

    // 新增方法：重置指定骰子的位置
    public void ResetDicePosition(int index)
    {
        Dice dice = GetDice(index);
        if (dice != null)
        {
            dice.ResetPosition();
        }
    }

    // 新增方法：重置所有骰子的位置
    public void ResetAllDicePositions()
    {
        foreach (var dice in dices)
        {
            dice.ResetPosition();
        }
    }

    // 新增方法：獲取骰子數量
    public int GetDiceCount()
    {
        return dices.Count;
    }
}