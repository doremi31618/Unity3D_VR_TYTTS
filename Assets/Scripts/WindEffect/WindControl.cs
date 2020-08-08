using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindControl : MonoBehaviour
{
    [SerializeField]
    Transform targetTransform;
    [SerializeField]
    float radius;
    [SerializeField]
    float loopTime = 1f;
    [SerializeField]
    float startTime = 1f;

    ParticleSystem system
    {
        get
        {
            if (_CachedSystem == null)
                _CachedSystem = GetComponent<ParticleSystem>();
            return _CachedSystem;
        }
    }
    private ParticleSystem _CachedSystem;

    void StopEmitting()
    {
        system.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
    void AutoPlayControl()
    {
        float newPosx = Random.Range(-radius, radius);
        float newPosy = Random.Range(-radius, radius);
        transform.position = targetTransform.position + new Vector3(newPosx,
                                                                    newPosy,
                                                                    0);
        system.Play(true);
        Invoke("StopEmitting", 1f);
    }
    private void Start()
    {
        InvokeRepeating("AutoPlayControl", startTime, loopTime);
    }
}
