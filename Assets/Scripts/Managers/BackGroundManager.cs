using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;

public class BackGroundManager : MonoBehaviour
{
    public static BackGroundManager Instance;

    private Camera mainCamera;

    public Animator GramophoneAnim;
    public Animator BellAnim;
    public GameObject TurnFlag;
    public TextMeshPro RankingTMP;
    public TextMeshPro KillerTMP;

    public float flagHeight = 5f; // 旗子初始出现的高度
    public float plantingDuration = 3f; // 插旗动作的持续时间

    public string Gold;
    public string Silver;
    public string Bronze;

    public string GoldHexCode;
    public string SilverHexCode;
    public string BronzeHexCode;

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
    }
    void Start()
    {
        mainCamera = Camera.main;
        TurnFlag.transform.position = new Vector3(-15f, 0.61f, 15f);
        Gold = string.Empty;
        Silver = string.Empty;
        Bronze = string.Empty;
        GoldHexCode = defaultHexCode;
        SilverHexCode = defaultHexCode;
        BronzeHexCode = defaultHexCode;
    }

    void Update()
    {

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
                Debug.Log(objectName);
                // Debug.Log(objectName);
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
                }
            }
        }
    }
    public void SetUpRanking()
    {
        Gold =
        LudoPieceManager.Instance.FinishedColors.Count > 0 ? LudoPieceManager.Instance.FinishedColors[0].ToString() : string.Empty;
        Silver =
        LudoPieceManager.Instance.FinishedColors.Count > 1 ? LudoPieceManager.Instance.FinishedColors[1].ToString() : string.Empty;
        Bronze =
        LudoPieceManager.Instance.FinishedColors.Count > 2 ? LudoPieceManager.Instance.FinishedColors[2].ToString() : string.Empty;

        GoldHexCode =
        LudoPieceManager.Instance.FinishedColors.Count > 0 ? LudoPieceManager.Instance.GetHexCode(LudoPieceManager.Instance.FinishedColors[0]) : defaultHexCode;
        SilverHexCode =
        LudoPieceManager.Instance.FinishedColors.Count > 1 ? LudoPieceManager.Instance.GetHexCode(LudoPieceManager.Instance.FinishedColors[1]) : defaultHexCode;
        BronzeHexCode =
        LudoPieceManager.Instance.FinishedColors.Count > 2 ? LudoPieceManager.Instance.GetHexCode(LudoPieceManager.Instance.FinishedColors[2]) : defaultHexCode;

        RankingTMP.text =
        $"<color=#FFD700>Gold:</color> <color={GoldHexCode}>{Gold}</color>\n" +
        $"<color=#E6E8FA>Silver:</color> <color={SilverHexCode}>{Silver}</color>\n" +
        $"<color=#D2691E>Bronze:</color> <color={BronzeHexCode}>{Bronze}</color>";
        KillerTMP.text = UpdateKillCountDisplay();
    }
    public string UpdateKillCountDisplay()
    {
        var colorKillCounts = new[]
        {
            new { Color = LudoPiece.PieceColor.Orange, HexCode = "#FF8C00" },
            new { Color = LudoPiece.PieceColor.Green, HexCode = "#228B22" },
            new { Color = LudoPiece.PieceColor.Blue, HexCode = "#1E90FF" },
            new { Color = LudoPiece.PieceColor.Red, HexCode = "#CD5C5C" }
        };

        var sortedColors = colorKillCounts
            .OrderByDescending(c => LudoPieceManager.Instance.GetColorKillCount(c.Color))
            .ToList();

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<align=center><size=150%><color=#FF0000><b>Top Killer</b></color></size></align>");

        foreach (var color in sortedColors)
        {
            int killCount = LudoPieceManager.Instance.GetColorKillCount(color.Color);
            sb.AppendLine($"<align=left><color={color.HexCode}>{color.Color}: {killCount}</color></align>");
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

}
