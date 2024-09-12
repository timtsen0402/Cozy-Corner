using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;
using static AIStrategies;
using static GameConstants;

public class GameStartManager : MonoBehaviour
{
    public static GameStartManager Instance;

    private Camera mainCamera;
    public float transitionDuration = 2f;
    public CamController cc; // 遊戲開始時需要顯示的物件
    public GameObject canvas;
    [SerializeField] private float transferTime = 5;

    public Difficulty difficulty { get; private set; } = Difficulty.Peaceful;

    public bool gameStarted { get; private set; } = false;

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
        AudioManager.Instance.PlayBgmRandomly();

        CameraManager.Instance.SetInitialCameraPosition(TitleView);

        // 隱藏相機旋轉
        cc.enabled = false;
        GameManager.Instance.enabled = false;
        canvas.SetActive(true);
    }


    public void StartGame()
    {
        canvas.SetActive(false);
        CameraManager.Instance.MoveCameraTo(GameView, transferTime);
        // 在這裡添加其他遊戲開始邏輯
        StartCoroutine(LateSet(transferTime));
    }

    IEnumerator LateSet(float second)
    {
        yield return new WaitForSeconds(second);
        Debug.Log("Game Started!");
        gameStarted = true;
        cc.enabled = true;
        GameManager.Instance.enabled = true;
    }
}