using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSelectionManager : MonoBehaviour
{
    public Slider humanPlayersSlider;
    public Slider computerPlayersSlider;
    public Button startGameButton;

    private void Start()
    {
        SetupSliders();
        UpdatePlayerCounts();
        startGameButton.onClick.AddListener(StartGame);
    }

    private void SetupSliders()
    {
        humanPlayersSlider.onValueChanged.AddListener(delegate { UpdatePlayerCounts(); });
        computerPlayersSlider.onValueChanged.AddListener(delegate { UpdatePlayerCounts(); });
    }

    private void UpdatePlayerCounts()
    {
        int humanPlayers = Mathf.RoundToInt(humanPlayersSlider.value);
        int computerPlayers = Mathf.RoundToInt(computerPlayersSlider.value);

        int totalPlayers = humanPlayers + computerPlayers;
        startGameButton.interactable = (totalPlayers > 1 && totalPlayers <= 4);
    }

    private void StartGame()
    {
        int humanPlayers = Mathf.RoundToInt(humanPlayersSlider.value);
        int computerPlayers = Mathf.RoundToInt(computerPlayersSlider.value);

        PlayerPrefs.SetInt("HumanPlayers", humanPlayers);
        PlayerPrefs.SetInt("ComputerPlayers", computerPlayers);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Game Scene");
    }
}