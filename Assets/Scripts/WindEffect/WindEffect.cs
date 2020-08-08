using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour
{
    [SerializeField]
    Transform playerTransform;

    bool IsBeginning = true;

    [SerializeField]
    ParticleSystem startWind;
    [SerializeField]
    ParticleSystem leadWind;

    [SerializeField]
    float height;
    [SerializeField]
    float beginningLoopTime = 1f;
    [SerializeField]
    float beginningStartTime = 0f;

    [SerializeField]
    float radius;
    [SerializeField]
    float loopTime = 1f;
    [SerializeField]
    float startTime = 0f;

    public void ChangeState()
    {
        CancelInvoke();
        if (IsBeginning)
        {
            leadWind.gameObject.SetActive(true);
            startWind.gameObject.SetActive(false);
            InvokeRepeating("AutoPlayLeadWind", startTime, loopTime);
        }
        else
        {
            leadWind.gameObject.SetActive(false);
            startWind.gameObject.SetActive(true);
            InvokeRepeating("AutoPlayStartWind", beginningStartTime, beginningLoopTime);
        }
        IsBeginning = !IsBeginning;
        
    }

    void AutoPlayLeadWind()
    {
        float newPosx = Random.Range(-radius, radius);
        float newPosy = Random.Range(-radius, radius);
        leadWind.gameObject.transform.position = playerTransform.position + new Vector3(newPosx,
                                                                    newPosy,
                                                                    0);
        leadWind.Play(true);
    }
    void AutoPlayStartWind()
    {
        float newPosy = Random.Range(0, height);
        startWind.gameObject.transform.position = playerTransform.position + new Vector3(startWind.transform.position.x,
                                                                                         newPosy,
                                                                                         0);
        startWind.Play(true);
    }
    private void Start()
    {
        startWind.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        leadWind.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        InvokeRepeating("AutoPlayStartWind", beginningStartTime, beginningLoopTime);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeState();
        }
    }
}
