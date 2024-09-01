using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    [SerializeField] private Text humanPlayersText;
    [SerializeField] private Text computerPlayersText;
    [SerializeField] private Button startGameButton;

    private void Start()
    {
        humanPlayersText.text = "1";
        computerPlayersText.text = "3";
        startGameButton.onClick.AddListener(StartGame);
    }

    private void Update()
    {
        SetPlayerNumber();

    }

    void SetPlayerNumber()
    {
        switch (humanPlayersText.text)
        {
            case "0":
                computerPlayersText.text = "4";
                break;
            case "1":
                computerPlayersText.text = "3";
                break;
            case "2":
                computerPlayersText.text = "2";
                break;
            case "3":
                computerPlayersText.text = "1";
                break;
            case "4":
                computerPlayersText.text = "0";
                break;
            default:
                computerPlayersText.text = "0";
                break;
        }
    }

    void StartGame()
    {
        if (int.TryParse(humanPlayersText.text, out int humanPlayers) &&
            int.TryParse(computerPlayersText.text, out int computerPlayers))
        {
            PlayerPrefs.SetInt("HumanPlayers", humanPlayers);
            PlayerPrefs.SetInt("ComputerPlayers", computerPlayers);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Game Scene");
        }
        else
        {
            Debug.LogError("無法解析玩家數量，無法開始遊戲！");
        }
    }
}