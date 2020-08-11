using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeControl : MonoBehaviour
{
    public ParticleSystem leave;
    public ParticleSystem subParticle;

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

    private void OnParticleCollision(GameObject other)
    {
        if (other.name == gameObject.name) return;
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        ParticlePhysicsExtensions.GetCollisionEvents(system, other, collisionEvents);
        for (int i = 0; i< collisionEvents.Count; i++)
        {
            //EmitAtLocation(collisionEvents[i]);
        }
    }

    void EmitAtLocation(ParticleCollisionEvent particleCollisionEvents)
    {
        leave.transform.position = particleCollisionEvents.intersection;
        leave.transform.rotation = Quaternion.Euler(0,0,0);
        leave.Play();
        //ParticleSystem treeFade = Instantiate(subParticle, particleCollisionEvent.intersection, Quaternion.LookRotation(particleCollisionEvent.normal));
    }
}
