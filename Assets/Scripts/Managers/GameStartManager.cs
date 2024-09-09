using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GameStartManager : MonoBehaviour
{
    public static GameStartManager Instance;

    private Camera mainCamera;
    private Vector3 startViewPosition;
    private Quaternion startViewRotation;
    private Vector3 gamePlayPosition;
    private Quaternion gamePlayRotation;
    public float transitionDuration = 2f;
    public CamController cc; // 遊戲開始時需要顯示的物件
    public GameObject canvas;

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

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        // 設置起始和結束的相機位置和旋轉
        startViewPosition = new Vector3(336f, 323f, 364f);
        startViewRotation = Quaternion.Euler(25f, -135f, 0f); // 將四元數轉換為更易讀的歐拉角
        gamePlayPosition = new Vector3(-33f, 25f, 0);
        gamePlayRotation = Quaternion.Euler(30f, 90f, 0);

        // 設置相機初始位置和旋轉
        mainCamera.transform.position = startViewPosition;
        mainCamera.transform.rotation = startViewRotation;

        // 隱藏相機旋轉
        cc.enabled = false;
        GameManager.Instance.enabled = false;
        canvas.SetActive(true);
    }


    public void StartGame()
    {
        canvas.SetActive(false);
        StartCoroutine(StartGameSequence());

    }

    IEnumerator StartGameSequence()
    {


        // 相機轉換動畫
        Sequence cameraSequence = DOTween.Sequence();
        cameraSequence.Append(mainCamera.transform.DOMove(gamePlayPosition, transitionDuration));
        cameraSequence.Join(mainCamera.transform.DORotate(gamePlayRotation.eulerAngles, transitionDuration));

        // 等待相機動畫完成
        yield return cameraSequence.WaitForCompletion();
        // 在這裡添加其他遊戲開始邏輯
        Debug.Log("Game Started!");
        gameStarted = true;
        cc.enabled = true;
        GameManager.Instance.enabled = true;

    }

    // void SetGameObjectsActive(bool active)
    // {
    //     foreach (GameObject obj in gameObjects)
    //     {
    //         if (obj != null)
    //         {
    //             obj.SetActive(active);
    //         }
    //     }
    // }
}