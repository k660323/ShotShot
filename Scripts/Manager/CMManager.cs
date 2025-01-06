using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMManager : MonoBehaviour
{
    private static CMManager instacne;
    public static CMManager Instance()
    {
        if (instacne == null)
        {
            var cm = GameObject.FindObjectOfType<CMManager>();
            if (cm != null)
                instacne = cm;
            else
            {
                GameObject go = new GameObject();
                go.AddComponent<CMManager>();
                instacne = go.GetComponent<CMManager>();
            }
                
        }
        return instacne;
    }

    public CinemachineVirtualCamera virtualCamera;
    public CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    public CinemachineConfiner cinemachineConfiner;
    // Start is called before the first frame update
    void Start()
    {
        var cms = GameObject.FindObjectsOfType<CMManager>();
        if (cms.Length != 1)
        {
            Destroy(gameObject);
            return;
        }

        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        virtualCameraNoise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        cinemachineConfiner = GetComponent<CinemachineConfiner>();
    }

    public void CameraShake(float Amplitude, float Frequency, float shakeTime)
    {
        StartCoroutine(Shakeing(Amplitude, Frequency, shakeTime));
    }

    IEnumerator Shakeing(float Amplitude, float Frequency, float shakeTime)
    {
        virtualCameraNoise.m_AmplitudeGain = Amplitude;
        virtualCameraNoise.m_FrequencyGain = Frequency;
        yield return new WaitForSeconds(shakeTime);
        virtualCameraNoise.m_AmplitudeGain = 0f;
        virtualCameraNoise.m_FrequencyGain = 0f;
    }
}
