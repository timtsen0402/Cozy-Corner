using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BackGroundManager : MonoBehaviour
{
    public static BackGroundManager Instance;

    private Camera mainCamera;

    public Animator GramophoneAnim;
    public GameObject TurnFlag;
    public TextMeshPro RankingTMP;
    public TextMeshPro KillerTMP;

    public float flagHeight = 5f; // 旗子初始出现的高度
    public float plantingDuration = 3f; // 插旗动作的持续时间

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
    void Start()
    {
        mainCamera = Camera.main;
        TurnFlag.transform.position = new Vector3(-6.7f, 0.61f, 12.25f);
        RankingTMP.text = "<color=#FFD700>Gold:</color>\n<color=#E6E8FA>Silver:</color>\n<color=#D2691E>Bronze:</color>";
        KillerTMP.text = $"<align=center><size=150%><b>Top Killer</b></size></align>\n" +
                               $"<align=left><color=#FF8C00>Orange:</color></align>\n" +
                               $"<align=left><color=#228B22>Green:</color></align>\n" +
                               $"<align=left><color=#00008B>Blue:</color></align>\n" +
                               $"<align=left><color=#FF0000>Red:</color></align>";
    }

    void Update()
    {
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
                // Debug.Log(objectName);
                switch (objectName)
                {
                    case "Gramophone":
                        if (AudioManager.Instance.BgmVolume() == 1f)
                        {
                            AudioManager.Instance.FadeBGM(5f, 0f);
                            // GramophoneAnim.speed = 0f;
                        }
                        else
                        {
                            AudioManager.Instance.FadeBGM(5f, 1f);
                            // GramophoneAnim.speed = 1f;
                        }
                        break;
                    case "Cat":
                        AudioManager.Instance.PlaySFX("Meow");
                        break;
                }
            }
        }
    }

    public void SetUpTurnFlag()
    {

        Vector3 targetPosition = Vector3.zero;

        switch (GameManager.Instance.CurrentPlayerTurn)
        {
            case 1:
                targetPosition = new Vector3(-6.7f, 0.61f, 12.25f);
                break;
            case 2:
                targetPosition = new Vector3(6.7f, 0.61f, 12.25f);
                break;
            case 3:
                targetPosition = new Vector3(6.7f, 0.61f, -12.25f);
                break;
            case 4:
                targetPosition = new Vector3(-6.7f, 0.61f, -12.25f);
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
