using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameConstants;

public class BackGroundManager : MonoBehaviour
{
    public static BackGroundManager Instance;

    private Camera mainCamera;

    public GameObject canvas;

    [Header("Animation")]
    public Animator GramophoneAnim;
    public Animator BellAnim;
    [Header("Flag Setting")]
    public GameObject TurnFlag;
    public float flagHeight = 5f; // 旗子初始出现的高度
    public float plantingDuration = 3f; // 插旗动作的持续时间
    [Header("TextMeshPro")]
    public TextMeshPro RankingTMP;
    public TextMeshPro KillerTMP;

    [SerializeField] private Button orangeButton;
    [SerializeField] private Button greenButton;
    [SerializeField] private Button blueButton;
    [SerializeField] private Button redButton;

    [SerializeField] private TextMeshPro orangeStateText;
    [SerializeField] private TextMeshPro greenStateText;
    [SerializeField] private TextMeshPro blueStateText;
    [SerializeField] private TextMeshPro redStateText;

    private string Gold;
    private string Silver;
    private string Bronze;

    private string GoldHexCode;
    private string SilverHexCode;
    private string BronzeHexCode;

    string defaultHexCode = "#FFFFFF";

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
        mainCamera = Camera.main;
        TurnFlag.transform.position = new Vector3(-15f, 0.61f, 15f);
        Gold = string.Empty;
        Silver = string.Empty;
        Bronze = string.Empty;
        GoldHexCode = defaultHexCode;
        SilverHexCode = defaultHexCode;
        BronzeHexCode = defaultHexCode;
    }
    private void Start()
    {
        InitializeButtons();
        UpdateAllStateTexts();
    }

    private void InitializeButtons()
    {
        orangeButton.onClick.AddListener(() => CycleTeamState(Team.Orange));
        greenButton.onClick.AddListener(() => CycleTeamState(Team.Green));
        blueButton.onClick.AddListener(() => CycleTeamState(Team.Blue));
        redButton.onClick.AddListener(() => CycleTeamState(Team.Red));
    }

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
        if (team == Team.Orange) return orangeStateText;
        if (team == Team.Green) return greenStateText;
        if (team == Team.Blue) return blueStateText;
        if (team == Team.Red) return redStateText;
        return null;
    }

    private void UpdateAllStateTexts()
    {
        UpdateStateText(Team.Orange);
        UpdateStateText(Team.Green);
        UpdateStateText(Team.Blue);
        UpdateStateText(Team.Red);
    }

    void Update()
    {
        // SetUpSetting();
        SetUpRanking();

        TurnFlag.transform.LookAt(Camera.main.transform.position);
        GramophoneAnim.speed = AudioManager.Instance.BgmVolume();

        // 檢測滑鼠左鍵點擊
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 進行射線檢測
            if (Physics.Raycast(ray, out hit))
            {
                // 獲取點擊物體的名稱
                string objectName = hit.collider.gameObject.name;
                //Debug.Log(objectName);
                switch (objectName)
                {
                    case "Bell":
                        BellAnim.SetTrigger("BeClicked");
                        AudioManager.Instance.PlaySFX("Bell");
                        if (AudioManager.Instance.BgmVolume() == 1f)
                        {
                            AudioManager.Instance.FadeBGM(5f, 0f);
                        }
                        else
                        {
                            AudioManager.Instance.FadeBGM(5f, 1f);
                        }
                        break;
                    case "Restart":
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        break;
                    case "Exit":
                        Application.Quit();
                        break;
                    case "Gramophone":
                        //
                        break;
                    case "Cat":
                        AudioManager.Instance.PlaySFX("Meow");
                        break;
                    case "OrangeBTN":
                        CycleTeamState(Team.Orange);
                        break;
                    case "GreenBTN":
                        CycleTeamState(Team.Green);
                        break;
                    case "BlueBTN":
                        CycleTeamState(Team.Blue);
                        break;
                    case "RedBTN":
                        CycleTeamState(Team.Red);
                        break;
                    case "Close":
                        CameraManager.Instance.MoveCameraTo(TitleView, 3f);
                        break;

                }
            }
        }
    }
    public void SetUpRanking()
    {
        if (LudoPieceManager.Instance.FinishedTeams == null)
        {
            Debug.LogWarning("FinishedTeams is null");
            return;
        }
        Gold =
        LudoPieceManager.Instance.FinishedTeams.Count > 0 ? LudoPieceManager.Instance.FinishedTeams[0].name : string.Empty;
        Silver =
        LudoPieceManager.Instance.FinishedTeams.Count > 1 ? LudoPieceManager.Instance.FinishedTeams[1].name : string.Empty;
        Bronze =
        LudoPieceManager.Instance.FinishedTeams.Count > 2 ? LudoPieceManager.Instance.FinishedTeams[2].name : string.Empty;

        GoldHexCode =
        LudoPieceManager.Instance.FinishedTeams.Count > 0 ? LudoPieceManager.Instance.FinishedTeams[0].HexCode : defaultHexCode;
        SilverHexCode =
        LudoPieceManager.Instance.FinishedTeams.Count > 1 ? LudoPieceManager.Instance.FinishedTeams[1].HexCode : defaultHexCode;
        BronzeHexCode =
        LudoPieceManager.Instance.FinishedTeams.Count > 2 ? LudoPieceManager.Instance.FinishedTeams[2].HexCode : defaultHexCode;

        RankingTMP.text =
        $"<color=#FFD700>Gold:</color> <color={GoldHexCode}>{Gold}</color>\n" +
        $"<color=#E6E8FA>Silver:</color> <color={SilverHexCode}>{Silver}</color>\n" +
        $"<color=#D2691E>Bronze:</color> <color={BronzeHexCode}>{Bronze}</color>";
        KillerTMP.text = UpdateKillCountDisplay();
    }
    public string UpdateKillCountDisplay()
    {
        var teamKillCounts = new[]
        {
            new { Team = Team.Orange, HexCode = "#FF8C00" },
            new { Team = Team.Green, HexCode = "#228B22" },
            new { Team = Team.Blue, HexCode = "#1E90FF" },
            new { Team = Team.Red, HexCode = "#CD5C5C" }
        };

        var sortedTeams = teamKillCounts
            .OrderByDescending(t => t.Team.GetKillCount())
            .ToList();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<align=center><size=150%><color=#FF0000><b>Top Killer</b></color></size></align>");

        foreach (var teamInfo in sortedTeams)
        {
            int killCount = teamInfo.Team.GetKillCount();
            sb.AppendLine($"<align=left><color={teamInfo.HexCode}>{teamInfo.Team.Name}: {killCount}</color></align>");
        }

        return sb.ToString();
    }
    public void SetUpTurnFlag()
    {

        Vector3 targetPosition = Vector3.zero;

        switch (GameManager.Instance.CurrentPlayerTurn)
        {
            case 1:
                targetPosition = new Vector3(-15f, 0.61f, 15f);
                break;
            case 2:
                targetPosition = new Vector3(15f, 0.61f, 15f);
                break;
            case 3:
                targetPosition = new Vector3(15f, 0.61f, -15f);
                break;
            case 4:
                targetPosition = new Vector3(-15f, 0.61f, -15f);
                break;
            default:
                targetPosition = new Vector3(0, 0.61f, 0);
                break;
        }

        StartCoroutine(PlantFlag(TurnFlag, targetPosition));
    }

    private IEnumerator PlantFlag(GameObject flag, Vector3 targetPosition)
    {
        // 设置旗子的初始位置（在目标位置的上方）
        Vector3 startPosition = targetPosition + Vector3.up * flagHeight;
        flag.transform.position = startPosition;

        // 创建插旗的动画序列
        Sequence plantSequence = DOTween.Sequence();

        // 向下移动旗子
        plantSequence.Append(flag.transform.DOMove(targetPosition, plantingDuration));

        // 等待动画完成
        yield return plantSequence.WaitForCompletion();
    }
    public void OnSettingsButtonClicked()
    {
        CameraManager.Instance.MoveCameraTo(SettingView, 3f); // 使用3秒的過渡時間，您可以根據需要調整
    }
}
