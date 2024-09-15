using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance { get; private set; }

    public List<Dice> dices = new List<Dice>();
    public bool IsAnyDiceMoving { get; private set; }

    public static float VelocityThreshold = 0.01f;
    public static float AngularVelocityThreshold = 0.01f;
    public static float SettleTime = 0.1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
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


    #region Get
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
    // 新增方法：獲取骰子數量
    public int GetDiceCount()
    {
        return dices.Count;
    }
    // 新增方法：獲取指定骰子的結果
    public int GetDiceResult(int index)
    {
        Dice dice = GetDice(index);
        return dice != null ? dice.GetLatestDiceResult() : 0;
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

    // 新增方法：重置所有骰子的位置
    public void ResetAllDicePositions()
    {
        foreach (var dice in dices)
        {
            dice.ResetPosition();
        }
    }
    #endregion Reset



    #region Roll

    public IEnumerator AIRollDice(params Dice[] dices)
    {
        float elapsedTime = 0f;
        float diceRollDuration = Random.Range(1.5f, 3f);

        foreach (var dice in dices)
        {
            dice.transform.rotation = Random.rotation;
        }

        while (elapsedTime < diceRollDuration)
        {
            foreach (var dice in dices)
            {
                dice.ResetPosition();
                dice.transform.Rotate(dice.rotatingSpeed);
            }
            elapsedTime += 0.01f;
            yield return null;
        }
    }
    // public IEnumerator AIRollDice(params Dice[] dices)
    // {
    //     float diceRollDuration = Random.Range(1f, 2.5f);
    //     float elapsedTime = 0f;

    //     // 初始化骰子的隨機旋轉
    //     foreach (var dice in dices)
    //     {
    //         dice.transform.rotation = Random.rotation;
    //     }

    //     while (elapsedTime < diceRollDuration)
    //     {
    //         foreach (var dice in dices)
    //         {
    //             // 使用 RotateAround 來實現更自然的旋轉
    //             dice.transform.RotateAround(dice.transform.position, Random.insideUnitSphere, dice.speed * Time.deltaTime);

    //             // 根據經過的時間逐漸減緩旋轉速度
    //             float slowdownFactor = 1f - (elapsedTime / diceRollDuration);
    //             dice.transform.position = Vector3.Lerp(dice.transform.position, dice.rotatingPos, Time.deltaTime * (1f - slowdownFactor));
    //         }

    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }
}
#endregion Roll
