using UnityEngine;
using DG.Tweening;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using static GameConstants;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Animator GramophoneAnim;
    [SerializeField] private Animator BellAnim;

    [Header("Flag Settings")]
    [SerializeField] private GameObject TurnFlag;
    [SerializeField] private float flagHeight = 5f;
    [SerializeField] private float plantingDuration = 3f;

    [Header("UI Elements")]
    [SerializeField] private TextMeshPro RankingTMP;
    [SerializeField] private TextMeshPro KillerTMP;
    [SerializeField] private Button orangeButton;
    [SerializeField] private Button greenButton;
    [SerializeField] private Button blueButton;
    [SerializeField] private Button redButton;
    [SerializeField] private TextMeshPro orangeStateText;
    [SerializeField] private TextMeshPro greenStateText;
    [SerializeField] private TextMeshPro blueStateText;
    [SerializeField] private TextMeshPro redStateText;

    [Header("Game Settings")]
    [SerializeField] private float transferTime = 5f;

    private string Gold, Silver, Bronze;
    private string GoldHexCode, SilverHexCode, BronzeHexCode;


    public bool gameStarted { get; private set; } = false;

    private void Awake()
    {
        InitializeSingleton();
        InitializeReferences();
        InitializeRankingVariables();
    }

    private void Start()
    {
        InitializeButtons();
        UpdateAllStateTexts();
        TurnFlag.transform.position = FlagPosOrange;
    }

    private void Update()
    {
        UpdateUI();
        HandleInput();
    }

    #region Initialization Methods

    private void InitializeSingleton()
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

    private void InitializeReferences()
    {
        mainCamera = Camera.main;
    }

    private void InitializeRankingVariables()
    {
        Gold = Silver = Bronze = string.Empty;
        GoldHexCode = SilverHexCode = BronzeHexCode = DEFAULT_HEX_CODE;
    }

    private void InitializeButtons()
    {
        orangeButton.onClick.AddListener(() => CycleTeamState(TeamOrange.Instance));
        greenButton.onClick.AddListener(() => CycleTeamState(TeamGreen.Instance));
        blueButton.onClick.AddListener(() => CycleTeamState(TeamBlue.Instance));
        redButton.onClick.AddListener(() => CycleTeamState(TeamRed.Instance));
    }

    #endregion

    #region UI Update Methods

    private void UpdateUI()
    {
        UpdateFlagRotation();
        UpdateGramophoneAnimation();

        UpdateRankingData();
        UpdateKillerData();
    }

    private void UpdateFlagRotation()
    {
        TurnFlag.transform.LookAt(mainCamera.transform.position);
    }

    private void UpdateGramophoneAnimation()
    {
        GramophoneAnim.speed = AudioManager.Instance.BgmVolume();
    }

    private void UpdateRankingData()
    {
        Gold = LudoPieceManager.Instance.FinishedTeams.Count > 0 ? LudoPieceManager.Instance.FinishedTeams[0].name.Replace("Team ", "") : string.Empty;
        Silver = LudoPieceManager.Instance.FinishedTeams.Count > 1 ? LudoPieceManager.Instance.FinishedTeams[1].name.Replace("Team ", "") : string.Empty;
        Bronze = LudoPieceManager.Instance.FinishedTeams.Count > 2 ? LudoPieceManager.Instance.FinishedTeams[2].name.Replace("Team ", "") : string.Empty;

        GoldHexCode = LudoPieceManager.Instance.FinishedTeams.Count > 0 ? LudoPieceManager.Instance.FinishedTeams[0].HexCode : DEFAULT_HEX_CODE;
        SilverHexCode = LudoPieceManager.Instance.FinishedTeams.Count > 1 ? LudoPieceManager.Instance.FinishedTeams[1].HexCode : DEFAULT_HEX_CODE;
        BronzeHexCode = LudoPieceManager.Instance.FinishedTeams.Count > 2 ? LudoPieceManager.Instance.FinishedTeams[2].HexCode : DEFAULT_HEX_CODE;

        RankingTMP.text =
            $"<color=#FFD700>Gold:</color> <color={GoldHexCode}>{Gold}</color>\n" +
            $"<color=#E6E8FA>Silver:</color> <color={SilverHexCode}>{Silver}</color>\n" +
            $"<color=#D2691E>Bronze:</color> <color={BronzeHexCode}>{Bronze}</color>";
    }

    private void UpdateKillerData()
    {
        List<Team> teams = new List<Team>
        {
            TeamOrange.Instance,
            TeamGreen.Instance,
            TeamBlue.Instance,
            TeamRed.Instance
        };

        var sortedTeams = teams.OrderByDescending(t => t.GetKillCount()).ToList();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<align=center><size=150%><color=#FF0000><b>Top Killer</b></color></size></align>");

        foreach (var teamInfo in sortedTeams)
        {
            int killCount = teamInfo.GetKillCount();
            sb.AppendLine($"<align=left><color={teamInfo.HexCode}>{teamInfo.Name}: {killCount}</color></align>");
        }

        KillerTMP.text = sb.ToString();
    }

    #endregion

    #region Input Handling

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                HandleObjectClick(hit.collider.gameObject.name);
            }
        }
    }

    private void HandleObjectClick(string objectName)
    {
        switch (objectName)
        {
            case "Bell":
                HandleBellClick();
                break;
            case "Restart":
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case "Exit":
                Application.Quit();
                break;
            case "Cat":
                AudioManager.Instance.PlaySFX("Meow");
                break;
            case "OrangeBTN":
                CycleTeamState(TeamOrange.Instance);
                break;
            case "GreenBTN":
                CycleTeamState(TeamGreen.Instance);
                break;
            case "BlueBTN":
                CycleTeamState(TeamBlue.Instance);
                break;
            case "RedBTN":
                CycleTeamState(TeamRed.Instance);
                break;
            case "Close":
                canvas.SetActive(false);
                CameraManager.Instance.MoveCameraTo(GameView, transferTime);
                StartCoroutine(LateSet(transferTime));
                break;
        }
    }

    private void HandleBellClick()
    {
        BellAnim.SetTrigger("BeClicked");
        AudioManager.Instance.PlaySFX("Bell");
        float targetVolume = AudioManager.Instance.BgmVolume() == 1f ? 0f : 1f;
        AudioManager.Instance.FadeBGM(5f, targetVolume);
    }

    #endregion

    #region Team State Management

    private void CycleTeamState(Team team)
    {
        team.CycleState();
        UpdateStateText(team);
    }

    private void UpdateStateText(Team team)
    {
        TextMeshPro stateText = GetStateTextForTeam(team);
        stateText.text = team.GetStateString();
    }

    private TextMeshPro GetStateTextForTeam(Team team)
    {
        if (team == TeamOrange.Instance) return orangeStateText;
        else if (team == TeamGreen.Instance) return greenStateText;
        else if (team == TeamBlue.Instance) return blueStateText;
        else if (team == TeamRed.Instance) return redStateText;
        return null;
    }

    private void UpdateAllStateTexts()
    {
        UpdateStateText(TeamOrange.Instance);
        UpdateStateText(TeamGreen.Instance);
        UpdateStateText(TeamBlue.Instance);
        UpdateStateText(TeamRed.Instance);
    }

    #endregion

    #region Game Flow Methods

    private IEnumerator LateSet(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Debug.Log("Game Started!");
        gameStarted = true;
        GameManager.Instance.START_GAME();
        CameraManager.Instance.SetCameraOffset();
    }

    public void OnClassicButtonClicked()
    {
        GameManager.Instance.CurrentGameMode = GameMode.Classic;
        CameraManager.Instance.MoveCameraTo(SettingView, 3f);
        canvas.SetActive(false);
    }

    public void OnCrazyButtonClicked()
    {
        GameManager.Instance.CurrentGameMode = GameMode.Crazy;
        CameraManager.Instance.MoveCameraTo(SettingView, 3f);
        canvas.SetActive(false);
    }

    public void SetUpTurnFlag()
    {
        Vector3 targetPosition = GetTurnFlagPosition();
        StartCoroutine(PlantFlag(TurnFlag, targetPosition));
    }

    private Vector3 GetTurnFlagPosition()
    {
        return GameManager.Instance.CurrentPlayerTurn switch
        {
            1 => FlagPosOrange,
            2 => FlagPosGreen,
            3 => FlagPosBlue,
            4 => FlagPosRed,
            _ => FlagPosDefault
        };
    }

    private IEnumerator PlantFlag(GameObject flag, Vector3 targetPosition)
    {
        Vector3 startPosition = targetPosition + Vector3.up * flagHeight;
        flag.transform.position = startPosition;

        Sequence plantSequence = DOTween.Sequence();
        plantSequence.Append(flag.transform.DOMove(targetPosition, plantingDuration));

        yield return plantSequence.WaitForCompletion();
    }

    #endregion
}



// private TextMeshPro GetStateTextForTeam(Team team)
// {
//     if (team == Team.Orange) return orangeStateText;
//     if (team == Team.Green) return greenStateText;
//     if (team == Team.Blue) return blueStateText;
//     if (team == Team.Red) return redStateText;
//     return null;
// }
