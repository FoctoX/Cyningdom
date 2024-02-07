using Cinemachine;
using UnityEngine;

public class CinemachineControllerScript : MonoBehaviour
{   
    public static CinemachineControllerScript Instance { get; private set; }

    private CinemachineVirtualCamera virtualCamera;
    private float shakeTime;
    private float shakeTimeMax;
    private float startIntensity;

    private void Awake()
    {
        Instance = this;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTime > 0f)
        {
            shakeTime -= Time.unscaledDeltaTime;
            if (shakeTime <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
                Mathf.Lerp(startIntensity, 0f, 1 - (shakeTime/shakeTimeMax));
            }
        }
        
    }

    public void CameraShake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTime = time;
        shakeTimeMax = time;
        startIntensity = intensity;
    }
}
