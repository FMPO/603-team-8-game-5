using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Camera mainCamera;
    public float shakeAmount = 0;
    Vector3 camPos;
    Vector3 camOffset = Vector3.zero;

   
    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }


    public void Shake(float amount, float length)
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        shakeAmount = amount;
        //tempCamPos = mainCamera.transform.position;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", length);
    }

    private void Update()
    {
        //if (mainCamera == null)
        //{
        //    mainCamera = Camera.main;
        //}
    }

    void DoShake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (shakeAmount > 0)
        {
            camPos = mainCamera.transform.position - camOffset;
            Vector3 tempPos = mainCamera.transform.position;

            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;

            //tempPos.x += offsetX;
            //tempPos.y += offsetY;
            camOffset = new Vector3(offsetX, offsetY, 0);

            mainCamera.transform.position = camPos + camOffset;
        }
        else
        {
            CancelInvoke("DoShake");
            mainCamera.transform.localPosition = camPos;
        }

    }

    void StopShake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        CancelInvoke("DoShake");
        mainCamera.transform.localPosition = camPos;
    }

    public void GetCamera()
    {
        mainCamera = Camera.main;
    }
}
