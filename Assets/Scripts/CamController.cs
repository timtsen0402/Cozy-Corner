using System.Collections;
using UnityEngine;

public class CamController : MonoBehaviour
{
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

    Vector3 offset;

    void Start()
    {
        offset = transform.position - Target.transform.position;
    }

    void LateUpdate()
    {
        transform.position = Target.transform.position + offset;


        transform.LookAt(new Vector3(Target.transform.position.x, Target.transform.position.y + 6f, Target.transform.position.z));



        offset = Scrollview(offset, scrollSpeed, minDistance, maxDistance);
        Rotateview(Target);

    }
    Vector3 Scrollview(Vector3 offset, float scrollSpeed, float start, float end)
    {
        float distance = offset.magnitude;
        distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        distance = Mathf.Clamp(distance, start, end);
        offset = offset.normalized * distance;
        return offset;
    }

    void Rotateview(GameObject Target)
    {
        Vector3 posAboveTarget = new Vector3(Target.transform.position.x, Target.transform.position.y + 1, Target.transform.position.z);
        Vector3 up = posAboveTarget - Target.transform.position;


        float camAngle = transform.eulerAngles.x;

        if (Input.GetMouseButton(1))
        {
            if (camAngle < minAngle)
            {
                transform.RotateAround(Target.transform.position, up, rotateSpeed * Input.GetAxis("Mouse X") - (camAngle - minAngle));
                transform.RotateAround(Target.transform.position, transform.right, -rotateSpeed * Input.GetAxis("Mouse Y") - (camAngle - minAngle));

            }

            else if (camAngle > maxAngle)
            {
                transform.RotateAround(Target.transform.position, up, rotateSpeed * Input.GetAxis("Mouse X") - (camAngle - maxAngle));
                transform.RotateAround(Target.transform.position, transform.right, -rotateSpeed * Input.GetAxis("Mouse Y") - (camAngle - maxAngle));
            }
            else
            {
                transform.RotateAround(Target.transform.position, up, rotateSpeed * Input.GetAxis("Mouse X"));
                transform.RotateAround(Target.transform.position, transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));
            }

            offset = transform.position - Target.transform.position;
        }
    }
}