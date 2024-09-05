using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundManager : MonoBehaviour
{
    private Camera mainCamera;

    public Animator GramophoneAnim;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
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
                switch (objectName)
                {
                    case "Gramophone":
                        if (AudioManager.Instance.BgmVolume() == 1f)
                        {
                            AudioManager.Instance.FadeBGM(5f, 0f);
                            GramophoneAnim.speed = 0f;
                        }
                        else
                        {
                            AudioManager.Instance.FadeBGM(5f, 1f);
                            GramophoneAnim.speed = 1f;
                        }
                        break;
                    case "Cat":
                        AudioManager.Instance.PlaySFX("Meow");
                        break;
                }
            }
        }
    }

}
