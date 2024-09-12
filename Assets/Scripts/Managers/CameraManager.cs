using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public Camera MainCamera { get; private set; }

    // // 定義所有相機視圖
    // public static readonly CameraView TitleView = new CameraView(new Vector3(336f, 323f, 364f), Quaternion.Euler(25f, -135f, 0f));
    // public static readonly CameraView GameView = new CameraView(new Vector3(-33f, 25f, 0), Quaternion.Euler(30f, 90f, 0));
    // public static readonly CameraView SettingView = new CameraView(new Vector3(268.3f, -16f, 74.4f), Quaternion.Euler(0, -134.3f, 0));

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

        MainCamera = Camera.main;
    }

    public void MoveCameraTo(CameraView view, float duration)
    {
        Sequence cameraSequence = DOTween.Sequence();
        cameraSequence.Append(MainCamera.transform.DOMove(view.Position, duration).SetEase(Ease.InOutSine));
        cameraSequence.Join(MainCamera.transform.DORotateQuaternion(view.Rotation, duration).SetEase(Ease.InOutSine));
        cameraSequence.Play();
    }

    public void SetInitialCameraPosition(CameraView view)
    {
        MainCamera.transform.position = view.Position;
        MainCamera.transform.rotation = view.Rotation;
    }
}
