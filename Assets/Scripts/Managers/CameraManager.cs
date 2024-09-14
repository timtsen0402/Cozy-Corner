using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public Camera MainCamera { get; private set; }

    [Header("Target")]
    [SerializeField] private GameObject Target;

    [Header("Speed Setting")]
    [SerializeField] private float rotateSpeed = 2f;
    [SerializeField] private float scrollSpeed = 30f;

    [Header("Scrollview Distance Setting")]
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 80f;

    [Header("Rotateview Angle Setting")]
    [SerializeField] private float minAngle = 5f;
    [SerializeField] private float maxAngle = 85f;

    private Vector3 offset;

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

    void LateUpdate()
    {
        if (!UIManager.Instance.gameStarted) return;
        CamControl();
    }

    public void MoveCameraTo(CameraView view, float duration)
    {
        Sequence cameraSequence = DOTween.Sequence();
        cameraSequence.Append(MainCamera.transform.DOMove(view.Position, duration).SetEase(Ease.InOutSine));
        cameraSequence.Join(MainCamera.transform.DORotateQuaternion(view.Rotation, duration).SetEase(Ease.InOutSine));
        cameraSequence.Play();
    }

    public void SetCameraOffset()
    {
        offset = MainCamera.transform.position - Target.transform.position;
    }

    public void SetInitialCameraPosition(CameraView view)
    {
        MainCamera.transform.position = view.Position;
        MainCamera.transform.rotation = view.Rotation;
    }

    private void CamControl()
    {
        MainCamera.transform.position = Target.transform.position + offset;
        MainCamera.transform.LookAt(new Vector3(Target.transform.position.x, Target.transform.position.y + 6f, Target.transform.position.z));
        offset = Scrollview(offset, scrollSpeed, minDistance, maxDistance);
        Rotateview(Target);
    }

    private Vector3 Scrollview(Vector3 offset, float scrollSpeed, float start, float end)
    {
        float distance = offset.magnitude;
        distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        distance = Mathf.Clamp(distance, start, end);
        offset = offset.normalized * distance;
        return offset;
    }

    private void Rotateview(GameObject Target)
    {
        if (!Input.GetMouseButton(1)) return;


        Vector3 posAboveTarget = new Vector3(Target.transform.position.x, Target.transform.position.y + 1, Target.transform.position.z);
        Vector3 up = posAboveTarget - Target.transform.position;


        float camAngle = MainCamera.transform.eulerAngles.x;


        if (camAngle < minAngle)
        {
            MainCamera.transform.RotateAround(Target.transform.position, up, rotateSpeed * Input.GetAxis("Mouse X") - (camAngle - minAngle));
            MainCamera.transform.RotateAround(Target.transform.position, MainCamera.transform.right, -rotateSpeed * Input.GetAxis("Mouse Y") - (camAngle - minAngle));

        }

        else if (camAngle > maxAngle)
        {
            MainCamera.transform.RotateAround(Target.transform.position, up, rotateSpeed * Input.GetAxis("Mouse X") - (camAngle - maxAngle));
            MainCamera.transform.RotateAround(Target.transform.position, MainCamera.transform.right, -rotateSpeed * Input.GetAxis("Mouse Y") - (camAngle - maxAngle));
        }
        else
        {
            MainCamera.transform.RotateAround(Target.transform.position, up, rotateSpeed * Input.GetAxis("Mouse X"));
            MainCamera.transform.RotateAround(Target.transform.position, MainCamera.transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));
        }

        offset = MainCamera.transform.position - Target.transform.position;
    }

}
